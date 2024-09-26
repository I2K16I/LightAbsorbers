using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BSA
{
	public class ButtonHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
    {

        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] float _fadeDuration = 0.2f;
        [SerializeField] RectTransform _underline;
        [SerializeField] AudioSource _audioSource;

        private Coroutine _openAndCloseRoutine;
        private float _currentMax = 0f;
        // --- Properties ---------------------------------------------------------------------------------------------

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Awake()
		{
            this.DoAfter(.5f, () => _audioSource.enabled = true);
		}
        
        // If the button is selected again before the animation is finished, it should cancel wit closing animation and start the 
        // open animation from the point it is currently at, instead of finishing the closing animation and then starting from 
        // 0 again.
        public void OnSelect(BaseEventData eventData)
        {
            if(_audioSource.enabled)
            {
                _audioSource.Play();
            }

            _currentMax = _underline.anchorMax.x;

            if(_openAndCloseRoutine != null)
            {
                StopCoroutine(_openAndCloseRoutine);
                _openAndCloseRoutine = null;
            }
            _openAndCloseRoutine = this.AutoLerp(_currentMax, 1f, _fadeDuration, p => _underline.anchorMax = new Vector2(p,1), EasingType.Smooth);    
        }

        // The same problem as with the OnSelect Method
        public void OnDeselect(BaseEventData eventData)
        {
            _currentMax = _underline.anchorMax.x;

            if (_openAndCloseRoutine != null)
            {
                StopCoroutine(_openAndCloseRoutine);
                _openAndCloseRoutine = null;
            }
            _openAndCloseRoutine = this.AutoLerp(_currentMax, 0f, _fadeDuration, p => _underline.anchorMax = new Vector2(p, 1), EasingType.Smooth);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }




        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------

        // --- Protected/Private Methods ------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
    }
}