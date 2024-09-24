using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BSA
{
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------

        // Header, Tooltip, Space

        [Header("Settings")]
        [SerializeField] private Settings _settings;        

        [Header("Camera and Transitions")]
        [SerializeField] private Vector3 _winningCameraOffset;
        [Tooltip("Place the Countdown Slider here, it's used to show the player how long everyone has to be ready before the round starts")]
        [SerializeField] private Slider _countdownBar;

        [Header("Managers & Spawnpoints")]
        [SerializeField] private PlayerInputManager _playerInputManager;
        [SerializeField] private BannerManager _bannerManager;

        private Transform[] _spawnPointsGame = new Transform[4];
        private int _consecutiveOrbAttacks = 1;
        private readonly List<PlayerMovement> _players = new();
        private Coroutine _startRoutineInstance = null;
        private Coroutine _increaseAttacksRoutine = null;
        private Coroutine _attackRoutine = null;

        [Header("Debugging")]
        [SerializeField] private bool _canStartSolo = false;
        private bool _canRestart;

        private Options _options;

        // --- Properties ---------------------------------------------------------------------------------------------
        public static GameManager Instance { get; private set; }

        public static Settings Settings => Instance._settings;
        public static Options Options => Instance._options;
        public int PlayerCount { get { return _players.Count; } }
        public CinemachineVirtualCamera Camera { get; set; }
        public CinemachineVirtualCamera WinningCamera { get; set; }
        public OrbManager OrbManager { get; set; }
        public TransitionHandler TransitionHandler { get; set; }
        public GameState State { get; private set; } = GameState.None;

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
            State = GameState.Preparation;

            LoadOptions();

            _playerInputManager.onPlayerJoined += OnPlayerJoined;
            _playerInputManager.onPlayerLeft += OnPlayerLeft;
            _consecutiveOrbAttacks = _settings.StartAmountOfAttacks;

            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            if(CameraCollector.TryGetCamera(CameraType.Join, out CameraCollector mainCam)
                || CameraCollector.TryGetCamera(CameraType.Game, out mainCam))
            {
                Camera = mainCam.Camera;
            }

            if(CameraCollector.TryGetCamera(CameraType.Win, out CameraCollector winCam))
            {
                WinningCamera = winCam.Camera;
            }

            string newSceneName = SceneManager.GetActiveScene().name;

            if(newSceneName.Equals("ShockAbosorber"))
            {
                State = GameState.Running;
            }
            
            
        }

        private void OnDestroy()
        {
            _playerInputManager.onPlayerJoined -= OnPlayerJoined;
            _playerInputManager.onPlayerLeft -= OnPlayerLeft;
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------
        private void OnPlayerJoined(UnityEngine.InputSystem.PlayerInput player)
        {
            int index = _players.Count;
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            //Debug.Log(string.Join("\n", Gamepad.all));
            //Debug.Log(Gamepad.current);


            movement.PositionId = index;
            player.name = $"Player_{index:00}";
            //DontDestroyOnLoad(movement.gameObject);

            _bannerManager.PlayerJoined(movement);
            _players.Add(movement);

            if(player.devices[0] is Gamepad g)
            {
                movement.MyGamepad = g;
                g.SetRumbleForDuration(Rumble.Light, .1f);

                if(g is DualShockGamepad playstationController)
                {
                    this.DoAfter(.5f, () => playstationController.SetLightBarColor(movement.MainColor));
                    //playstationController.SetLightBarColor(movement.MainColor);
                }
            }
        }


        private void OnPlayerLeft(UnityEngine.InputSystem.PlayerInput player)
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            int index = _players.IndexOf(movement);
            _players.Remove(movement);
            //Destroy(movement.gameObject);

            switch(State)
            {
                case GameState.Preparation:
                    _bannerManager.PlayerLeft(movement);
                    break;

                case GameState.Running:
                    CheckGameEnd();
                    break;

                case GameState.Finished:
                    break;
            }
        }

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public void CheckGameStart(PlayerMovement player)
        {
            // Check if all players are ready 

            // Version A
            //bool allReady = true;
            //foreach (PlayerMovement player in _players)
            //{
            //    if (!player.IsReady) 
            //    {
            //        allReady = false;
            //    }
            //}

            //if(allReady)
            //{
            //    StartGame();
            //}

            // Version B
            //bool AreAllPlayersReady()
            //{
            //    foreach(PlayerMovement player in _players)
            //    {
            //        if(!player.IsReady)
            //        {
            //            return false;
            //        }
            //    }

            //    return true;
            //}

            //if(AreAllPlayersReady())
            //{
            //    StartGame();
            //}

            // Version C
            //foreach(PlayerMovement player in _players)
            //{
            //    if(!player.IsReady)
            //        return;
            //}

            //StartGame();


            // Version D
            //if(_players.All(IsPlayerReady))
            //{
            //    StartGame();
            //}

            //bool IsPlayerReady(PlayerMovement player)
            //{
            //    return player.IsReady;
            //}

            // Version E

            _bannerManager.ColorBanner(player);

            if(_players.All(p => p.IsReady))
            {
                if(_players.Count(p => p.IsReady) > 1 || _canStartSolo)
                {
                    _countdownBar.gameObject.SetActive(true);
                    this.AutoLerp(0f, 1f, _settings.StartDelay, fill => _countdownBar.value = fill, EasingType.Smoother);
                    _startRoutineInstance = StartCoroutine(MoveToGameScene());
                }
            }
            else
            {
                _countdownBar.gameObject.SetActive(false);
                if(_startRoutineInstance != null)
                {
                    StopCoroutine(_startRoutineInstance);
                    _startRoutineInstance = null;
                }
            }
        }

        public void CheckGameEnd()
        {
            // If only one player is still alive, end the game
            var alivePlayers = _players.Where(p => p.IsAlive);
            if(alivePlayers.Count() == 1)
            {
                State = GameState.Finished;
                PlayerMovement winner = alivePlayers.First();

                if(_increaseAttacksRoutine != null)
                {
                    StopCoroutine(_increaseAttacksRoutine);
                    _increaseAttacksRoutine = null;
                }
                if(_attackRoutine != null)
                {
                    StopCoroutine(_attackRoutine);
                    _attackRoutine = null;
                }

                OrbManager.EndGame();
                winner.DisalowMovement();
                this.DoAfter(_settings.ShowWinnerDelay, () => ShowWinner(winner));
                //WinningCamera.transform.position = winner.transform.position + _winningCameraOffset;
                //UpdateCameras();
                //winner.EndGame();
                // Show victory screen or whatever
            }
        }

        public void AddSpawnPoint(int index, Transform newSpawnpoint)
        {
            _spawnPointsGame[index] = newSpawnpoint;
            if(_spawnPointsGame.Where(p => p != null).ToList().Count == _spawnPointsGame.Length)
            {
                StartCoroutine(StartGame());
            }
        }


        public void DeviceLost(int playerNumber)
        {
            Time.timeScale = 0.0f;
            TransitionHandler.ShowDeviceLostScreen(playerNumber);
        }

        public void DeviceRegained()
        {
            Time.timeScale = 1.0f;
            TransitionHandler.HideDeviceLostScreen();
        }

        public void TryRestartGame()
        {
            if(_canRestart == false)
                return;
            _canRestart = false;
            for(int i = 0; i < _spawnPointsGame.Length; i++)
            {
                _spawnPointsGame[i] = null;
            }
            foreach(PlayerMovement player in _players) { player.ResetStatus(); }
            _consecutiveOrbAttacks = _settings.StartAmountOfAttacks;
            float transitionTime = _settings.TransitionTime / 2;
            TransitionHandler.SwtichFromScene(transitionTime / 2);
            this.DoAfter(transitionTime, () => SceneManager.LoadScene(2));
        }

        public void ReturnToMain()
        {
            SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());
            _players.ForEach(p => { p.SetToActiveScene(); });
            State = GameState.None;
            float transitionTime = _settings.TransitionTime / 2;
            TransitionHandler.SwtichFromScene(transitionTime / 2);
            this.DoAfter(transitionTime, () => SceneManager.LoadScene(0));
        }

        // --- Protected/Private Methods ------------------------------------------------------------------------------
        private void ShowWinner(PlayerMovement winner)
        {
            WinningCamera.transform.position = winner.transform.position + _winningCameraOffset;
            UpdateCameras();
            winner.EndGame();
            this.DoAfter(2.5f, () => _canRestart = true);
        }
        private IEnumerator MoveToGameScene()
        {
            yield return new WaitForSeconds(_settings.StartDelay);
            State = GameState.Running;
            _playerInputManager.DisableJoining();
            float transitionTime = _settings.TransitionTime;

            TransitionHandler.SwtichFromScene(transitionTime / 2);
            yield return new WaitForSeconds(transitionTime / 2);

            _countdownBar.gameObject.SetActive(false);
            SceneManager.LoadScene(2);
            //UpdateCameras();
            //_bannerManager.DisableCloths();
        }

        private IEnumerator StartGame()
        {
            float timeBetweenTransitonAndGameStart = _settings.TimeBetweenTransitionAndStart;
            float transitionTime = _settings.TransitionTime;

            for(int i = 0; i < _players.Count; i++)
            {
                _players[i].GameStart(_spawnPointsGame[i].position, timeBetweenTransitonAndGameStart);
            }

            yield return new WaitForSeconds(transitionTime / 2);

            yield return new WaitForSeconds(timeBetweenTransitonAndGameStart);
            OrbManager.StartOrbs();
            _increaseAttacksRoutine = StartCoroutine(IncreaseAttacksRoutine());
            _attackRoutine = StartCoroutine(StartRecurringOrbAttacks());
        }

        private IEnumerator IncreaseAttacksRoutine()
        {
            yield return new WaitForSeconds(_settings.TimeUntilFirstIncrease);
            _consecutiveOrbAttacks++;
            yield return new WaitForSeconds(_settings.TimeUntilSecondIncrease);
            _consecutiveOrbAttacks++;
            _increaseAttacksRoutine = null;
        }

        private void UpdateCameras()
        {
            Camera.Priority = 0;
            if(WinningCamera != null)
            {
                WinningCamera.Priority = 0;
            }

            switch(State)
            {
                case GameState.Running:
                    Camera.Priority = 10;
                    break;
                case GameState.Finished:
                    if(WinningCamera != null)
                    {
                        WinningCamera.Priority = 10;
                    }
                    break;
            }
        }

        private IEnumerator StartRecurringOrbAttacks()
        {
            while(State == GameState.Running)
            {
                yield return new WaitForSeconds(_settings.TimeBetweenAttacks);

                for(int i = 0; i < _consecutiveOrbAttacks; i++)
                {
                    OrbManager.OrbAttack(_settings.TotalBeamAttackDuration);
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        // ----------------------------------------------------------------------------------------

        #region Options
        public void LoadOptions()
        {
            string optionsPath = GetPath();
            if (File.Exists(optionsPath))
            {
                try
                {
                    string json = File.ReadAllText(optionsPath);
                    _options = JsonUtility.FromJson<Options>(json);
                }
                catch(System.Exception e)
                {
                    _options = new Options();
                    Debug.LogError($"Failed to load Options. Creating new." +
                        $"\n{e}");
                }
            }
            else
            {
                _options = new Options();
            }
        }

        public void SaveOptions()
        {
            if(_options == null)
                throw new UnassignedReferenceException("Options is NULL.");

            try
            {
                string json = JsonUtility.ToJson(_options);
                File.WriteAllText(GetPath(), json);
            }
            catch(System.Exception e)
            {
                Debug.LogError($"Failed to write Options." +
                    $"\n{e}");
            }
        }

        private string GetPath()
        {
            return Path.Combine(Application.persistentDataPath, "options.json");
        }
        #endregion

        // ----------------------------------------------------------------------------------------
    }
}