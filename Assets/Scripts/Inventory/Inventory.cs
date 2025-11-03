using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Progress;


namespace InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private List<ItemStack> slots = new();
        [SerializeField] private GameObject base_item_object = null;
        [SerializeField] private int maxInventorySize = 4;
        [SerializeField] private Vector3 dropped_item_pos_offset = new Vector3(0,1,1);


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                DropItems();
            }
        }

        public bool CanPickupItem(InventoryItem itemType)
        {
            List<ItemStack> slotsOfItemType = slots.FindAll(x => x.type.name == itemType.name);

            // And there is room for another stack.
            if (slots.Count < maxInventorySize)
            {
                return true;
            }

            // If there are no stacks of the appropriate type.
            if (slotsOfItemType != null)
            {
                // Loop through each & check if there is space to spare in the slot. 
                foreach (ItemStack slot in slotsOfItemType)
                {
                    if (slot.count < slot.type.stackLimit)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Use to add multiple items of a single type to the inventory.
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="numToAdd"></param>
        /// <returns>Returns count of SUCCESSFUL additions to inventory.</returns>
        public int AddItems(InventoryItem itemType, int numToAdd = 1)
        {
            List<ItemStack> slotsOfItemType = slots.FindAll(x => x.type.name == itemType.name);
            int remaining = numToAdd;

            // If there are no stacks of the appropriate type.
            if (slotsOfItemType == null)
            {
                // And there is room for another stack.
                if (slots.Count < maxInventorySize)
                {
                    // Add new stack of the appropriate type (with a count of 1) to inventory & exit. 
                    slots.Add(new ItemStack(itemType));
                    remaining -= 1;

                    // If there are more items to add recursively call AddItem.
                    if (remaining > 0)
                    {
                        return 1 + AddItems(itemType, remaining);
                    }

                    return 1;
                }
            }
            else // If there are slots of the added item type.
            {
                // Loop through each & check if there is space to spare in the slot. 
                foreach (ItemStack slot in slotsOfItemType)
                {
                    if (slot.count < slot.type.stackLimit)
                    {
                        // Add item to inventory slot & exit. 
                        remaining = slot.AddItems(remaining);

                        if (remaining == 0) return numToAdd;
                    }
                }
                // If room could not be found in inventory slots of the added type.
                // Check if there is room for a new stack.
                if (slots.Count < maxInventorySize)
                {
                    // Add new stack of type.
                    slots.Add(new ItemStack(itemType));
                    remaining -= 1;

                    return remaining > 0? 1 + AddItems(itemType, remaining) : numToAdd - remaining;
                }
            }

            return numToAdd;
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

        public void RemoveItems(InventoryItem itemType, int count)
        {
            List<ItemStack> slotsOfItemType = slots.FindAll(x => x.type.name == itemType.name);
            int remaining = count;

            if (slotsOfItemType != null)
            {
                foreach (var slot in slotsOfItemType)
                {
                    if (remaining > 0)
                    {
                        remaining = slot.RemoveItems(remaining);
                        continue;
                    }
                    break;
                }
            }
        }

        public void DropItems()
        {
            foreach (var stack in slots)
            {
                for (int i = stack.count; i > 0; --i)
                {
                    Vector3 position = transform.TransformPoint(dropped_item_pos_offset);
                    var BaseObject = Instantiate(base_item_object, position, Quaternion.identity);
                    var itemObject = Instantiate(stack.type.obj_prefab, position, Quaternion.identity, BaseObject.transform);

                    BaseObject.GetComponent<ItemObject>().setItemType(stack.type);
                }
            }

            slots.Clear();
        }
    }
}
