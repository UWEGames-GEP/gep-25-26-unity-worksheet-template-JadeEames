using UnityEngine;
using UnityEngine.EventSystems;

namespace InventorySystem
{
    public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public GUI_Slot sourceSlot { get; private set; }
        
        private Transform originalParent;
        private RectTransform rectTransform;
        private CanvasGroup draggableCanvasGroup;

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

            if (sourceSlot == null) return;

            droppedOnSlot = false;

            // Make transparent.
            draggableCanvasGroup.alpha = 0.6f;

            // Set originalParent & set parent to root parent - so renders above all other GUI elements.
            originalParent = transform.parent;
            transform.SetParent(transform.root);

            // Allow raycasts to pass through the draggable.
            draggableCanvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            draggableCanvasGroup.alpha = 1f;
            draggableCanvasGroup.blocksRaycasts = true;

            
            if (!droppedOnSlot && sourceSlot != null)
            {
                sourceSlot.gui.HandleDrop(null, this, eventData);
            }

            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;
            droppedOnSlot = false;
        }

        
        public void MarkDroppedOnSlot()
        {
            droppedOnSlot = true;
        }
    }
}
