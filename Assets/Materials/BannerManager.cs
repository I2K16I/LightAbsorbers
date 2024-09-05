using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BSA
{
	public class BannerManager : MonoBehaviour
	{
		// --- Fields -------------------------------------------------------------------------------------------------
		[SerializeField] Material _noPlayerMaterial;
		[SerializeField] Material[] _playerMaterials;
		[SerializeField] MeshRenderer[] _bannerRenderer;
		[SerializeField] MeshRenderer[] _podiumRenderer;
		

		private bool[] _materialIsUsed = new bool[4];
		// --- Properties ---------------------------------------------------------------------------------------------
		
		// --- Events -------------------------------------------------------------------------------------------------

		// --- Unity Functions ----------------------------------------------------------------------------------------
		private void Awake()
		{

		}		

		// --- Interface implementations ------------------------------------------------------------------------------

		// --- Event callbacks ----------------------------------------------------------------------------------------

		// --- Public/Internal Methods --------------------------------------------------------------------------------
		public void DeterminColor(PlayerMovement player, int spawnIndex)
		{
			for(int i = 0;  i < _materialIsUsed.Length; i++)
			{
				if(!_materialIsUsed[i])
				{
					player.Id = i;
					_materialIsUsed[i] = true;
					player.ChangeMaterial(_playerMaterials[i]);

					int indexToChangeMaterial = GameManager.Instance.PlayerCount;
					_bannerRenderer[indexToChangeMaterial].material = _playerMaterials[i];
					_podiumRenderer[indexToChangeMaterial].material = _playerMaterials[i];
					break;
				}
			}
		}

		public void ReleaseMaterialLock(PlayerMovement player, int index)
		{
			_materialIsUsed[player.Id] = false;	

			// Hier noch die Banner rechts vom Spieler der gegangen ist umfärben
			for(int i = index ; i < _podiumRenderer.Length; i++)
			{

			}
		}
		
		// --- Protected/Private Methods ------------------------------------------------------------------------------
		
		// ----------------------------------------------------------------------------------------
	}
}