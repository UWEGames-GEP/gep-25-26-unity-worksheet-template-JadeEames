  
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ToggleInventoryUnityEvent : UnityEvent<ToggleInventory> { }

public class ToggleInventoryEventListener : EventListener<ToggleInventory, ToggleInventoryUnityEvent> { }
