using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BSA
{
	public class PlayerJoinUI : MonoBehaviour
	{
		// --- Fields -------------------------------------------------------------------------------------------------
		[SerializeField] private GameObject _joinButton;
		[SerializeField] private GameObject _readyButton;
		[SerializeField] private GameObject _notReadyButton;
		[SerializeField] private GameObject _leaveButton;

		// --- Properties ---------------------------------------------------------------------------------------------
		
		// --- Events -------------------------------------------------------------------------------------------------

		// --- Unity Functions ----------------------------------------------------------------------------------------
		private void Awake()
		{
			
		}		

		// --- Interface implementations ------------------------------------------------------------------------------

		// --- Event callbacks ----------------------------------------------------------------------------------------

		// --- Public/Internal Methods --------------------------------------------------------------------------------

		public void UpdateReadyStatus(bool IsReady)
		{
			_joinButton.SetActive(false);
            _readyButton.SetActive(!IsReady);
            _notReadyButton.SetActive(IsReady);
            _leaveButton.SetActive(!IsReady);
        }

		public void SetStatusPlayerLeft()
		{
			_joinButton.SetActive(true);
			_readyButton.SetActive(false);
			_notReadyButton.SetActive(false);
			_leaveButton.SetActive(false);

		}


		// --- Protected/Private Methods ------------------------------------------------------------------------------
		
		// ----------------------------------------------------------------------------------------
	}
}