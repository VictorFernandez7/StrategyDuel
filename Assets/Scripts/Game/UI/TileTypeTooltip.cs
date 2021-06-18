using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

using Game.Managers;

namespace Game.UI
{
    public class TileTypeTooltip : MonoBehaviour
    {
        [Title("Current Type")]
        [SerializeField] [EnumToggleButtons] [HideLabel] [DisableInPlayMode] private TileManager.TileType _currentType;

        [TitleGroup("References")]
        [FoldoutGroup("References/Main")] [SerializeField] private Image _typeIcon;
        [FoldoutGroup("References/Main")] [SerializeField] private TextMeshProUGUI _typeName;
        [FoldoutGroup("References/Properties")] [SerializeField] private GameObject _plainProperties;
        [FoldoutGroup("References/Properties")] [SerializeField] private GameObject _forestProperties;
        [FoldoutGroup("References/Properties")] [SerializeField] private GameObject _hillProperties;
        [FoldoutGroup("References/Properties")] [SerializeField] private GameObject _mountainProperties;
        [FoldoutGroup("References/Properties")] [SerializeField] private GameObject _swampProperties;
        [FoldoutGroup("References/Properties")] [SerializeField] private GameObject _islandProperties;
        [FoldoutGroup("References/Properties")] [SerializeField] private GameObject _waterProperties;
        [FoldoutGroup("References/Icons")] [SerializeField] private Sprite _plainIcon;
        [FoldoutGroup("References/Icons")] [SerializeField] private Sprite _forestIcon;
        [FoldoutGroup("References/Icons")] [SerializeField] private Sprite _hillIcon;
        [FoldoutGroup("References/Icons")] [SerializeField] private Sprite _mountainIcon;
        [FoldoutGroup("References/Icons")] [SerializeField] private Sprite _swampIcon;
        [FoldoutGroup("References/Icons")] [SerializeField] private Sprite _islandIcon;
        [FoldoutGroup("References/Icons")] [SerializeField] private Sprite _waterIcon;
        [FoldoutGroup("References/Costs")] [SerializeField] private TextMeshProUGUI _plainCost;
        [FoldoutGroup("References/Costs")] [SerializeField] private TextMeshProUGUI _forestCost;
        [FoldoutGroup("References/Costs")] [SerializeField] private TextMeshProUGUI _hillCost;
        [FoldoutGroup("References/Costs")] [SerializeField] private TextMeshProUGUI _mountainCost;
        [FoldoutGroup("References/Costs")] [SerializeField] private TextMeshProUGUI _swampCost;
        [FoldoutGroup("References/Costs")] [SerializeField] private TextMeshProUGUI _islandCost;

        private Animator _anim;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            _anim.SetTrigger("Show");
        }

        private void OnDisable()
        {
            _anim.SetTrigger("Hide");
        }

        public void UpdateTooltip(TileManager.TileType desiredType)
        {
            _currentType = desiredType;
            _typeName.text = desiredType.ToString();

            switch (desiredType)
            {
                case TileManager.TileType.Plain:
                    _plainProperties.SetActive(true);
                    _forestProperties.SetActive(false);
                    _hillProperties.SetActive(false);
                    _mountainProperties.SetActive(false);
                    _swampProperties.SetActive(false);
                    _islandProperties.SetActive(false);
                    _waterProperties.SetActive(false);
                    _typeIcon.sprite = _plainIcon;
                    break;
                case TileManager.TileType.Forest:
                    _plainProperties.SetActive(false);
                    _forestProperties.SetActive(true);
                    _hillProperties.SetActive(false);
                    _mountainProperties.SetActive(false);
                    _swampProperties.SetActive(false);
                    _islandProperties.SetActive(false);
                    _waterProperties.SetActive(false);
                    _typeIcon.sprite = _forestIcon;
                    break;
                case TileManager.TileType.Hill:
                    _plainProperties.SetActive(false);
                    _forestProperties.SetActive(false);
                    _hillProperties.SetActive(true);
                    _mountainProperties.SetActive(false);
                    _swampProperties.SetActive(false);
                    _islandProperties.SetActive(false);
                    _waterProperties.SetActive(false);
                    _typeIcon.sprite = _hillIcon;
                    break;
                case TileManager.TileType.Mountain:
                    _plainProperties.SetActive(false);
                    _forestProperties.SetActive(false);
                    _hillProperties.SetActive(false);
                    _mountainProperties.SetActive(true);
                    _swampProperties.SetActive(false);
                    _islandProperties.SetActive(false);
                    _waterProperties.SetActive(false);
                    _typeIcon.sprite = _mountainIcon;
                    break;
                case TileManager.TileType.Swamp:
                    _plainProperties.SetActive(false);
                    _forestProperties.SetActive(false);
                    _hillProperties.SetActive(false);
                    _mountainProperties.SetActive(false);
                    _swampProperties.SetActive(true);
                    _islandProperties.SetActive(false);
                    _waterProperties.SetActive(false);
                    _typeIcon.sprite = _swampIcon;
                    break;
                case TileManager.TileType.Island:
                    _plainProperties.SetActive(false);
                    _forestProperties.SetActive(false);
                    _hillProperties.SetActive(false);
                    _mountainProperties.SetActive(false);
                    _swampProperties.SetActive(false);
                    _islandProperties.SetActive(true);
                    _waterProperties.SetActive(false);
                    _typeIcon.sprite = _islandIcon;
                    break;
                case TileManager.TileType.Water:
                    _plainProperties.SetActive(false);
                    _forestProperties.SetActive(false);
                    _hillProperties.SetActive(false);
                    _mountainProperties.SetActive(false);
                    _swampProperties.SetActive(false);
                    _islandProperties.SetActive(false);
                    _waterProperties.SetActive(true);
                    _typeIcon.sprite = _waterIcon;
                    break;
            }
        }

        public void UpdateCosts(int plain, int forest, int hill, int mountain, int swamp, int island)
        {
            _plainCost.text = plain.ToString();
            _forestCost.text = forest.ToString();
            _hillCost.text = hill.ToString();
            _mountainCost.text = mountain.ToString();
            _swampCost.text = swamp.ToString();
            _islandCost.text = island.ToString();
        }
    }
}