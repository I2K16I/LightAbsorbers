using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BSA
{
    public class MusicPlayer : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioClip _menuClip;
        [SerializeField] private AudioClip _gameClip;
        [SerializeField] private AudioClip _winClip;
        [SerializeField] private float _menuFadeDuration = 2f;
        [SerializeField] private float _gameFadeDuration = 1f;

        private Coroutine _stopRoutine = null;
        private float startVolume = 1f;
        // --- Properties ---------------------------------------------------------------------------------------------
        public static MusicPlayer Instance { get; private set; }
        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;

            DontDestroyOnLoad(this.gameObject);

            this.DoAfter(1f, () => _musicSource.Play());
                
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public void SwitchMusic(MusicType type, bool autoStart = true)
        {
            if(_stopRoutine != null)
            {
                StopCoroutine(_stopRoutine);
                _stopRoutine = null;
                _musicSource.volume = startVolume;
            }

            _musicSource.loop = true;

            switch(type)
            {
                case MusicType.Menu:
                    _musicSource.clip = _menuClip;
                    break;
                case MusicType.Game:
                    _musicSource.clip = _gameClip;
                    break;
                case MusicType.Win:
                    _musicSource.loop = false;
                    _musicSource.clip = _winClip;
                    break;
            }

            if(autoStart)
            {
                _musicSource.Play();
            }
        }

        public void StopMusicWithFade()
        {
            AudioClip currentClip = _musicSource.clip;

            if(currentClip == _menuClip)
            {
                _stopRoutine = StartCoroutine(FadeVolumeRoutine(_menuFadeDuration));
            }
            else if(currentClip == _gameClip)
            {
                _stopRoutine = StartCoroutine(FadeVolumeRoutine(_gameFadeDuration));
            }
        }

        public void PlayMusic()
        {
            _musicSource.Play();
        }
        public void PauseMusic()
        {
            _musicSource.Pause();
        }

        public void ResumeMusic()
        {
            _musicSource.UnPause();
        }

        public void StopMusicWithFade(float fadeDuration)
        {
            AudioClip currentClip = _musicSource.clip;
            _stopRoutine = StartCoroutine(FadeVolumeRoutine(fadeDuration));
        }
        // --- Protected/Private Methods ------------------------------------------------------------------------------
        private IEnumerator FadeVolumeRoutine(float duration)
        {
            startVolume = _musicSource.volume;
            yield return this.AutoLerp(startVolume, 0f, duration, Fade);

            _musicSource.Stop();
            _musicSource.volume = startVolume;

            _stopRoutine = null;

            void Fade(float volume)
            {
                _musicSource.volume = volume;
            }
        }
        // ----------------------------------------------------------------------------------------
    }
}