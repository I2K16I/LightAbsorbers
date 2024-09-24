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
        [SerializeField] private Transform _podiumObject;
        [SerializeField] private Color _noPlayerColor;
        [SerializeField] private Color _noPlayerBannerColor;
        [SerializeField] private PlayerJoinUI _spriteManager;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Cloth _bannerCloth;
        [SerializeField] private Transform _targetPosition;
        private Vector3 _startPos;
        private Coroutine _coroutine;
        // --- Properties ---------------------------------------------------------------------------------------------
        public PlayerMovement Player { get; private set; }

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Awake()
        {
            _startPos = _podiumObject.transform.position;
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public void AssignPlayer(PlayerMovement player)
        {
            Player = player;
            _podium.material.color = Player.MainColor * 1.5f + _noPlayerColor;
            if(Player.IsReady)
            {
                _banner.material.SetColor("_BaseColor", Player.MainColor);
            }
            _spriteManager.UpdateReadyStatus(Player.IsReady);
            Player.transform.position = _spawnPoint.position;
            Player.transform.rotation = _spawnPoint.rotation;

            if(_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = StartCoroutine(UpdatePodiumRoutine());
        }

        public void RemovePlayer()
        {
            if(this == null)
                return;

            Player = null;
            _podium.material.color = _noPlayerColor;
            _banner.material.SetColor("_BaseColor", _noPlayerBannerColor);
            _spriteManager.SetStatusPlayerLeft();

            if(_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = StartCoroutine(UpdatePodiumRoutine());
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
                    _banner.material.SetColor("_BaseColor", _noPlayerBannerColor);
                }
                _spriteManager.UpdateReadyStatus(Player.IsReady);
            }
            else
            {
                _banner.material.SetColor("_BaseColor", _noPlayerBannerColor);
            }
        }

        public void DisableCloth()
        {
            _bannerCloth.enabled = false;
        }
        // --- Protected/Private Methods ------------------------------------------------------------------------------
        private IEnumerator UpdatePodiumRoutine()
        {
            yield return new WaitForSeconds(.2f);
            float startY = 0f;
            float targetY = 0f;
            Vector3 newPos = _startPos;

            if(Player != null)
            {
                startY = _podiumObject.position.y;
                targetY = _targetPosition.position.y;
                Player.transform.parent = _podium.transform;
                yield return this.AutoLerp(startY, targetY, 1f, SetNewPos, EasingType.EasyOutQuart);
            }
            else
            {
                startY = _podiumObject.position.y;
                targetY = _startPos.y;
                yield return this.AutoLerp(startY, targetY, 1f, SetNewPos, EasingType.EasyInQuart);
            }



            if(Player != null)
            {
                _podiumObject.position = _targetPosition.position;
                Player.transform.parent = null;
                DontDestroyOnLoad(Player.gameObject);
            }
            else
            {
                _podiumObject.position = _startPos;
            }

            // Maybe fade in UI here

            _coroutine = null;

            void SetNewPos(float newY)
            {
                newPos.y = newY;
                _podiumObject.position = newPos;
            }
        }

        // ----------------------------------------------------------------------------------------
    }
}