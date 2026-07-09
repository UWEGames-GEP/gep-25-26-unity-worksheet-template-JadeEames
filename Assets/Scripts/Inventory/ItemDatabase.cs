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

                if (item.id <= 0)
                {
                    Debug.LogWarning("Item '{item.name}' has an invalid ID. IDs should be greater than 0.");
                    continue;
                }

                if (lookup.ContainsKey(item.id))
                {
                    Debug.LogWarning("Duplicate item ID found: {item.id}. Overwriting previous entry.");
                }

                lookup[item.id] = item;
            }
        }

        public ItemMetaData Get(int id)
        {
            if (id <= 0) return null;
            if (lookup == null) BuildLookup();

            return lookup.TryGetValue(id, out ItemMetaData item) ? item : null;
        }
    }
}

