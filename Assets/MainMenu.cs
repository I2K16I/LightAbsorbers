using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BSA
{
    public class MainMenu : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private TransitionHandler _transitionManager;
        [Space]
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;

        // --- Properties ---------------------------------------------------------------------------------------------

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void OnEnable()
        {
            _startButton.onClick.AddListener(OnStartClick);
            _settingsButton.onClick.AddListener(OnSettingsClicked);
            _exitButton.onClick.AddListener(OnExitClicked);
        }

        private void OnDisable()
        {
            _startButton.onClick.RemoveListener(OnStartClick);
            _settingsButton.onClick.RemoveListener(OnSettingsClicked);
            _exitButton.onClick.RemoveListener(OnExitClicked);
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------
        private  void OnStartClick()
        {
            _transitionManager.SwtichFromScene(1f);
            this.DoAfter(1f, LoadGame);
        }

        private void OnSettingsClicked()
        {
            Debug.Log("Settings, yay");
        }

        public void OnBackClicked()
        {
            Debug.Log("Back to the main Menu");
        }

        public void OnSaveClicked()
        {
            Debug.Log("Saved Settings");
        }

        private void OnExitClicked()
        {
            if(Application.isEditor)
            {
                EditorApplication.isPlaying = false;
            }
            else
            {
                Application.Quit();
            }
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