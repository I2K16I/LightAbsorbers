using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BSA
{
	public class TransitionManager : MonoBehaviour
	{
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private bool _isOnHomeScreen = false;
        [SerializeField] private RectTransform _joinScreen;
		[SerializeField] private RectTransform _gameScreen;

        [SerializeField] private Vector3 _screenStartPos = new Vector3(-75, 0, -6);
        [SerializeField] private Vector3 _screenBlockPos = new Vector3(0, 0, -6);
        [SerializeField] private Vector3 _screenEndPos = new Vector3(75, 0, -6);
        

        // --- Properties ---------------------------------------------------------------------------------------------

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Awake()
		{
			//_joinScreen.position = new Vector3(0, 0, -10);
            _joinScreen.localPosition = new Vector3(0, 0, -10);
            if(_isOnHomeScreen)
            {
                StartCoroutine(FadeRoutine(1f, _joinScreen.gameObject.GetComponent<Image>()));
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
            
            StartCoroutine(DelayedMoveTillDennisHelpsMeRoutine(totalTransitionTime/2, _gameScreen));   

            // Wird nicht ausgeführt, wieso?
            //this.DoAfter(totalTransitionTime/2, () => MoveScreenCoverOutRoutine(totalTransitionTime / 2, _gameScreen));
        }

        public void MoveFromMainMenuToGame(float transitionTime)
        {
            StartCoroutine(MoveScreenCoverInRoutine(transitionTime, _gameScreen));
        }
        // --- Protected/Private Methods ------------------------------------------------------------------------------
        private IEnumerator MoveScreenCoverOutRoutine(float duration, RectTransform screen)
        {
            Debug.Log(screen.gameObject.name);
            screen.localPosition = _screenBlockPos;
            float timeSinceTransitionStart = 0f;
            while (timeSinceTransitionStart < duration) 
            {
                timeSinceTransitionStart += Time.deltaTime;
                screen.localPosition = Vector3.Lerp(_screenBlockPos, _screenEndPos, timeSinceTransitionStart / duration);
                yield return null;
            }
            screen.localPosition = _screenEndPos;
        }

        private IEnumerator MoveScreenCoverInRoutine(float duration, RectTransform screen)
        {
            screen.localPosition = _screenStartPos;
            float timeSinceTransitionStart = 0f;
            while(timeSinceTransitionStart < duration)
            {
                timeSinceTransitionStart += Time.deltaTime;
                screen.localPosition = Vector3.Lerp(_screenStartPos, _screenBlockPos, timeSinceTransitionStart / duration);
                yield return null;
            }
            screen.localPosition = _screenBlockPos;
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

        private IEnumerator DelayedMoveTillDennisHelpsMeRoutine(float duration, RectTransform screen)
        {
            yield return new WaitForSeconds(duration);
            screen.localPosition = _screenBlockPos;
            float timeSinceTransitionStart = 0f;
            while(timeSinceTransitionStart < duration)
            {
                timeSinceTransitionStart += Time.deltaTime;
                screen.localPosition = Vector3.Lerp(_screenBlockPos, _screenEndPos, timeSinceTransitionStart / duration);
                yield return null;
            }
            screen.localPosition = _screenEndPos;
        }
        // ----------------------------------------------------------------------------------------
    }
}