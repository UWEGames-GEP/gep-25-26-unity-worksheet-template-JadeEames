using UnityEngine;

namespace InventorySystem
{
    [Tooltip("Represents a stack of items - stores item_id and count")]
    [System.Serializable]
    public class ItemStack
    {
        public int item_id;
        public int count;


        public static ItemStack Empty => new ItemStack { item_id = 0, count = 0 };

        public bool IsEmpty => item_id == 0 || count <= 0;
    }
}

