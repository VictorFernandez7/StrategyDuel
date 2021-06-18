using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Data.Pools
{
    public class RuntimeRenderTexture : MonoBehaviour
    {
        [TitleGroup("Render Texture Container")]
        [SerializeField] [HideLabel] [Sirenix.OdinInspector.ReadOnly] private RenderTexture _renderTexture;

        [Title("References")]
        [SerializeField] private RenderTexture _renderTextureSample;

        public RenderTexture renderTexture => _renderTexture;

        private void Awake()
        {
            _renderTexture = CreateRenderTexture();
        }

        private RenderTexture CreateRenderTexture()
        {
            RenderTexture newRenderTexture = new RenderTexture(_renderTextureSample);
            newRenderTexture.Create();
            return newRenderTexture;
        }
    }
}