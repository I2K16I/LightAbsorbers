using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace BSA.UI
{
    public class OptionsMenu : MonoBehaviour, IMoveHandler, ICancelHandler, ISubmitHandler
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private Carousel _masterVolume;
        [SerializeField] private Carousel _musicVolume;
        [SerializeField] private Carousel _sfxVolume;
        [Space]
        [SerializeField] private Carousel _useRumble;
        [SerializeField] private Carousel _beamsReflectOrbs;
        [SerializeField] private Carousel _bordersKillPlayers;
        [Space]
        [SerializeField] private SimpleButton _resetButton;
        [SerializeField] private SimpleButton _saveButton;
        [SerializeField] private SimpleButton _backButton;
        [Space]
        [Header("Audio")]
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _selectSound;


        private MenuItem _currentSelection;

        // --- Properties ---------------------------------------------------------------------------------------------

        // --- Events -------------------------------------------------------------------------------------------------
        public event Action Closed;

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Start()
        {
            LoadOptions();
        }

        private void OnEnable()
        {
            SetSelection(_masterVolume);

            _masterVolume.ActiveItemChanged += OnMasterVolumeChanged;
            _musicVolume.ActiveItemChanged += OnMusicVolumeChanged;
            _sfxVolume.ActiveItemChanged += OnSfxVolumeChanged;

            _useRumble.ActiveItemChanged += OnUseRumbleChanged;
            _beamsReflectOrbs.ActiveItemChanged += OnBeamsReflectOrbsChanged;
            _bordersKillPlayers.ActiveItemChanged += OnBordersKillPlayersChanged;

            _backButton.OnClick += OnBackClicked;
            _resetButton.OnClick += OnResetClicked;
            _saveButton.OnClick += OnSaveClicked;

            foreach(MenuItem item in GetSelectableMenuItems())
            {
                item.Hovered += SetSelection;
            }

            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }

        private void OnDisable()
        {
            LoadOptions();
            _backButton.OnClick -= OnBackClicked;
            _resetButton.OnClick -= OnResetClicked;
            _saveButton.OnClick -= OnSaveClicked;

            foreach(MenuItem item in GetSelectableMenuItems())
            {
                item.Hovered -= SetSelection;
            }
        }

        // --- Interface implementations ------------------------------------------------------------------------------
        public void OnMove(AxisEventData eventData)
        {
            if(eventData.moveDir == MoveDirection.None)
                return;

            if(_currentSelection != null)
            {
                MenuItem neighbor = _currentSelection.GetNeighbor(eventData.moveDir);
                if(neighbor != null)
                {
                    SetSelection(neighbor);
                }
                else if(_currentSelection is Carousel carousel)
                {
                    switch(eventData.moveDir)
                    {
                        case MoveDirection.Left:
                            carousel.MoveItemSelection(-1);
                            break;

                        case MoveDirection.Right:
                            carousel.MoveItemSelection(1);
                            break;
                    }
                }
            }
        }

        public void OnCancel(BaseEventData eventData)
        {
            Close();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if(_currentSelection is SimpleButton button)
            {
                button.Click();
            }
        }

        // --- Event callbacks ----------------------------------------------------------------------------------------
        private void OnBackClicked(SimpleButton backButton)
        {
            Close();
        }

        private void OnSaveClicked(SimpleButton saveButton)
        {
            OptionsManager.Instance.SaveOptions();
        }

        private void OnResetClicked(SimpleButton resetButton)
        {
            OptionsManager.Instance.LoadOptions();
            LoadOptions();
        }

        private void OnMasterVolumeChanged(Carousel.Item item)
        {
            OptionsManager.Options.masterVolume = item.value;
            if(item.value == 0)
            {
                _audioMixer.SetFloat("MasterVolume", -80f);
            }
            else
            {
                _audioMixer.SetFloat("MasterVolume", -40f + 5 * item.value);
            }
        }
        private void OnMusicVolumeChanged(Carousel.Item item)
        {
            OptionsManager.Options.musicVolume = item.value;
            if(item.value == 0)
            {
                _audioMixer.SetFloat("MusicVolume", -80f);
            }
            else
            {
                _audioMixer.SetFloat("MusicVolume", -40f + 5 * item.value);
            }
        }
        private void OnSfxVolumeChanged(Carousel.Item item)
        {
            OptionsManager.Options.sfxVolume = item.value;
            if(item.value == 0)
            {
                _audioMixer.SetFloat("SfxVolume", -80f);
            }
            else
            {
                _audioMixer.SetFloat("SfxVolume", -40f + 5 * item.value);
            }
        }
        private void OnUseRumbleChanged(Carousel.Item item)
        {
            OptionsManager.Options.useRumble = item.value == 1;
        }

        private void OnBeamsReflectOrbsChanged(Carousel.Item item)
        {
            OptionsManager.Options.beamsReflectOrbs = item.value == 1;
        }
        private void OnBordersKillPlayersChanged(Carousel.Item item)
        {
            OptionsManager.Options.bordersKillPlayers = item.value == 1;
        }

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public void SetSelection(MenuItem item)
        {
            if(_currentSelection != null)
            {
                _currentSelection.Deselect();
            }

            _currentSelection = item;

            _audioSource.Play();
            if(_currentSelection != null)
            {
                _currentSelection.Select();
            }
        }

        // --- Protected/Private Methods ------------------------------------------------------------------------------
        private void Close()
        {
            OptionsManager.Instance.LoadOptions();
            LoadOptions();
            gameObject.SetActive(false);
            Closed?.Invoke();
        }

        private IEnumerable<MenuItem> GetSelectableMenuItems()
        {
            yield return _masterVolume;
            yield return _musicVolume;
            yield return _sfxVolume;
            yield return _useRumble;
            yield return _beamsReflectOrbs;
            yield return _bordersKillPlayers;
            yield return _resetButton;
            yield return _saveButton;
            //yield return _backButton;
        }

        private void LoadOptions()
        {
            Options options = OptionsManager.Options;

            // TODO: Set initial values for all our MenuItems to match the values found in Options
            _masterVolume.SetItem(options.masterVolume, true);
            //_audioMixer.SetFloat("MasterVolume", -80f + 10 * options.masterVolume);
            _musicVolume.SetItem(options.musicVolume, true);
            //_audioMixer.SetFloat("MusicVolume", -80f + 10 * options.musicVolume);
            _sfxVolume.SetItem(options.sfxVolume, true);
            //_audioMixer.SetFloat("SfxVolume", -80f + 10 * options.sfxVolume);
            SetAllAudios();

            _useRumble.SetItem(options.useRumble ? 1 : 0, true);
            _beamsReflectOrbs.SetItem(options.beamsReflectOrbs ? 1 : 0, true);
            _bordersKillPlayers.SetItem(options.bordersKillPlayers ? 1 : 0, true);
        }

        private void SetAllAudios()
        {
            Options options = OptionsManager.Options;
            if (options.masterVolume == 0)
            {
                _audioMixer.SetFloat("MasterVolume", -80f);
            }
            else
            {
                _audioMixer.SetFloat("MasterVolume", -40f + 5 * options.masterVolume);
            }
            if (options.musicVolume == 0)
            {
                _audioMixer.SetFloat("MusicVolume", -80f);
            }
            else
            {
                _audioMixer.SetFloat("MusicVolume", -40f + 5 * options.musicVolume);
            }
            if (options.sfxVolume == 0)
            {
                _audioMixer.SetFloat("SfxVolume", -80f);
            }
            else
            {
                _audioMixer.SetFloat("SfxVolume", -40f + 5 * options.sfxVolume);
            }
        }

        // ----------------------------------------------------------------------------------------
    }
}