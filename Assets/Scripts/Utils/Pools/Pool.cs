using System.Collections.Generic;
using UnityEngine;

// Generic Abstract Pool.
// Manages a pool of components that get instantiated when initialized, stores them deactivated,
// and delivers them when asked if possible, so that no extra instantiation is made.

namespace Utils.Pools
{
    [System.Serializable]
    public abstract class Pool<T> where T : MonoBehaviour
    {
        // List of available and used items
        private List<T> _itemPool;
        private List<T> _usedItems;

        // Transform to set as parent of available items
        private Transform _poolRoot;

        // Maximum number of available item, and a reference to the item to be managed
        public int maxItems;
        public T itemToPool;

        #region Gettters
        public bool isItemAvailable
        {
            get { return availableItemAmount > 0; }
        }

        public int availableItemAmount
        {
            get { return _itemPool.Count; }
        }

        public int usedItemAmount
        {
            get { return _usedItems.Count; }
        }
        #endregion

        /// <summary>
        /// Initializes the pool. Cleans items if needed and creates new ones to be managed
        /// </summary>
        /// <param name="transform"></param>
        public void Init(Transform transform)
        {
            // Clean
            if (_itemPool != null)
            {
                foreach (T item in _itemPool)
                {
                    GameObject.Destroy(item.gameObject);
                }
            }

            _itemPool = new List<T>();
            _usedItems = new List<T>();

            // Parent transform of items
            _poolRoot = transform;

            // Instantiate and hide items
            for (int i = 0; i < maxItems; i++)
            {
                T item = GameObject.Instantiate(itemToPool.gameObject).GetComponent<T>();
                _itemPool.Add(item);
                HideItem(item);
            }
        }

        /// <summary>
        /// Returns an available item from the pool, null if none is available
        /// </summary>
        /// <returns></returns>
        public T RetrieveItem()
        {
            if (isItemAvailable)
            {
                T item = _itemPool[0];
                // Detach from parent
                item.transform.SetParent(null);

                _usedItems.Add(item);
                _itemPool.Remove(item);

                return item;
            }

            else
                return null;
        }

        /// <summary>
        /// Destroy an item on demand
        /// </summary>
        /// <param name="item"></param>
        public void DestroyItem(T item)
        {
            HideItem(item);
            StoreItem(item);
        }

        /// <summary>
        /// Hide an item. Attach to parent and deactivate
        /// </summary>
        /// <param name="item"></param>
        private void HideItem(T item)
        {
            if (item != null)
            {
                item.gameObject.SetActive(false);
                item.transform.SetParent(_poolRoot);
            }
        }

        /// <summary>
        /// Restore an item. Remove from used and add to available
        /// </summary>
        /// <param name="item"></param>
        private void StoreItem(T item)
        {
            _usedItems.Remove(item);
            _itemPool.Add(item);
        }
    }
}