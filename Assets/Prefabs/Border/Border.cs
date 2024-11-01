using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BSA
{
	public class Border : MonoBehaviour
	{
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private BoxCollider _playerCollider;
		// --- Properties ---------------------------------------------------------------------------------------------
		
		// --- Events -------------------------------------------------------------------------------------------------

		// --- Unity Functions ----------------------------------------------------------------------------------------
		private void Awake()
		{
            if (OptionsManager.Options.bordersKillPlayers)
            {
                _playerCollider.isTrigger = true;
            } else
            {
                _playerCollider.isTrigger = false;
            }
		}

        private void OnTriggerEnter(Collider collision)
        {
            if(collision.gameObject.TryGetComponent(out PlayerMovement player))
            {
                if (OptionsManager.Options.bordersKillPlayers)
                {
                    Debug.Log("Hit the player");
                    player.Hit();
                }
            }
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------

        // --- Protected/Private Methods ------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
    }
}