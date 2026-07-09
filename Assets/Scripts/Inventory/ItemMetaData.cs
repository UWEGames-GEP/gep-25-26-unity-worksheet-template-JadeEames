using UnityEngine;
using System.Collections.Generic;

namespace InventorySystem
{
    [Tooltip("Stores meta data about an inventory item type.")]
    [CreateAssetMenu(fileName = "ItemMetaData", menuName = "Scriptable Objects/ItemMetaData")]
    public class ItemMetaData : ScriptableObject
    {
        public int id = 0;
        public string displayName = "item";
        public string description = "description";
        public List<Category> categories;
        public float weight = 1f;
        public GameObject objPrefab;
        public Sprite icon;

        public int stackLimit = 10;
    }

    public enum Category
    {
        WEAPON,
        TOOL,
        FOOD,
        MISC
    }
}

