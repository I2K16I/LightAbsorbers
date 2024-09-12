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
		[SerializeField] private Beam _beam;
		[SerializeField] private int _maxNumberOfAttacks = 4;
		
		private Settings _settings;
		private int _currentNumberOfAttacks = 0;

		// --- Properties ---------------------------------------------------------------------------------------------
		
		// --- Events -------------------------------------------------------------------------------------------------

		// --- Unity Functions ----------------------------------------------------------------------------------------
		private void Awake()
		{
			_settings = GameManager.Settings;
		}		

		// --- Interface implementations ------------------------------------------------------------------------------

		// --- Event callbacks ----------------------------------------------------------------------------------------

		// --- Public/Internal Methods --------------------------------------------------------------------------------
		public void SpawnBeamAt(Transform pos1, Transform pos2)
		{
			if(_currentNumberOfAttacks <= _maxNumberOfAttacks)
			{
                Beam newBeam = Instantiate(_beam);
				newBeam.PerformAttack(pos1, pos2);
				this.DoAfter(_settings.BeamActiveDuration, EndOfAttack);
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