using UnityEngine;

namespace Game.Data.Pools
{
    [System.Serializable]
    public class CardCharacterPoolImplementation : Utils.Pools.Pool<Game.Data.Pools.CardCharacter> { }

    public class CardCharacterPool : MonoBehaviour
    {
        [SerializeField] public CardCharacterPoolImplementation pool;
    }
}