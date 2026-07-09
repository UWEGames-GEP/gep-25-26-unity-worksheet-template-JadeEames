using InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class GUI_Slot : MonoBehaviour, IDropHandler
{
    public InventoryGUI gui { get; private set; }
    public Inventory inventory { get; private set; }
    public int index { get; private set; }

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;

    public void Bind(InventoryGUI gui, Inventory inventory, int index)
    {
        this.gui = gui;
        this.inventory = inventory;
        this.index = index;
    }

    public void Refresh(ItemDatabase database)
    {
        var stack = inventory.slots[index];
        var item = database.Get(stack.item_id);

        if (stack.IsEmpty || item == null)
        {
            icon.enabled = false;
            icon.sprite = null;
            countText.text = string.Empty;
            return;
        }


        icon.enabled = true;
        icon.sprite = item.icon;
        countText.text = stack.count > 1 ? stack.count.ToString() : string.Empty;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag?.GetComponent<DraggableItem>();

        if (dragged == null) return;

        gui.HandleDrop(this, dragged, eventData);
        dragged.MarkDroppedOnSlot();
    }
}
