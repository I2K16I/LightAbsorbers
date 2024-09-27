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

		[SerializeField] private RectTransform _winBannerTransform;

		// --- Properties ---------------------------------------------------------------------------------------------
		
		// --- Events -------------------------------------------------------------------------------------------------

		// --- Unity Functions ----------------------------------------------------------------------------------------

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public void SlideWinIn()
        {
            StartCoroutine(SetPosOverTime(200f, _winBannerTransform));

        }
        public void SlidePromptsIn()
		{
			StartCoroutine(SetPosOverTime(-200f, _promptTransform));
		}
		// --- Protected/Private Methods ------------------------------------------------------------------------------
		private IEnumerator SetPosOverTime(float startPos, RectTransform objectToMove)
		{
			Vector2 newPos = new Vector2(0, startPos);

            objectToMove.localPosition= newPos;

            yield return this.AutoLerp(newPos.y, 0, _duration, SetPosY);

            objectToMove.localPosition = new Vector2(0, 0);

            void SetPosY(float minimum)
            {
                newPos.y = minimum;
                objectToMove.localPosition = newPos;

            }
        }

        // ----------------------------------------------------------------------------------------
    }
}