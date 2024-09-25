using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BSA.UI
{
    public class SimpleButton : MenuItem, IPointerClickHandler
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] AudioSource _clickSfxPlayer;
        [SerializeField] AudioClip _clickSfx;
        // --- Properties ---------------------------------------------------------------------------------------------

        // --- Events -------------------------------------------------------------------------------------------------
        public event Action<SimpleButton> OnClick;

        // --- Unity Functions ----------------------------------------------------------------------------------------        
        private void Start()
        {
            if(_clickSfx != null)
            {
                _clickSfxPlayer.clip = _clickSfx;
            }
        }
        // --- Interface implementations ------------------------------------------------------------------------------
        public void OnPointerClick(PointerEventData eventData)
        {
            Click();
        }

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public void Click()
        {
            OnClick?.Invoke(this);

            if(_clickSfx == null)
                return;
            _clickSfxPlayer.Play();
        }

        // --- Protected/Private Methods ------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
    }
}