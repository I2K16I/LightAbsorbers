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
                StartCoroutine(MoveScreenCoverOutRoutine(1f, _joinScreen));
            }
		}


        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public void MoveFromJoinToGameScreen(float totalTransitionTime)
        {
            StartCoroutine(MoveScreenCoverInRoutine(totalTransitionTime/2, _joinScreen));
            
            this.DoAfter(totalTransitionTime/2, () => StartCoroutine(MoveScreenCoverOutRoutine(totalTransitionTime / 2, _joinScreen)));
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
            float timeSinceTransitionStart = 0f;
            while(timeSinceTransitionStart < duration)
            {
                timeSinceTransitionStart += Time.deltaTime;
                min.x = Mathf.Lerp(-1, 0, timeSinceTransitionStart / duration);
                max.x = Mathf.Lerp(0, 1, timeSinceTransitionStart / duration);
                screen.anchorMin = min;
                screen.anchorMax = max;
                yield return null;
            }
            screen.anchorMin = new Vector2(0, 0);
            screen.anchorMax = new Vector2(1, 1);
        }

        private IEnumerator FadeRoutine(float duration, Image fade)
        {
            fade.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            fade.color = Color.black;
            float timeSinceTransitionStart = 0f;
            while (timeSinceTransitionStart < duration)
            {
                timeSinceTransitionStart += Time.deltaTime;
                fade.color = new Color(0,0,0, Mathf.Lerp(1, 0, timeSinceTransitionStart/duration));
                yield return null;
            }
            fade.color = new Color(0, 0, 0, 0);
            fade.gameObject.SetActive(false);
        }

        // ----------------------------------------------------------------------------------------
    }
}