using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

namespace BSA
{
	public class Countdown : MonoBehaviour
	{
		// --- Fields -------------------------------------------------------------------------------------------------
		[SerializeField] private TMP_Text _countdown;	
		// --- Properties ---------------------------------------------------------------------------------------------
		
		// --- Events -------------------------------------------------------------------------------------------------

		// --- Unity Functions ----------------------------------------------------------------------------------------
		private void Awake()
		{
			StartCoroutine(CountDownRoutine());	
		}		

		// --- Interface implementations ------------------------------------------------------------------------------

		// --- Event callbacks ----------------------------------------------------------------------------------------

		// --- Public/Internal Methods --------------------------------------------------------------------------------

		// --- Protected/Private Methods ------------------------------------------------------------------------------
		private IEnumerator CountDownRoutine()
		{
			yield return new WaitForSeconds(GameManager.Settings.TransitionTime/2);

			int start = (int)Mathf.Floor(GameManager.Settings.TimeBetweenTransitionAndStart);

			for (int i = start; i > 0; i--)
			{
				_countdown.text = "" + i;
				yield return new WaitForSeconds(1f);
			}
			_countdown.text = "Go!";
			
			this.AutoLerp(1,0, .5f, p => _countdown.alpha = p);
		}
		// ----------------------------------------------------------------------------------------
	}
}