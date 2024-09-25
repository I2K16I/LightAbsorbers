using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BSA.UI
{
    public class Carousel : MenuItem
    {
        [System.Serializable]
        public class Item
        {
            public string displayString;
            public int value;
        }

        // --- Fields -------------------------------------------------------------------------------------------------
        [Header("Carousel")]
        [SerializeField] private TextMeshProUGUI _titleLabel;
        [SerializeField] private TextMeshProUGUI _valueLabel;
        [SerializeField] private SimpleButton _leftButton;
        [SerializeField] private TextMeshProUGUI _leftButtonLabel;
        [SerializeField] private SimpleButton _rightButton;
        [SerializeField] private TextMeshProUGUI _rightButtonLabel;
        [SerializeField] private Color _noMoreOptionsColor;
        [SerializeField] private Color _buttonColor;
        [SerializeField] private bool _wrapAround;
        [Space]
        [SerializeField] private List<Item> _items = new();
        [SerializeField] private AudioSource _valueChangeSfx;

        private int _activeItemIndex = 0;

        // --- Properties ---------------------------------------------------------------------------------------------
        private Item ActiveItem => _activeItemIndex >= 0 && _activeItemIndex < _items.Count ? _items[_activeItemIndex] : null;

        // --- Events -------------------------------------------------------------------------------------------------
        public event Action<Item> ActiveItemChanged;

        // --- Unity Functions ----------------------------------------------------------------------------------------
        protected override void Awake()
        {
            base.Awake();

            if(_items.Count > 0)
            {
                SetItem(0, true);
            }
        }

        private void OnEnable()
        {
            _leftButton.OnClick += OnLeftClicked;
            _rightButton.OnClick += OnRightClicked;
        }

        private void OnDisable()
        {
            _leftButton.OnClick -= OnLeftClicked;
            _rightButton.OnClick -= OnRightClicked;
        }

        // --- Interface implementations ------------------------------------------------------------------------------


        // --- Event callbacks ----------------------------------------------------------------------------------------
        private void OnLeftClicked(SimpleButton obj)
        {
            MoveItemSelection(-1);
        }

        private void OnRightClicked(SimpleButton obj)
        {
            MoveItemSelection(1);
        }

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public void SetItem(int index, bool silent = false)
        {
            if(_items.Count == 0)
                return;

            _activeItemIndex = Mathf.Clamp(index, 0, _items.Count - 1);
            Debug.Log($"{name} -> {_activeItemIndex:00} ({index})", this);

            Item selection = ActiveItem;
            _valueLabel.text = selection.displayString;


            if(silent == false)
            {
                ActiveItemChanged?.Invoke(ActiveItem);
            }


            if(_activeItemIndex == 0 || _activeItemIndex == _items.Count - 1)
            {
                if(_wrapAround)
                    return;

                if(_activeItemIndex == 0)
                {
                    _leftButtonLabel.color = _noMoreOptionsColor;
                }
                else if(_activeItemIndex == _items.Count - 1)
                {
                    _rightButtonLabel.color = _noMoreOptionsColor;
                }
            } else
            {
                _leftButtonLabel.color = _buttonColor;
                _rightButtonLabel.color = _buttonColor;
            }

        }

        public void MoveItemSelection(int movement)
        {
            if(_items.Count == 0)
                return;

            int newIndex = _activeItemIndex + movement;
            if(_wrapAround)
            {
                if(newIndex >= _items.Count)
                {
                    newIndex %= _items.Count;
                }
                else if(newIndex < 0)
                {
                    do
                    {
                        newIndex += _items.Count;
                    }
                    while(newIndex < 0);
                }
            }

            _valueChangeSfx.Play();
            SetItem(newIndex);
        }

        // --- Protected/Private Methods ------------------------------------------------------------------------------


        // ----------------------------------------------------------------------------------------
    }
}