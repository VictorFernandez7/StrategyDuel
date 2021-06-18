using UnityEngine;

namespace Game.Data.Pools
{
    [System.Serializable]
    public class PlayableCharacterPoolImplementation : Utils.Pools.Pool<Game.Characters.CharacterBasicInfo> { }

    public class PlayableCharacterPool : MonoBehaviour
    {
        [SerializeField] public PlayableCharacterPoolImplementation pool;
    }
}