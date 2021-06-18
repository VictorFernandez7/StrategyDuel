using UnityEngine;

using Game.Data.Pools;

namespace Utils.Pools.Controllers
{
    public class CardCharacterPoolController : MonoBehaviour
    {
        [SerializeReference] private CardCharacterPool _cardCharacterPool;

        void Start()
        {
            _cardCharacterPool?.pool.Init(_cardCharacterPool.transform);
        }

        public CardCharacter RetrieveCardCharacterInstance()
        {
            return _cardCharacterPool?.pool.RetrieveItem();
        }

        public void DispatchCardCharacterInstance(CardCharacter instance)
        {
            _cardCharacterPool?.pool.DestroyItem(instance as CardCharacter);
        }
    }
}