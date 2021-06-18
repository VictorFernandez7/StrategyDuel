using UnityEngine;

using Game.Data.Pools;

namespace Utils.Pools.Controllers
{
    public class RenderTexturePoolController : MonoBehaviour
    {
        [SerializeReference] private RenderTexturePool _RenderTexturePool;

        void Start()
        {
            _RenderTexturePool?.pool.Init(_RenderTexturePool.transform);
        }

        public RuntimeRenderTexture RetrieveRenderTextureInstance()
        {
            return _RenderTexturePool?.pool.RetrieveItem();
        }

        public void DispatchRenderTextureInstance(RuntimeRenderTexture instance)
        {
            _RenderTexturePool?.pool.DestroyItem(instance as RuntimeRenderTexture);
        }
    }
}