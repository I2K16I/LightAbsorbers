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
					player.MaterialId = i;
					player.PositionId = spawnIndex;
					_materialIsUsed[i] = true;
					player.ChangeMaterial(_playerMaterials[i]);

					int indexToChangeMaterial = GameManager.Instance.PlayerCount;
					//_bannerRenderer[indexToChangeMaterial].material = _playerMaterials[i];
					_podiumRenderer[indexToChangeMaterial].material = _playerMaterials[i];
					break;
				}
			}
		}

		public void ColorBanner(PlayerMovement player)
		{
			if(player.IsReady)
			{
				_bannerRenderer[player.PositionId].material = _playerMaterials[player.MaterialId];
			} else
			{
                _bannerRenderer[player.PositionId].material = _noPlayerMaterial;
            }
		}

		public void ReleaseMaterialLock(PlayerMovement player, int index, List<PlayerMovement> remainingPlayers)
		{
			_materialIsUsed[player.MaterialId] = false;	

			// Hier noch die Banner rechts vom Spieler der gegangen ist umfärben
			for(int i = index ; i < _podiumRenderer.Length; i++)
			{
				if(i<remainingPlayers.Count && remainingPlayers[i].IsReady)
				{
					_bannerRenderer[i].material = _playerMaterials[remainingPlayers[i].MaterialId];
					remainingPlayers[i].PositionId = i;
					_podiumRenderer[i].material = _playerMaterials[remainingPlayers[i].MaterialId];
				} else if(i<remainingPlayers.Count && !remainingPlayers[i].IsReady)
				{
                    _bannerRenderer[i].material = _noPlayerMaterial;
                    remainingPlayers[i].PositionId = i;
                    _podiumRenderer[i].material = _playerMaterials[remainingPlayers[i].MaterialId];
				} else
				{
                    _bannerRenderer[i].material = _noPlayerMaterial;
					_podiumRenderer[i].material = _noPlayerMaterial;
				}
			}
		}
		
		// --- Protected/Private Methods ------------------------------------------------------------------------------
		
		// ----------------------------------------------------------------------------------------
	}
}