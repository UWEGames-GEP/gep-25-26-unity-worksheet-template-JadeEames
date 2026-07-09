using System;
using UnityEngine;

namespace InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        public int size = 16;
        public ItemStack[] slots { get; private set; }
        public ItemDatabase database;

        [SerializeField] private GameObject baseItemPrefab = null;
        [SerializeField] private Vector3 droppedItemPosOffset = new Vector3(0,1,1);

        private void Awake()
        {
            // Initialise slots up to max size.
            slots = new ItemStack[size];

            for (int i = 0; i < size; i++)
            {
                slots[i] = ItemStack.Empty;
            }
        }

        private bool ValidIndex(int index)
        {
            return index >= 0 && index < slots.Length;
        }

        /// <summary>
        /// Adds amount of item to iventory into ANY AVAILABLE SPACE (starting with slots of same item id).
        /// </summary>
        /// <param name="item_id">ID of item to add.</param>
        /// <param name="amount">Amount/Count of item to add.</param>
        /// <returns>Returns SUCCESSFUL additions.</returns> 
        public int AddItems(int item_id, int amount)
        {
            // Cannot add invalid item id or 0 items. 
            if (item_id <= 0 || amount <= 0) return 0;
            var item = database.Get(item_id);
            if (item == null) return 0;

            int added = 0;

            // Fill existing stacks
            for (int i = 0; i < slots.Length && amount > 0; i++)
            {
                var stack = slots[i];

                if (stack.IsEmpty) continue;

                // If slot is not of the same type - skip.
                if (stack.item_id != item_id) continue;

                // Calculate how many will fit in indexed slot. 
                int available_space = item.stackLimit - stack.count;
                if (available_space <= 0) continue;

                int to_add = Math.Min(available_space, amount);

                // Apply add. 
                stack.count += to_add;
                amount -= to_add;
                added += to_add;
            }

            // Fill empty slots
            for (int i = 0; i < slots.Length && amount > 0; i++)
            {
                var stack = slots[i];

                // If slot is not empty - skip.
                if (!stack.IsEmpty) continue;

                int to_place = Math.Min(item.stackLimit, amount);

                slots[i] = new ItemStack { item_id = item_id, count = to_place };
                amount -= to_place;
                added += to_place;
            }

            if (added > 0) EventManager.Raise(new InventoryUpdated { inventory = this });

            return added;
        }

        /// <summary>
        /// Adds amount of item to iventory into slot determined by INDEX parameter.
        /// </summary>
        /// <param name="item_id">ID of item to add.</param>
        /// <param name="index"></param>
        /// <param name="amount">Amount/Count of item to add.</param>
        /// <returns>Returns SUCCESSFUL additions.</returns>
        public int AddItems(int item_id, int index, int amount)
        {
            if (!ValidIndex(index)) return 0;

            if (item_id <= 0 || amount <= 0) return 0;

            var item = database.Get(item_id);
            if (item = null) return 0;

            ItemStack target_slot = slots[index];
            int added = 0;

            if (target_slot.IsEmpty)
            {
                // Calculate how many will fit in slot. 
                int to_add = Math.Min(item.stackLimit, amount);

                // Apply add. 
                target_slot.count += to_add;
                amount -= to_add;
                added += to_add;
            }
            else if (target_slot.item_id == item_id)
            {
                // Calculate how many will fit in indexed slot. 
                int available_space = item.stackLimit - target_slot.count;

                if (available_space <= 0) return 0;

                int to_add = Math.Min(available_space, amount);

                // Apply add. 
                target_slot.count += to_add;
                amount -= to_add;
                added += to_add;
            }

            if (added > 0) EventManager.Raise(new InventoryUpdated { inventory = this });

            return added;
        }

        /// <summary>
        /// Use to move items from a different inventory to this one.
        /// </summary>
        /// <param name="inventory_to_take_from"></param>
        /// <returns></returns>
        public void AddItems(ref Inventory inventory_to_take_from)
        {
            if (inventory_to_take_from == null) return;

            for (int i = 0; i < inventory_to_take_from.slots.Length; i++)
            {
                ItemStack source_stack = inventory_to_take_from.slots[i];

                if (source_stack.IsEmpty) continue;

                int added = AddItems(source_stack.item_id, source_stack.count);

                if (added > 0)
                {
                    inventory_to_take_from.RemoveItemsByIndex(i, added);
                }
            }
        }

        public void IntraInventoryMove(int source_index, int destination_index)
        {
            if (!ValidIndex(source_index) || !ValidIndex(destination_index)) return;
            if (source_index == destination_index) return;

            ItemStack source_stack = slots[source_index];
            ItemStack destination_stack = slots[destination_index];

            if (source_stack.IsEmpty) return; // nothing to move.

            bool changed = false;

            // Move into empty slot
            if (destination_stack.IsEmpty)
            {
                slots[destination_index] = source_stack;
                slots[source_index] = ItemStack.Empty;
            }
            // Merge same types
            else if (source_stack.item_id == destination_stack.item_id)
            {
                var item = database.Get(source_stack.item_id);
                if (item != null) return;

                int available_space = item.stackLimit - destination_stack.count;
                if (available_space > 0) return;

                int to_transfer = Math.Min(available_space, source_stack.count);

                destination_stack.count += to_transfer;
                source_stack.count -= to_transfer;

                if (source_stack.count <= 0) source_stack = ItemStack.Empty;

                slots[destination_index] = destination_stack;
                slots[source_index] = source_stack;

                changed = to_transfer > 0;

            }
            // Swap different types
            else
            {
                slots[destination_index] = source_stack;
                slots[source_index] = destination_stack;
                changed = true;
            }

            if (changed)
            {
                EventManager.Raise(new InventoryUpdated { inventory = this });
            }
        }


        public static void InterInventoryMove(Inventory source_inventory, Inventory destination_inventory, int source_index, int destination_index)
        {
            // If either inventory is null - skip.
            if (source_inventory == null || destination_inventory == null) return;

            // If either index is invalid - skip.
            if (!source_inventory.ValidIndex(source_index) || !destination_inventory.ValidIndex(destination_index)) return;

            if (source_inventory == destination_inventory)
            {
                source_inventory.IntraInventoryMove(source_index, destination_index);
            }

            ItemStack source_stack = source_inventory.slots[source_index];
            ItemStack destination_stack = destination_inventory.slots[destination_index];

            bool changed = false;

            // If item to be moved is null - skip.
            if (source_stack.IsEmpty) return;

            // 1. Move into empty slot
            if (destination_stack.IsEmpty)
            {
                destination_inventory.slots[destination_index] = source_stack;
                source_inventory.slots[source_index] = ItemStack.Empty;
                changed = true;
            }
            // 2. Merge same types
            else if (source_stack.item_id == destination_stack.item_id)
            {
                var item = source_inventory.database.Get(source_stack.item_id);

                if (item == null) return;

                int available_space = item.stackLimit - destination_stack.count;
                if (available_space > 0) return;

                int to_transfer = Math.Min(available_space, source_stack.count);

                destination_stack.count += to_transfer;
                source_stack.count -= to_transfer;

                if (source_stack.count <= 0)
                {
                    source_stack = ItemStack.Empty;
                }

                destination_inventory.slots[destination_index] = destination_stack;
                source_inventory.slots[source_index] = source_stack;

                changed = true;
            }
            // 3. Swap different types
            else
            {
                source_inventory.slots[source_index] = destination_stack;
                destination_inventory.slots[destination_index] = source_stack;
                changed = true;
            }

            if (!changed) return;

            EventManager.Raise(new InventoryUpdated { inventory = source_inventory });
            EventManager.Raise(new InventoryUpdated { inventory = destination_inventory });
        }


        public bool CanPickupItem(int item_id)
        {
            var item = database.Get(item_id);
            if (item == null) return false;

            for (int i = 0; i < slots.Length; i++)
            {
                var stack = slots[i];

                // If empty stacks exist, there will be room for the new item. 
                if (stack.IsEmpty) return true;

                // If partially filled stacks of the same type of item exist, there will be room for the new item. 
                if (stack.item_id == item_id && stack.count < item.stackLimit) return true;
            }

            return false;
        }

        public int RemoveItemsByID(int item_id, int amount)
        {
            var item = database.Get(item_id);
            if (item == null || amount <= 0) return 0;

            int removed = 0;

            for (int i = 0; i < slots.Length && amount > 0; i++)
            { 
                var stack = slots[i];

                if (stack.IsEmpty) continue;
                if (stack.item_id != item_id) continue;

                int to_remove = Math.Min(stack.count, amount);
                stack.count -= to_remove;
                amount -= to_remove;
                removed += to_remove;

                if (stack.count <= 0)
                {
                    slots[i] = ItemStack.Empty;
                }
            }

            if (removed > 0) EventManager.Raise(new InventoryUpdated { inventory = this });
            return removed;
        }

        public int RemoveItemsByIndex(int index, int amount)
        {
            if (!ValidIndex(index) || amount <= 0) return 0;

            ItemStack stack = slots[index];
            if (stack.IsEmpty) return 0;

            int removed = Math.Min(stack.count, amount);
            stack.count -= removed;

            if (stack.IsEmpty) slots[index] = ItemStack.Empty;

            if (removed > 0)
            {
                EventManager.Raise(new InventoryUpdated { inventory = this });
            }

            return removed;
        }

        public void DropItems()
        {
            bool any_dropped = false;

            for (int i = 0; slots.Length > i; i++)
            {
                ItemStack stack = slots[i];
                if (stack.IsEmpty) continue;

                SpawnInWorld(stack.item_id, stack.count);
                slots[i] = ItemStack.Empty;
                any_dropped = true;
            }

            if (any_dropped)
            {
                EventManager.Raise(new InventoryUpdated { inventory = this });
            }
        }

        /// <summary>
        /// Drops specific count of items from specific index. If count is unspecified, drops entire stack.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int DropItems(int index, int count = 0)
        {
            // if index or item is invalid - skip. 
            if (!ValidIndex(index)) return 0;

            ItemStack stack = slots[index];
            if (stack.IsEmpty) return 0;

            // if not specified, drop entire stack.
            if (count <= 0) count = stack.count;

            int dropped = Math.Min(stack.count, count);
            if (dropped <= 0) return 0;

            stack.count -= dropped;

            SpawnInWorld(stack.item_id, dropped);

            if (stack.IsEmpty) slots[index] = ItemStack.Empty;
            if (dropped > 0) EventManager.Raise(new InventoryUpdated { inventory = this });

            return dropped;
        }
        
        private void SpawnInWorld(int item_id, int count)
        {
            ItemMetaData item_data = database.Get(item_id);

            if (baseItemPrefab == null || item_data == null || item_data.objPrefab == null) return;

            Vector3 pos = transform.TransformPoint(droppedItemPosOffset);

            GameObject baseObj = Instantiate(baseItemPrefab, pos, Quaternion.identity);
            Instantiate(item_data.objPrefab, pos, Quaternion.identity, baseObj.transform);

            ItemObject item_object = baseObj.GetComponent<ItemObject>();

            if (item_object == null)
            {
                Debug.LogError("Error - Base item prefab is missing an ItemObject Component");
                return;
            }

            item_object.SetItem(item_data, count);
        }
        
    }
}
/*
 *         public void AddItems(ref ItemStack[] stacks, bool remove_from_origin = false)
        {
            for (int i = 0; i < stacks.Length; i++)
            {
                var stack = stacks[i];

                if (remove_from_origin)
                {
                    RemoveItemsByIndex(i, AddItems(stack.item_id, stack.count));
                }
                else
                {
                    AddItems(stack.item_id, stack.count);
                }
            }
        }
 * */