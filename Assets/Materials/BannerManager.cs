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
            public Material material;
            public Color capeColor;
            public Color metalColor;
            public bool isInUse;
        }


        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private PlayerMaterial[] _playerMaterials;

        [SerializeField] private List<Banner> _banners;


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
            player.Material = playerMaterial.material;
            player.CapeColor = playerMaterial.capeColor;
            player.MetalColor = playerMaterial.metalColor;
            playerMaterial.isInUse = true;
            player.ChangeMaterial();

            Banner banner = _banners[player.PositionId];
            banner.AssignPlayer(player);
        }

        public void ColorBanner(PlayerMovement player)
        {
            _banners[player.PositionId].UpdateReady();
        }

        public void PlayerLeft(PlayerMovement player)
        {
            PlayerMaterial playerMaterial = _playerMaterials.First(pm => pm.material == player.Material);
            playerMaterial.isInUse = false;

            Banner banner = _banners[player.PositionId];
            banner.RemovePlayer();

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