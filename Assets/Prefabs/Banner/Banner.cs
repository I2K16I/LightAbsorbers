using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BSA
{
    public class Banner : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private MeshRenderer _banner;
        [SerializeField] private MeshRenderer _podium;
        [SerializeField] private Material _noPlayerMaterial;
        [SerializeField] private PlayerJoinUI _spriteManager;
        [SerializeField] private Transform _spawnPoint;

        // --- Properties ---------------------------------------------------------------------------------------------
        public PlayerMovement Player { get; private set; }

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Awake()
        {

        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public void AssignPlayer(PlayerMovement player)
        {
            Player = player;
            _podium.material = Player.Material;
            if(Player.IsReady)
            {
                _banner.material = Player.Material;
            }
            _spriteManager.UpdateReadyStatus(Player.IsReady);
            Player.transform.position = _spawnPoint.position;
            Player.transform.rotation = _spawnPoint.rotation;
        }

        public void RemovePlayer()
        {
            if(this == null)
                return;

            Player = null;
            _podium.material = _noPlayerMaterial;
            _banner.material = _noPlayerMaterial;
            _spriteManager.SetStatusPlayerLeft();
        }

        public void UpdateReady()
        {
            if(Player != null)
            {
                if(Player.IsReady)
                {
                    _banner.material = Player.Material;
                }
                else
                {
                    _banner.material = _noPlayerMaterial;
                }
                _spriteManager.UpdateReadyStatus(Player.IsReady);
            }
            else
            {
                _banner.material = _noPlayerMaterial;
            }
        }
        // --- Protected/Private Methods ------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
    }
}