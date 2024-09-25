using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BSA.UI
{
	public abstract class MenuItem : MonoBehaviour, IPointerEnterHandler
	{
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] protected Image _selectionHighlight;

        [Header("Navigation")]
        [SerializeField] protected MenuItem _neighborUp;
        [SerializeField] protected MenuItem _neighborDown;
        [SerializeField] protected MenuItem _neighborLeft;
        [SerializeField] protected MenuItem _neighborRight;

        protected Dictionary<Vector2Int, MenuItem> _neighbors;

        // --- Properties ---------------------------------------------------------------------------------------------
        public bool IsSelected { get; protected set; }

        // --- Events -------------------------------------------------------------------------------------------------
        public Action<MenuItem> Hovered;

        // --- Unity Functions ----------------------------------------------------------------------------------------
        protected virtual void Awake()
        {
            Deselect();
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------
        public void OnPointerEnter(PointerEventData eventData)
        {
            Hovered?.Invoke(this);
        }

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public virtual void Select()
        {
            IsSelected = true;
            if (_selectionHighlight != null)
            {
                _selectionHighlight.enabled = true;                
            }
        }

        public virtual void Deselect()
        {
            IsSelected = false;
            if (_selectionHighlight != null)
            {
                _selectionHighlight.enabled = false;
            }
        }

        // ----------------------------------------------------------------------------------------
        public MenuItem GetNeighbor(Vector2Int movement)
        {
            CreateNeighbors();
            if(_neighbors.TryGetValue(movement, out MenuItem neighbor))
            {
                return neighbor;
            }

            return null;
        }

        public MenuItem GetNeighbor(MoveDirection direction)
        {
            return direction switch
            {
                MoveDirection.Left => _neighborLeft,
                MoveDirection.Up => _neighborUp,
                MoveDirection.Right => _neighborRight,
                MoveDirection.Down => _neighborDown,
                _ => null,
            };
        }

        // --- Protected/Private Methods ------------------------------------------------------------------------------
        protected void CreateNeighbors()
        {
            _neighbors ??= new Dictionary<Vector2Int, MenuItem>()
                {
                    {  Vector2Int.up, _neighborUp },
                    {  Vector2Int.down , _neighborDown },
                    {  Vector2Int.left, _neighborLeft },
                    {  Vector2Int.right, _neighborRight },
                };
        }

        // ----------------------------------------------------------------------------------------
    }
}