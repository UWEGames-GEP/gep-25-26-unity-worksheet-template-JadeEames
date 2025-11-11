
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InventoryUpdatedUnityEvent : UnityEvent<InventoryUpdated> { }

public class InventoryUpdatedEventListener : EventListener<InventoryUpdated, InventoryUpdatedUnityEvent> { }
