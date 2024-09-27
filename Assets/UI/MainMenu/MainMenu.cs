using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BSA.UI
{
    public class MainMenu : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private TransitionHandler _transitionManager;
        [SerializeField] private OptionsMenu _optionsMenu;
        [Space]
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;
        [Space]
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _startAudio;
        [SerializeField] private AudioClip _SettingAudio;


        // --- Properties ---------------------------------------------------------------------------------------------

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Start()
        {
            EventSystem.current.SetSelectedGameObject(_startButton.gameObject);
        }

        private void Awake()
        {
            _optionsMenu.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _startButton.onClick.AddListener(OnStartClick);
            _settingsButton.onClick.AddListener(OnSettingsClicked);
            _exitButton.onClick.AddListener(OnExitClicked);
            
            _optionsMenu.Closed += CloseedOptionsMenu;
        }


        private void OnDisable()
        {
            _startButton.onClick.RemoveListener(OnStartClick);
            _settingsButton.onClick.RemoveListener(OnSettingsClicked);
            _exitButton.onClick.RemoveListener(OnExitClicked);
            
            _optionsMenu.Closed -= CloseedOptionsMenu;
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------
        private  void OnStartClick()
        {
            _audioSource.clip = _startAudio;
            _audioSource.Play();
            _transitionManager.SwtichFromScene(.5f);
            this.DoAfter(1f, LoadGame);
        }

        private void OnSettingsClicked()
        {
            _audioSource.clip = _SettingAudio;
            _audioSource.Play();
            _optionsMenu.gameObject.SetActive(true);
        }

        private void OnExitClicked()
        {
#if UNITY_EDITOR
            //if(Application.isEditor)
            //{
            //    EditorApplication.isPlaying = false;
            //}
            //else
            //{
            //    Application.Quit();
            //}
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
        private void CloseedOptionsMenu()
        {
            EventSystem.current.SetSelectedGameObject(_settingsButton.gameObject);
        }

        // --- Public/Internal Methods --------------------------------------------------------------------------------

        // --- Protected/Private Methods ------------------------------------------------------------------------------
        private void LoadGame()
        {
            SceneManager.LoadScene(1);
        }
        // ----------------------------------------------------------------------------------------
    }
}