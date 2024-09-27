using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BSA
{
	public class TransitionHandler : MonoBehaviour
	{
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private bool _isOnHomeScreen = false;
        [SerializeField] private GameObject _homeScreenFade;
        [SerializeField] private RectTransform _joinScreen;
        [SerializeField] private GameObject _deviceLostCanvas;
        [SerializeField] private TMP_Text _deviceLostText;
        [SerializeField] private EndScreenPrompts _endScreenPrompts;
        // --- Properties ---------------------------------------------------------------------------------------------

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Awake()
		{
            if(_isOnHomeScreen)
            {
                StartCoroutine(FadeRoutine(1f, _homeScreenFade.gameObject.GetComponent<Image>()));
            }
            else
            { 
                StartCoroutine(MoveScreenCoverOutRoutine(GameManager.Settings.TransitionTime/2, _joinScreen));
                GameManager.Instance.TransitionHandler = this;
            }
		}

        private void OnDestroy()
        {
            
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public void SwtichFromScene(float totalTransitionTime)
        {
            StartCoroutine(MoveScreenCoverInRoutine(totalTransitionTime, _joinScreen));
        }

        public void MoveFromMainMenuToGame(float transitionTime)
        {
            StartCoroutine(MoveScreenCoverInRoutine(transitionTime, _joinScreen));
        }

        public void ShowDeviceLostScreen(int deviceNumber)
        {
            _deviceLostCanvas.SetActive(true);
            int displayedDeviceNumber = deviceNumber + 1;
            _deviceLostText.text = "Controller " + displayedDeviceNumber + " disconnected!";
        }

        public void ShowWinnerBanner()
        {
            _endScreenPrompts.SlideWinIn();
        }
        public void ShowEndGamePrompts()
        {
            _endScreenPrompts.SlidePromptsIn();
        }

        public void HideDeviceLostScreen()
        {
            _deviceLostCanvas?.SetActive(false);
        }
        // --- Protected/Private Methods ------------------------------------------------------------------------------
        private IEnumerator MoveScreenCoverOutRoutine(float duration, RectTransform screen)
        {

            Vector2 min = new Vector2(0, 0);
            Vector2 max = new Vector2(1, 1);
            screen.anchorMin = min;
            screen.anchorMax = max;
            float timeSinceTransitionStart = 0f;
            while (timeSinceTransitionStart < duration) 
            {
                timeSinceTransitionStart += Time.deltaTime;
                min.x = Mathf.Lerp(0, 1, timeSinceTransitionStart / duration);
                max.x = Mathf.Lerp(1, 2, timeSinceTransitionStart / duration);
                screen.anchorMin = min;
                screen.anchorMax = max;
                yield return null;
            }
            screen.anchorMin = new Vector2(1, 0);
            screen.anchorMax = new Vector2(2, 1);
        }

        private IEnumerator MoveScreenCoverInRoutine(float duration, RectTransform screen)
        {
            Vector2 min = new Vector2(-1, 0);
            Vector2 max = new Vector2(0, 1);

            screen.anchorMin = min;
            screen.anchorMax = max;

            this.AutoLerp(min.x, min.x + 1, duration, SetAnchorMin);
            yield return this.AutoLerp(max.x, max.x +1, duration, SetAnchorMax);

            screen.anchorMin = new Vector2(0, 0);
            screen.anchorMax = new Vector2(1, 1);

            void SetAnchorMin(float minimum)
            {
                min.x = minimum;
                screen.anchorMin = min;

            }
            void SetAnchorMax(float maximum)
            {
                max.x = maximum;
                screen.anchorMax = max;
            }
        }

        private IEnumerator FadeRoutine(float duration, Image fade)
        {
            fade.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            fade.color = Color.black;

            yield return this.AutoLerp(1, 0, duration, SetAlphaValue);

            fade.color = new Color(0, 0, 0, 0);
            fade.gameObject.SetActive(false);

            void SetAlphaValue(float alpha)
            {
                fade.color = new Color(0,0,0,alpha);
            }
        }

        // ----------------------------------------------------------------------------------------
    }
}