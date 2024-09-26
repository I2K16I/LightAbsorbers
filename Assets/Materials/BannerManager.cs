using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BSA
{
    public class BannerManager : MonoBehaviour
    {
        [System.Serializable]
        public class PlayerMaterial
        {
            public Color mainColor;
            public Color capeColor;
            public Color metalColor;
            public bool isInUse;
        }


        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private PlayerMaterial[] _playerMaterials;

        [SerializeField] private List<Banner> _banners;

        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _playerJoinClip;
        [SerializeField] private AudioClip _playerLeaveClip;
        [SerializeField] private AudioClip _playerReady;


        // --- Properties ---------------------------------------------------------------------------------------------

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Awake()
        {

        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public void PlayerJoined(PlayerMovement player)
        {
            PlayerMaterial playerMaterial = _playerMaterials.First(pm => pm.isInUse == false);
            player.MainColor = playerMaterial.mainColor;
            player.CapeColor = playerMaterial.capeColor;
            player.MetalColor = playerMaterial.metalColor;
            playerMaterial.isInUse = true;
            player.ChangeMaterial();

            _audioSource.clip = _playerJoinClip;
            _audioSource.Play();

            Banner banner = _banners[player.PositionId];
            banner.AssignPlayer(player);
        }

        public void ColorBanner(PlayerMovement player)
        {

            _audioSource.clip = _playerReady;
            _audioSource.Play();

            _banners[player.PositionId].UpdateReady();
        }

        public void DisableCloths()
        {
            foreach(Banner item in _banners)
            {
                item.DisableCloth();
            }
        }

        public void PlayerLeft(PlayerMovement player)
        {
            PlayerMaterial playerMaterial = _playerMaterials.First(pm => pm.mainColor == player.MainColor);
            playerMaterial.isInUse = false;

            Banner banner = _banners[player.PositionId];
            banner.RemovePlayer();

            _audioSource.clip = _playerLeaveClip;
            _audioSource.Play();

            // Iterate banners right of the one that got removed
            int startIndex = player.PositionId + 1;
            for(int i = startIndex; i < _banners.Count; i++)
            {
                if(_banners[i].Player != null)
                {
                    _banners[i].Player.PositionId--;
                    _banners[i - 1].AssignPlayer(_banners[i].Player);
                    _banners[i].RemovePlayer();
                }
            }
        }

        public void UpdateSlotColors(PlayerMovement player, int index)
        {
            _banners[index].AssignPlayer(player);
        }

        public void RemoveSlotColor(int index)
        {
            _banners[index].RemovePlayer();
        }

        // --- Protected/Private Methods ------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
    }
}