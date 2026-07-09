using UnityEngine;
using UnityEngine.EventSystems;

namespace InventorySystem
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public GUI_Slot sourceSlot;
        
        private Transform originalParent;
        private RectTransform rectTransform;
        private CanvasGroup draggableCanvasGroup;

        public Inventory inventory { get; private set; }
        private bool droppedOnSlot = false;

        public void Awake()
        {
            sourceSlot = gameObject.GetComponentInParent<GUI_Slot>();
            rectTransform = GetComponent<RectTransform>();
            draggableCanvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            sourceSlot = GetComponentInParent<GUI_Slot>();

            // Make transparent.
            draggableCanvasGroup.alpha = 0.6f;

            // Set originalParent & set parent to root parent - so renders above all UI.
            originalParent = transform.parent;
            transform.SetParent(transform.root);

            // Allow raycasts to pass through the draggable.
            draggableCanvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            draggableCanvasGroup.alpha = 1f;
            draggableCanvasGroup.blocksRaycasts = true;

            /*
            if (!droppedOnSlot)
            {
                inventory.DropItems(sourceSlot.index);
            }
            */

            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;
            droppedOnSlot = false;
        }

        
        public void DroppedOnSlot(GUI_Slot targetSlot)
        {
            droppedOnSlot = true;
            Inventory.InterInventoryMove(inventory, targetSlot.inventory, sourceSlot.index, targetSlot.index);
        }/*
        */
    }
}
