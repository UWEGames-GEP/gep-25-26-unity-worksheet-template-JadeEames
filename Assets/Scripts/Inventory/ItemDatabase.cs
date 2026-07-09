using InventorySystem;
using System.Collections.Generic;
using UnityEngine;


namespace InventorySystem
{
    [Tooltip("Database storing references to ItemMetaData mapped to associated item ID.")]
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        public List<ItemMetaData> items = new List<ItemMetaData>();

        private Dictionary<int, ItemMetaData> lookup;

        private void OnEnable()
        {
            BuildLookup();
        }

        public void BuildLookup()
        {
            lookup = new Dictionary<int, ItemMetaData>();

            foreach (ItemMetaData item in items)
            {
                if (item == null) continue;
                lookup[item.id] = item;
            }
        }

        public ItemMetaData Get(int id)
        {
            if (lookup.TryGetValue(id, out var item))
            {
                return item;
            }

            return null;
        }
    }
}

