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
        [SerializeField] private SkinnedMeshRenderer _banner;
        [SerializeField] private MeshRenderer _podium;
        [SerializeField] private Color _noPlayerColor;
        [SerializeField] private PlayerJoinUI _spriteManager;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Cloth _bannerCloth;

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
            _podium.material.color = Player.MainColor;
            if(Player.IsReady)
            {
                _banner.material.SetColor("_BaseColor", Player.MainColor);
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
            _podium.material.color = _noPlayerColor;
            _banner.material.SetColor("_BaseColor", _noPlayerColor);
            _spriteManager.SetStatusPlayerLeft();
        }

        public void UpdateReady()
        {
            if(Player != null)
            {
                if(Player.IsReady)
                {
                    _banner.material.SetColor("_BaseColor", Player.MainColor);
                }
                else
                {
                    _banner.material.SetColor("_BaseColor", _noPlayerColor);
                }
                _spriteManager.UpdateReadyStatus(Player.IsReady);
            }
            else
            {
                _banner.material.SetColor("_BaseColor", _noPlayerColor);
            }
        }

        public void DisableCloth()
        {
            _bannerCloth.enabled = false;
        }
        // --- Protected/Private Methods ------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
    }
}