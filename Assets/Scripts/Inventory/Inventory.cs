using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.tvOS;


namespace InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private List<ItemStack> slots = new();
        [SerializeField] private GameObject baseItemPrefab = null;
        [SerializeField] private Vector3 droppedItemPosOffset = new Vector3(0,1,1);

        public int maxInventorySize = 4;
        public List<ItemStack> Slots { get { return slots; } }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                DropItems();
            }
        }

        public bool CanPickupItem(InventoryItem itemType)
        {
            if (itemType == null) return false;

            // Check for partially filled stacks
            foreach (var slot in slots)
            {
                if (slot.type == itemType && slot.count < slot.type.stackLimit)
                    return true;
            }

            // Or check for empty slot
            return slots.Count < maxInventorySize;
        }


        /// <summary>
        /// Use to add multiple items of a single type to the inventory.
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="numToAdd"></param>
        /// <returns>Returns count of SUCCESSFUL additions to inventory.</returns>
        public int AddItems(InventoryItem itemType, int numToAdd = 1)
        {
            // Cannot add null items or 0 items. 
            if (itemType == null || numToAdd <= 0) return 0;

            int added = 0;
            int remaining = numToAdd;

            // Fill any existing stacks.
            foreach (var stack in slots)
            {
                if (stack.type == itemType && stack.count < stack.type.stackLimit)
                {
                    int overflow = stack.AddItems(remaining);
                    int addedHere = remaining - overflow;
                    added += addedHere;
                    remaining = overflow;

                    if (remaining == 0)
                        break;
                }
            }

            // Create new stacks if necessary and possible. 
            while (remaining > 0 && slots.Count < maxInventorySize)
            {
                var stack = new ItemStack(itemType);
                remaining--;   // New stack starts with 1 item
                added++;

                // Add extra items if possible
                int excess = stack.AddItems(remaining);
                added += remaining - excess;
                remaining = excess;

                slots.Add(stack);
            }

            if (added > 0) EventManager.Raise(new InventoryUpdated());

            return added;
        }

        /// <summary>
        /// Use to move items from a different inventory to this one.
        /// </summary>
        /// <param name="inventoryToTakeFrom"></param>
        /// <returns></returns>
        public void AddItems(ref Inventory inventoryToTakeFrom)
        {
            foreach(var slot in inventoryToTakeFrom.slots)
            {
                inventoryToTakeFrom.RemoveItems(slot.type, AddItems(slot.type, slot.count));
            }
        }
       
        public void AddItems(ref List<ItemStack> stacks, bool removeFromRef = false)
        {
            foreach(var stack in stacks)
            {
                if (removeFromRef)
                {
                    stack.RemoveItems(AddItems(stack.type, stack.count));
                }
                else
                {
                    AddItems(stack.type, stack.count);
                }
            }
        }

        public int RemoveItems(InventoryItem itemType, int count)
        {
            if (itemType == null || count <= 0)  return 0;

            int removed = 0;
            int remaining = count;

            for (int i = slots.Count-1; i >= 0; --i)
            {
                var stack = slots[i];

                if (stack.type == itemType)
                {
                    remaining = stack.RemoveItems(remaining);
                    removed += count - remaining;
                }

                if (remaining == 0) break;
            }

            slots.RemoveAll(s => s.count == 0 || s.type == null);
            if (removed != 0) EventManager.Raise(new InventoryUpdated());
            return removed;
        }

        public void DropItems()
        {
            foreach (var stack in slots)
            {
                Vector3 position = transform.TransformPoint(droppedItemPosOffset);
                var BaseObject = Instantiate(baseItemPrefab, position, Quaternion.identity);
                var itemObject = Instantiate(stack.type.obj_prefab, position, Quaternion.identity, BaseObject.transform);

                BaseObject.GetComponent<ItemObject>().setItemType(stack.type);
            }

            slots.Clear();
            EventManager.Raise(new InventoryUpdated());
        }
    }
}
