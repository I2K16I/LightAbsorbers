using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BSA.UI
{
    public class OptionsMenu : MonoBehaviour, IMoveHandler, ICancelHandler, ISubmitHandler
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private Carousel _masterVolume;
        [SerializeField] private Carousel _musicVolume;
        [SerializeField] private Carousel _sfxVolume;
        [Space]
        [SerializeField] private Carousel _numberOfOrbs;
        [SerializeField] private Carousel _beamsReflectOrbs;
        [SerializeField] private Carousel _bordersKillPlayers;
        [SerializeField] private Carousel _useRumble;
        [Space]
        [SerializeField] private SimpleButton _resetButton;
        [SerializeField] private SimpleButton _saveButton;
        [SerializeField] private SimpleButton _backButton;

        [Space]
        [Header("ScrollView")]
        [SerializeField] private ScrollRect _scrollRect;

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

            _numberOfOrbs.ActiveItemChanged += OnNumberOfOrbsChanged;
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
                _audioMixer.SetFloat("MasterVolume", -50f + 5 * item.value);
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
                _audioMixer.SetFloat("MusicVolume", -50f + 5 * item.value);
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
                _audioMixer.SetFloat("SfxVolume", -50f + 5 * item.value);
            }
        }
        private void OnNumberOfOrbsChanged(Carousel.Item item)
        {
            OptionsManager.Options.numberOfOrbs = item.value;
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
                //Debug.Log("Selection size: " + _viewPortRect.rect.height);
                RectTransform selectionRt = _currentSelection.transform as RectTransform;
                RectTransform viewport = _scrollRect.viewport;
                RectTransform content = _scrollRect.content;
                
                // Navigate up the hierarchy until we get the element that is a child of our content parent
                while(selectionRt != null && selectionRt.parent != content)
                {
                    selectionRt = selectionRt.parent as RectTransform;
                }
                if(selectionRt == null)
                {
                    Debug.LogWarning($"{item.name} is not a child of the ScrollRects Content.");
                    return;
                }

                // -700 to 0
                Rect visibleContent = viewport.rect;
                //Debug.Log($"Viewport: {visibleContent.yMin:0.##} to {visibleContent.yMax:0.##}");
                // 0 to 700
                visibleContent.y += visibleContent.height * content.pivot.y;
                // 0 to 700 (bottom) / 150 to 850 (top)
                float scrollHeight = content.rect.height - visibleContent.height;
                visibleContent.y += _scrollRect.verticalNormalizedPosition * scrollHeight;
                //Debug.Log($"NormalizedY: {_scrollRect.verticalNormalizedPosition:0.##}" +
                //    $"\nVisible: {visibleContent.yMin:0.##} to {visibleContent.yMax:0.##}");

                float selectionY = content.rect.height + selectionRt.anchoredPosition.y;
                float yMin = selectionY + selectionRt.rect.yMin;
                float yMax = selectionY + selectionRt.rect.yMax;
                //Debug.Log($"Visible: {visibleContent.yMin:0.##} to {visibleContent.yMax:0.##}" +
                //    $"\nSelection: {yMin:0.##} to {yMax:0.##}");

                int scrollDirection = yMin < visibleContent.yMin ? -1
                        : yMax > visibleContent.yMax ? 1
                        : 0;

                if(scrollDirection != 0)
                {
                    // Example
                    // Direction: - 1 Visible: 150, Target: 120 (ContentHeight : 900 : ViewportHeight: 700)
                    // Difference: Target - Visible = 150 - 120 = -30
                    // Normalized: Difference /  (ContentHeight - ViewportHeight) = -30 / (900 - 700) = -0.151
                    Debug.Log($"ScrollDirection: {scrollDirection}");

                    float current = scrollDirection > 0 ? visibleContent.yMax : visibleContent.yMin;
                    float target = scrollDirection > 0 ? yMax : yMin;
                    float dif = target - current;
                    float n = dif / scrollHeight;
                    _scrollRect.verticalNormalizedPosition += n;
                }
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
            yield return _numberOfOrbs;
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

            _numberOfOrbs.SetItem(options.numberOfOrbs, true);
            _useRumble.SetItem(options.useRumble ? 1 : 0, true);
            _beamsReflectOrbs.SetItem(options.beamsReflectOrbs ? 1 : 0, true);
            _bordersKillPlayers.SetItem(options.bordersKillPlayers ? 1 : 0, true);
        }

        private void SetAllAudios()
        {
            Options options = OptionsManager.Options;
            if(options.masterVolume == 0)
            {
                _audioMixer.SetFloat("MasterVolume", -80f);
            }
            else
            {
                _audioMixer.SetFloat("MasterVolume", -50f + 5 * options.masterVolume);
            }
            if(options.musicVolume == 0)
            {
                _audioMixer.SetFloat("MusicVolume", -80f);
            }
            else
            {
                _audioMixer.SetFloat("MusicVolume", -50f + 5 * options.musicVolume);
            }
            if(options.sfxVolume == 0)
            {
                _audioMixer.SetFloat("SfxVolume", -80f);
            }
            else
            {
                _audioMixer.SetFloat("SfxVolume", -50f + 5 * options.sfxVolume);
            }
        }

        // ----------------------------------------------------------------------------------------
    }
}