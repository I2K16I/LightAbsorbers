using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Device;

namespace BSA
{
	public class EndScreenPrompts : MonoBehaviour
	{
		// --- Fields -------------------------------------------------------------------------------------------------
		[SerializeField] private RectTransform _promptTransform;
		[SerializeField] private float _duration = 1f;
		// --- Properties ---------------------------------------------------------------------------------------------
		
		// --- Events -------------------------------------------------------------------------------------------------

		// --- Unity Functions ----------------------------------------------------------------------------------------
		private void Awake()
		{
			
		}		

		// --- Interface implementations ------------------------------------------------------------------------------

		// --- Event callbacks ----------------------------------------------------------------------------------------

		// --- Public/Internal Methods --------------------------------------------------------------------------------
		public void SlideIn()
		{
			StartCoroutine(SetMinAndMaxAnchor());
		}
		// --- Protected/Private Methods ------------------------------------------------------------------------------
		private IEnumerator SetMinAndMaxAnchor()
		{
            Vector2 min = new Vector2(0, -1);
            Vector2 max = new Vector2(0, 0);

            _promptTransform.anchorMin = min;
            _promptTransform.anchorMax = max;

            this.AutoLerp(min.y, min.y + 1, _duration, SetAnchorMin);
            yield return this.AutoLerp(max.y, max.y + 1, _duration, SetAnchorMax);

            _promptTransform.anchorMin = new Vector2(0, 0);
            _promptTransform.anchorMax = new Vector2(1, 1);

            void SetAnchorMin(float minimum)
            {
                min.y = minimum;
                _promptTransform.anchorMin = min;

            }
            void SetAnchorMax(float maximum)
            {
                max.y = maximum;
                _promptTransform.anchorMax = max;
            }
        }
		// ----------------------------------------------------------------------------------------
	}
}