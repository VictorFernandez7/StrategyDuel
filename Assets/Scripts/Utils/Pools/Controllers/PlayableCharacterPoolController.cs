using UnityEngine;

using Game.Characters;
using Game.Data.Pools;

namespace Utils.Pools.Controllers
{
    public class PlayableCharacterPoolController : MonoBehaviour
    {
        [SerializeReference] private PlayableCharacterPool _playableCharacterPool;

        void Start()
        {
            _playableCharacterPool?.pool.Init(_playableCharacterPool.transform);
        }

        public CharacterBasicInfo RetrievePlayableCharacter()
        {
            return _playableCharacterPool?.pool.RetrieveItem();
        }

        public void DispatchPlayableCharacterInstance(CharacterBasicInfo instance)
        {
            _playableCharacterPool?.pool.DestroyItem(instance as CharacterBasicInfo);
        }
    }
}