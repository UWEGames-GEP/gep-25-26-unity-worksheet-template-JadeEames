using UnityEngine;
using static UnityEditor.Progress;

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

        
        /*public bool IsFull => count >= type.stackLimit;

        
        public ItemMetaData type { get; private set; } = null;

        /// <summary>
        /// Instantiate ItemStack of given InventoryItemType.
        /// </summary>
        /// <param name="type">Inventory Item scriptable object detailing type of stack.</param>
        /// <param name="count">Number of items in stack.</param>
        public ItemStack(ItemMetaData type, int count = 1)
        {
            this.type = type;
            this.count = count;
        }

        /// <summary>
        /// Add x items to the item stack.
        /// </summary>
        /// <param name="amount">The number of items to ATTEMPT add to stack.</param>
        /// <returns>Returns the number of REJECTED items.</returns>
        public int Add(int amount)
        {
            if (amount <= 0) return amount;

            int availableSpace = type.stackLimit - count;
            int toAdd = Mathf.Min(availableSpace, amount);

            count += toAdd;
            return amount - toAdd;
        }

        /// <summary>
        /// Remove x items from the item stack.
        /// </summary>
        /// <param name="amount">The number to ATTEMPT to remove from stack</param>
        /// <returns>Returns the number of REJECTED removals.</returns>
        public int Remove(int amount)
        {
            if (amount <= 0) return amount;

            int toRemove = Mathf.Min(count, amount);
            count -= toRemove;

            return amount - toRemove;
        }
        
    }
}
        */
