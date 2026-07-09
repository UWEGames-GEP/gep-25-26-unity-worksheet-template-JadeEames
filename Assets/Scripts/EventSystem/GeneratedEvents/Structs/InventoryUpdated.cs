
using InventorySystem;
using System;
using UnityEngine;

[System.Serializable]
public struct InventoryUpdated
{
    public InventorySystem.Inventory inventory;

    public InventoryUpdated(Inventory inv)
    {
        inventory = inv;
    }
}
