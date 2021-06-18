using UnityEngine;

namespace Utils
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T instance { get; private set; }

        protected virtual void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning(name + " - Singleton instance already exists. This is not permitted ");
                Destroy(gameObject);
                return;
            }

            instance = GetComponent<T>();
        }

        /// <summary>
        /// Delete the instance if it is removed from the scene
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (instance != null && instance == this)
                instance = null;
        }
    }
}