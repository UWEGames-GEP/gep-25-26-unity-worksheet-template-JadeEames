using UnityEngine;
using System.Collections.Generic;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "InventoryItem", menuName = "Scriptable Objects/InventoryItem")]
    public class InventoryItem : ScriptableObject
    {
        [SerializeField] private uint id = 0;
        [SerializeField] private string displayName = "item";
        [SerializeField] private string description = "description";
        [SerializeField] private List<Category> categories = new();
        [SerializeField] public float weight = 1f;
        public GameObject obj_prefab;
        public Sprite icon;

        public int stackLimit = 10;
        public enum Category
        {
            WEAPON,
            TOOL,
            FOOD,
            MISC
        }
    }
}

