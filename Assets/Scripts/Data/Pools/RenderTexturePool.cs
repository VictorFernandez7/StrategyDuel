using UnityEngine;

namespace Game.Data.Pools
{
    [System.Serializable]
    public class RenderTexturePoolImplementation : Utils.Pools.Pool<Game.Data.Pools.RuntimeRenderTexture> { }

    public class RenderTexturePool : MonoBehaviour
    {
        [SerializeField] public RenderTexturePoolImplementation pool;
    }
}