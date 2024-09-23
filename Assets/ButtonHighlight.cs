using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BSA
{
	public class ButtonHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
    {

        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] float _fadeDuration = 0.2f;
        [SerializeField] RectTransform _underline;

        // --- Properties ---------------------------------------------------------------------------------------------

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Awake()
		{
			
		}		
        public void OnSelect(BaseEventData eventData)
        {
            this.AutoLerp(0f, 1f, _fadeDuration, p => _underline.anchorMax = new Vector2(p,1), EasingType.Smooth);    
        }

        public void OnDeselect(BaseEventData eventData)
        {   
            this.AutoLerp(1f, 0f, _fadeDuration, p => _underline.anchorMax = new Vector2(p, 1), EasingType.Smooth);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.AutoLerp(0f, 1f, _fadeDuration, p => _underline.anchorMax = new Vector2(p, 1), EasingType.Smooth);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.AutoLerp(1f, 0f, _fadeDuration, p => _underline.anchorMax = new Vector2(p, 1), EasingType.Smooth);
        }


        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------

        // --- Protected/Private Methods ------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
    }
}