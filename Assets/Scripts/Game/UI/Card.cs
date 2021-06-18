using System.Collections.Generic;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

using Game.Data;
using Game.Managers;

namespace Game.UI
{
    public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [Title("Main Data")]
        [SerializeField] private MainData.Players _owner;
        [SerializeField] private MainData.CharacterName _character;

        [TitleGroup("References")]
        [FoldoutGroup("References/Dynamic Elements")] [SerializeField] private RawImage _cardRawImage;
        [FoldoutGroup("References/Dynamic Elements")] [SerializeField] private TextMeshProUGUI _name;
        [FoldoutGroup("References/Dynamic Elements")] [SerializeField] private TextMeshProUGUI _description;
        [FoldoutGroup("References/Dynamic Elements")] [SerializeField] private Image _classIcon;
        [FoldoutGroup("References/Dynamic Elements")] [SerializeField] private TextMeshProUGUI _cost;
        [FoldoutGroup("References/Dynamic Elements")] [SerializeField] private TextMeshProUGUI _heatlhPoints;
        [FoldoutGroup("References/Dynamic Elements")] [SerializeField] private TextMeshProUGUI _armorPoints;
        [FoldoutGroup("References/Dynamic Elements")] [SerializeField] private TextMeshProUGUI _attackPoints;
        [FoldoutGroup("References/Dynamic Elements")] [SerializeField] private TextMeshProUGUI _movementPoints;
        [FoldoutGroup("References/Other")] [SerializeField] private GameObject _content;

        public MainData.CharacterName character
        {
            get => _character;
            set { _character = value; }
        }

        private Vector2 _lastMousePos;
        private Vector2 _startingPos;
        private Character currentCharacterData;

        private void Start()
        {
            _startingPos = transform.position;
        }

        private void OnEnable()
        {
            CardManager.instance.CreateNewCharacter(_character, _owner);
            _cardRawImage.texture = CardManager.instance.GetRenderTexture(_character, _owner);
            SetDynamicElements();
        }

        private void OnDisable()
        {
            if (CardManager.instance != null)
                CardManager.instance.RemoveExistingCharacter(_character, _owner);
        }

        private void OnDestroy()
        {
            if (CardManager.instance != null)
                CardManager.instance.RemoveExistingCharacter(_character, _owner);
        }

        /// <summary>
        /// Sets the value of all the dynamic elements of the card
        /// </summary>
        private void SetDynamicElements()
        {
            for (int i = 0; i < GameManager.instance.army.characters.Count; i++)
            {
                if (GameManager.instance.army.characters[i].name == _character)
                    currentCharacterData = GameManager.instance.army.characters[i];
            }

            _name.text = currentCharacterData.cardName;
            _description.text = currentCharacterData.description;
            _classIcon.sprite = currentCharacterData.classIcon;
            _cost.text = currentCharacterData.cost.ToString();
            _heatlhPoints.text = currentCharacterData.healthPoints.ToString();
            _armorPoints.text = currentCharacterData.armorPoints.ToString();
            _attackPoints.text = currentCharacterData.attackPoints.ToString();
            _movementPoints.text = currentCharacterData.movementPoints.ToString();
        }

        /// <summary>
        /// Triggered when the player starts dragging a card
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            _lastMousePos = eventData.position;
        }

        /// <summary>
        /// Called every frame when the player is dragging a card or a character
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 _currentMousePos = eventData.position;
            Vector2 _offset = _currentMousePos - _lastMousePos;
            RectTransform _rectTransform = GetComponent<RectTransform>();
            Vector3 _newPos = _rectTransform.position + new Vector3(_offset.x, _offset.y, transform.position.z);
            Vector3 _oldPos = _rectTransform.position;

            _rectTransform.position = _newPos;

            if (!IsRectTransformInsideSreen(_rectTransform))
                _rectTransform.position = _oldPos;

            else
            {
                if (_currentMousePos.y <= Screen.height / 2)
                {
                    if (_content.activeInHierarchy)
                    {
                        _content.SetActive(false);
                        BuyManager.instance.GetCharacter(_character);
                    }
                }

                else if (!_content.activeInHierarchy)
                {
                    _content.SetActive(true);
                    BuyManager.instance.DeleteCharacter();
                }
            }

            _lastMousePos = _currentMousePos;
        }

        /// <summary>
        /// Triggered when the player lets go of the mouse before dragging
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            BuyManager.instance.PlaceCharacter(_character);

            if (!BuyManager.instance.canPlace)
            {
                _content.SetActive(true);
                transform.position = _startingPos;
            }

            else
            {
                Destroy(this.gameObject);
                BuyManager.instance.canPlace = false;
            }
        }

        /// <summary>
        /// Checks if the mouse in inside the screen space and prevents the card from leaving it
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        private bool IsRectTransformInsideSreen(RectTransform rectTransform)
        {
            bool _isInside = false;
            Vector3[] _corners = new Vector3[4];
            rectTransform.GetWorldCorners(_corners);
            int _visibleCorners = 0;
            Rect _rect = new Rect(0, 0, Screen.width, Screen.height);

            foreach (Vector3 corner in _corners)
            {
                if (_rect.Contains(corner))
                    _visibleCorners++;
            }

            if (_visibleCorners == 4)
                _isInside = true;

            return _isInside;
        }
    }
}