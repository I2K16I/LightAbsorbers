using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BSA
{
	public class BeamSpawner : MonoBehaviour
	{
		// --- Fields -------------------------------------------------------------------------------------------------
		[SerializeField] private GameObject _beam;
		[SerializeField] private int _maxNumberOfAttacks = 4;

		private int _currentNumberOfAttacks = 0;
		// --- Properties ---------------------------------------------------------------------------------------------
		
		// --- Events -------------------------------------------------------------------------------------------------

		// --- Unity Functions ----------------------------------------------------------------------------------------
		private void Awake()
		{
			
		}		

		// --- Interface implementations ------------------------------------------------------------------------------

		// --- Event callbacks ----------------------------------------------------------------------------------------

		// --- Public/Internal Methods --------------------------------------------------------------------------------
		public void SpawnBeamAt(Transform pos1, Transform pos2, float duration)
		{
			if(_currentNumberOfAttacks <= _maxNumberOfAttacks)
			{
				GameObject newBeam = Instantiate(_beam);
				newBeam.GetComponentInChildren<BeamManager>().SetNewProperties(pos1, pos2, duration);
				this.DoAfter(duration, EndOfAttack);
			}
		}
		// --- Protected/Private Methods ------------------------------------------------------------------------------
		private void EndOfAttack()
		{
			_currentNumberOfAttacks--;
		}
		// ----------------------------------------------------------------------------------------
	}
}