using InventorySystem;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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

        if (stack.IsEmpty)
        {
            icon.enabled = false;
            countText.text = string.Empty;
            return;
        }

        var item = database.Get(stack.item_id);

        icon.enabled = true;
        icon.sprite = item.icon;
        countText.text = stack.count > 1 ? stack.count.ToString() : string.Empty;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dropped = eventData.pointerDrag?.GetComponent<DraggableItem>();

        if (dropped != null) gui.OnSlotDropped(this, dropped);
    }
}
