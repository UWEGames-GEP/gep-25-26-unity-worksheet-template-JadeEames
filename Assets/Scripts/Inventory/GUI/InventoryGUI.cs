using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    [System.Serializable]
    public class UISlotData
    {
        public GameObject uiObjectRef;
        public Image imageRef;
        public ItemStack itemStackRef;
        public TextMeshProUGUI countTextRef;
    }

    public class InventoryGUI : MonoBehaviour
    {
        [SerializeField] private Inventory inventoryData;
        [SerializeField] private GameObject inventoryUI;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject inventorySlotPrefab;

        private List<UISlotData> uiStacks = new();

        void Start()
        {
            CreateUISlots();
        }
        
        public void CloseGUI()
        {
            inventoryUI.SetActive(false);
            Cursor.visible = false;
        }

        private void CreateUISlots()
        {
            for (int i = 0; i < inventoryData.maxInventorySize; i++)
            {
                var slot = Instantiate(inventorySlotPrefab, inventoryPanel.transform);
                var image = slot.transform.GetChild(0).GetComponent<Image>();
                var text = slot.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                uiStacks.Add(new UISlotData { uiObjectRef = slot, imageRef = image, countTextRef = text });
            }
        }

        private void UpdateUISlots()
        {
            for (int i = 0; i < uiStacks.Count; i++)
            {
                var uiSlot = uiStacks[i];
                if (i < inventoryData.Slots.Count)
                {
                    var stackData = inventoryData.Slots[i];
                    uiSlot.imageRef.sprite = stackData.type.icon;
                    uiSlot.countTextRef.text = stackData.count.ToString();
                    uiSlot.imageRef.enabled = true;
                }
                else
                {
                    uiSlot.imageRef.sprite = null;
                    uiSlot.countTextRef.text = "";
                    uiSlot.imageRef.enabled = false;
                }
            }
        }

        public void UpdateUISlots(InventoryUpdated inventory)
        {
            // again inventory is redudant - will fix this later. 
            UpdateUISlots();
        }

        public void toggleGUI(ToggleInventory eventData)
        {
            // eventData is redudant.
            bool state = !inventoryUI.activeSelf;
            inventoryUI.SetActive(state);

            Cursor.lockState = state ? CursorLockMode.Confined : CursorLockMode.Locked;
            Cursor.visible = state;
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