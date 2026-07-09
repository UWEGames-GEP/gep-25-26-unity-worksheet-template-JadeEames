using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

            if (primary_inventory != null)
            {
                Build(ref primary_inventory, primary_inventory_panel, ref primary_slots);
                primary_inventory_panel.gameObject.SetActive(true);
            }
            else
            {
                Clear(primary_inventory_panel, ref primary_slots);
                primary_inventory_panel.gameObject.SetActive(false);

            }

            if (secondary_inventory != null)
            {
                Build(ref secondary_inventory, secondary_inventory_panel, ref secondary_slots);
                secondary_inventory_panel.gameObject.SetActive(true);
            }
            else
            {
                Clear(secondary_inventory_panel, ref secondary_slots);
                secondary_inventory_panel.gameObject.SetActive(false);
            }

            Refresh();
            inventory_ui.SetActive(true);
        }

        public void Hide()
        {
            primary_inventory = null;
            secondary_inventory = null;

            Clear();

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
                return;
            }

            if (primary == null) return;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;

            Show(primary, secondary);

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

        private void Clear(Transform inventoryPanel, ref List<GUI_Slot> slots)
        {
            if (inventoryPanel == null) return;

            for (int i = inventoryPanel.childCount - 1; i >= 0; i--)
            {
                Destroy(inventoryPanel.GetChild(i).gameObject);
            }

            slots.Clear();
        }

        private void Build(ref Inventory inventory, Transform inventoryPanel, ref List<GUI_Slot> slots)
        {
            Clear(inventoryPanel, ref slots);

            for (int i = 0; i < inventory.size; i++)
            {
                GameObject slot_object = Instantiate(inventory_slot_prefab, inventoryPanel);
                GUI_Slot gui_slot = slot_object.GetComponent<GUI_Slot>();

                if (gui_slot == null)
                {
                    Debug.LogError("Inventory slot prefab is missing a GUI_Slot component.");
                    continue;
                }

                gui_slot.Bind(this, inventory, i);
                slots.Add(gui_slot);
            }
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

        private void Refresh()
        {
            if (primary_inventory != null) { Refresh(ref primary_inventory, ref primary_slots); }
            if (secondary_inventory != null) { Refresh(ref secondary_inventory, ref secondary_slots); }
        }

        private void Refresh(ref Inventory inventory, ref List<GUI_Slot> slots)
        {
            if (inventory == null) return;

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


        public void HandleDrop(GUI_Slot target_slot, DraggableItem dragged, PointerEventData event_data)
        {
            if (dragged == null || dragged.sourceSlot == null) return;

            GUI_Slot source_slot = dragged.sourceSlot;
            if (source_slot == target_slot) return;

            Inventory source_inventory = source_slot.inventory;
            ItemStack source_stack = source_inventory.slots[source_slot.index];
            if (source_stack.IsEmpty) return;

            // Handle drop if it is outside of inventory UI space. 
            if (DroppedOutSideInventory(event_data.position))
            {
                source_inventory.DropItems(source_slot.index, source_stack.count);
                Refresh();

                return;
            }

            // Handle intra-inventory drop, but not on slot. 
            if (target_slot == null)
            {
                Refresh();
                return;
            }

            Inventory destination_inventory = target_slot.inventory;
            if (destination_inventory == null) return;

            if (source_inventory == destination_inventory)
            {
                // Intra-inventory move.
                source_inventory.IntraInventoryMove(source_slot.index, target_slot.index);
            }
            else
            {
                // Inter-inventory move.
                Inventory.InterInventoryMove(source_inventory, destination_inventory, source_slot.index, target_slot.index);
            }

            Refresh();
        }

        private bool DroppedOutSideInventory(Vector2 dropScreenLocation)
        {
            bool dropOverPrimary = false;
            bool dropOverSecondary = false;

            if (primary_inventory != null && primary_inventory_panel != null)
            {
                dropOverPrimary = RectTransformUtility.RectangleContainsScreenPoint(primary_inventory_panel.GetComponent<RectTransform>(), dropScreenLocation);
            }
            if (secondary_inventory != null && secondary_inventory_panel != null)
            {
                dropOverSecondary = RectTransformUtility.RectangleContainsScreenPoint(secondary_inventory_panel.GetComponent<RectTransform>(), dropScreenLocation);
            }

            return !(dropOverPrimary || dropOverSecondary);
        }
    }
}