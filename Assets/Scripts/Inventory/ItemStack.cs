using Unity.Mathematics;
using UnityEngine;

namespace InventorySystem
{
    public class ItemStack
    {
        public InventoryItem type { get; private set; } = null;
        public int count { get; private set; } = 0;

        /// <summary>
        /// Instantiate ItemStack of given InventoryItemType.
        /// Item count is initialised to 1.
        /// </summary>
        /// <param name="itemType"></param>
        public ItemStack(InventoryItem itemType)
        {
            type = itemType;
            count = 1;
        }

        /// <summary>
        /// Add x items to the item stack.
        /// </summary>
        /// <param name="numToAdd"></param>
        /// <returns>Rejected items.</returns>
        public int AddItems(int numToAdd)
        {
            if (type == null)
            {
                Debug.LogWarning("Cannot add to an ItemStack of item type null");
                return numToAdd;
            }

            int total = count + numToAdd;

            if (total > type.stackLimit)
            {
                int remaining = total - type.stackLimit;
                count = type.stackLimit;
                return remaining;
            }

            count = total;

            return 0;
        }

        /// <summary>
        /// Remove x items from the item stack.
        /// </summary>
        /// <param name="numToRemove"></param>
        /// <returns>Rejected removals.</returns>
        public int RemoveItems(int numToRemove)
        {
            if (type == null)
            {
                Debug.LogWarning("Cannot remove from an ItemStack of item type null");
                return numToRemove;
            }

            if (numToRemove > count)
            {
                //Debug.LogWarning("Cannot remove " + numToRemove + " from " + "an ItemStack of count " + count);

                int remainingRemovals = numToRemove - count;

                count = 0;
                return remainingRemovals;
            }

            count -= numToRemove;
            return 0;
        }

        public void wipe(bool wipeType = true, bool wipeCount = true)
        {
            if (wipeType) type = null;
            if (wipeCount) count = 0;
        }

        public void SetType(InventoryItem type)
        {
            this.type = type;
        }
    }
}
