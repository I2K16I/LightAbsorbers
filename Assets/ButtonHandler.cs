using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BSA
{
	public class ButtonHandler : MonoBehaviour
	{
		// --- Fields -------------------------------------------------------------------------------------------------
		[SerializeField] private TransitionHandler _transitionManager;

		// --- Properties ---------------------------------------------------------------------------------------------
		
		// --- Events -------------------------------------------------------------------------------------------------

		// --- Unity Functions ----------------------------------------------------------------------------------------
		private void Awake()
		{

		}		

		// --- Interface implementations ------------------------------------------------------------------------------

		// --- Event callbacks ----------------------------------------------------------------------------------------

		// --- Public/Internal Methods --------------------------------------------------------------------------------
		public void OnStartClick()
		{
			_transitionManager.SwtichFromScene(1f);
			this.DoAfter(1f, LoadGame);
		}

		public void OnSettingsClicked()
		{
			Debug.Log("Settings, yay");
		}

		public void OnBackClicked()
		{
			Debug.Log("Back to the main Menu");
		}

		public void OnSaveClicked()
		{
			Debug.Log("Saved Settings");
		}

		public void OnExitClicked()
		{
			Application.Quit();
		}
		// --- Protected/Private Methods ------------------------------------------------------------------------------
		private void LoadGame()
		{
            SceneManager.LoadScene(1);
        }
		// ----------------------------------------------------------------------------------------
	}
}