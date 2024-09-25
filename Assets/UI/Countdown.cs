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
		[SerializeField] private AudioSource _sfx;
		[SerializeField] private AudioClip _countdownSound;
		[SerializeField] private AudioClip _startSound;
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
			_sfx.clip = _countdownSound;
			int start = (int)Mathf.Floor(GameManager.Settings.TimeBetweenTransitionAndStart);

			for (int i = start; i > 0; i--)
			{
				_countdown.text = "" + i;
				_sfx.Play();
				yield return new WaitForSeconds(1f);
			}
			_sfx.clip = _startSound;
			_sfx.Play();
			_countdown.text = "Go!";
			
			this.AutoLerp(1,0, .5f, p => _countdown.alpha = p);
		}
		// ----------------------------------------------------------------------------------------
	}
}