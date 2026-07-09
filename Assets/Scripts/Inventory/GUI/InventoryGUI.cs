using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Build.Content;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem
{
    public class InventoryGUI : MonoBehaviour
    {
        [Header("GUI References")]
        [SerializeField] private Transform primary_inventory_panel;
        [SerializeField] private Transform secondary_inventory_panel;
        [SerializeField] private GameObject inventory_ui;
        [SerializeField] private GameObject inventory_slot_prefab;

        private Inventory primary_inventory;
        private Inventory secondary_inventory;

        private List<GUI_Slot> primary_slots = new();
        private List<GUI_Slot> secondary_slots = new();

        public void Show(Inventory primary, Inventory secondary = null)
        {
            primary_inventory = primary;
            secondary_inventory = secondary;


            Build(ref primary_inventory, primary_inventory_panel, ref primary_slots);
            primary_inventory_panel.gameObject.SetActive(true);

            if (secondary_inventory != null)
            {
                Build(ref secondary_inventory, secondary_inventory_panel, ref secondary_slots);
                secondary_inventory_panel.gameObject.SetActive(true);
            }

            Refresh();
            inventory_ui.SetActive(true);
        }

        public void Hide()
        {
            primary_inventory = null;
            secondary_inventory = null;

            primary_inventory_panel.gameObject.SetActive(false);
            secondary_inventory_panel.gameObject.SetActive(false);
            inventory_ui.SetActive(false);
        }

        public void Toggle(Inventory primary = null, Inventory secondary = null)
        {
            bool state = !inventory_ui.activeSelf;

            if (!state)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                Hide();
            }
            else if (primary != null)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;

                Show(primary, secondary);
            }
        }

        public void Toggle(ToggleInventory eventData)
        {
            Toggle(eventData.primaryInventory, eventData.secondaryInventory);
        }

        private void Clear()
        {
            Clear(primary_inventory_panel.transform, ref primary_slots);
            Clear(secondary_inventory_panel.transform, ref secondary_slots);
        }

        private void Clear(Transform inventoryPanelTransform, ref List<GUI_Slot> slots)
        {
            foreach (Transform slot in inventoryPanelTransform.transform) { Destroy(slot.gameObject); }

            slots.Clear();
        }

        private void Build()
        {
            if (primary_inventory != null)
            {
                Build(ref primary_inventory, primary_inventory_panel.transform, ref primary_slots);
            }
            if (secondary_inventory != null)
            {
                Build(ref secondary_inventory, secondary_inventory_panel.transform, ref secondary_slots);
            }
        }

        private void Build(ref Inventory inventory, Transform inventoryPanel, ref List<GUI_Slot> slots)
        {
            Clear(inventoryPanel, ref slots);

            for (int i = 0; i < inventory.size; i++)
            {
                var slot_object = Instantiate(inventory_slot_prefab, inventoryPanel.transform);
                var gui_slot = slot_object.GetComponent<GUI_Slot>();
                gui_slot.Bind(this, inventory, i);

                slots.Add(gui_slot);
            }
        }

        private void Refresh()
        {
            if (primary_inventory != null) { Refresh(ref primary_inventory, ref primary_slots); }
            if (secondary_inventory != null) { Refresh(ref secondary_inventory, ref secondary_slots); }
        }

        private void Refresh(ref Inventory inventory, ref List<GUI_Slot> slots)
        {
            for (int i = 0; i < inventory.size; ++i)
            {
                slots[i].Refresh(inventory.database);
            }
        }

        public void Refresh(InventoryUpdated eventData)
        {
            if (eventData.inventory == primary_inventory) Refresh(ref primary_inventory, ref primary_slots);
            else if (eventData.inventory == secondary_inventory) Refresh(ref secondary_inventory, ref secondary_slots);
        }

        public void OnSlotDropped(GUI_Slot target_slot, DraggableItem dragged)
        {
            GUI_Slot source_slot = dragged.sourceSlot;

            if (source_slot.inventory == target_slot.inventory)
            {
                source_slot.inventory.IntraInventoryMove(source_slot.index, target_slot.index);
            }
            else
            {
                Inventory.InterInventoryMove(source_slot.inventory, target_slot.inventory, source_slot.index, target_slot.index);
            }

            Refresh();
        }

        public void HandleDrop(GUI_Slot target_slot, DraggableItem dragged, PointerEventData event_data)
        {
            GUI_Slot source_slot = dragged.sourceSlot;
            if (source_slot == target_slot) return;

            Inventory source_inventory = source_slot.inventory;
            Inventory destination_inventory = target_slot != null ? target_slot.inventory : null;

            ItemStack source_stack = source_inventory.slots[source_slot.index];

            // Handle drop if it is outside of inventory UI space. 
            if (DroppedOutSideInventory(event_data.position))
            {
                if (!source_stack.IsEmpty)
                {
                    source_inventory.DropItems(source_slot.index, source_stack.count);
                }

                return;
            }
            // Handle intra-inventory drop. 
            if (source_inventory == destination_inventory)
            {
                source_inventory.IntraInventoryMove(source_slot.index, target_slot.index);
            }
            else if (destination_inventory != null) // Handle drop on slot within a different inventory.
            {
                Inventory.InterInventoryMove(source_inventory, destination_inventory, source_slot.index, target_slot.index);

            }
        }

        private bool DroppedOutSideInventory(Vector2 dropScreenLocation)
        {
            bool dropOverPrimary = false;
            bool dropOverSecondary = false;

            if (primary_inventory != null)
            {
                dropOverPrimary = RectTransformUtility.RectangleContainsScreenPoint(primary_inventory_panel.GetComponent<RectTransform>(), dropScreenLocation);
            }
            if (secondary_inventory != null)
            {
                dropOverSecondary = RectTransformUtility.RectangleContainsScreenPoint(secondary_inventory.GetComponent<RectTransform>(), dropScreenLocation);
            }

            return !(dropOverPrimary || dropOverSecondary);
        }
    }
}


/*
 * TODO 
 * dragabble GUI stacks.
 * Sort inventory based on categories & item ID.
 * Have filter options.
 * Have tool bar with equipable items. 
 */