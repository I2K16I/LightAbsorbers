using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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
        [SerializeField] private LayerMask _layerMask;

        [Header("Camera and Transitions")]
        [SerializeField] private CinemachineVirtualCamera _joinCamera;
        [SerializeField] private CinemachineVirtualCamera _gameCamera;
        [SerializeField] private CinemachineVirtualCamera _winningCamera;
        [SerializeField] private Vector3 _winningCameraOffset;
        [Tooltip("Place the Countdown Slider here, it's used to show the player how long everyone has to be ready before the round starts")]
        [SerializeField] private Slider _countdownBar;

        [Header("Managers & Spawnpoints")]
        [SerializeField] private PlayerInputManager _playerInputManager;
        [SerializeField] private OrbManager _orbManager;
        [SerializeField] private BannerManager _bannerManager;
        [SerializeField] private TransitionHandler _UiManager;
        [SerializeField] private Transform[] _spawnPointsGame;

        private int _startAmountOfAttacks = 1;
        private readonly List<PlayerMovement> _players = new();
        private Coroutine _startRoutineInsttance = null;
        private bool _gameEnded = false;
        private SceneManager _sceneManager;

        // --- Properties ---------------------------------------------------------------------------------------------
        public static GameManager Instance { get; private set; }

        public static Settings Settings => Instance._settings;
        public int PlayerCount { get { return _players.Count; } }

        public bool GameRunning { get; private set; } = false;

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

            _playerInputManager.onPlayerJoined += OnPlayerJoined;
            _playerInputManager.onPlayerLeft += OnPlayerLeft;
            _startAmountOfAttacks = _settings.StartAmountOfAttacks;
            UpdateCameras();
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
            movement.PositionId = index;

            _bannerManager.PlayerJoined(movement);
            _players.Add(movement);
        }

        private void OnPlayerLeft(UnityEngine.InputSystem.PlayerInput player)
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            int index = _players.IndexOf(movement);
            _players.Remove(movement);
            //Destroy(movement.gameObject);

            if(GameRunning)
            {
                CheckGameEnd();
            }
            else
            {
                _bannerManager.PlayerLeft(movement);
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

            if(_gameEnded)
            {

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }

            _bannerManager.ColorBanner(player);

            if(_players.All(p => p.IsReady) && _players.Count(p => p.IsReady) > 1)
            {
                _countdownBar.gameObject.SetActive(true);
                _countdownBar.value = 0.0f;
                StartCoroutine(FillProgressBarRoutine());
                _startRoutineInsttance = StartCoroutine(StartGameCoroutine());
            }
            else
            {
                _countdownBar.gameObject.SetActive(false);
                if(_startRoutineInsttance != null)
                {
                    StopCoroutine(_startRoutineInsttance);
                    _startRoutineInsttance = null;
                }
            }
        }

        public void CheckGameEnd()
        {
            // If only one player is still alive, end the game
            var alivePlayers = _players.Where(p => p.IsAlive);
            if(alivePlayers.Count() == 1)
            {
                PlayerMovement winner = alivePlayers.First();

                GameRunning = false;
                _orbManager.EndGame();
                _winningCamera.gameObject.transform.position = winner.transform.position + _winningCameraOffset;
                _winningCamera.Priority = 11;
                _joinCamera.Priority = 0;
                _gameCamera.Priority = 0;
                winner.EndGame();
                _gameEnded = true;
            }

            // Show victory screen or whatever
        }

        public void DeviceLost(int playerNumber)
        {
            Time.timeScale = 0.0f;
            _UiManager.ShowDeviceLostScreen(playerNumber);
        }

        public void DeviceRegained()
        {
            Time.timeScale = 1.0f;
            _UiManager.HideDeviceLostScreen();
        }

        // --- Protected/Private Methods ------------------------------------------------------------------------------

        private IEnumerator StartGameCoroutine()
        {
            yield return new WaitForSeconds(_settings.StartDelay);
            GameRunning = true;
            _playerInputManager.DisableJoining();
            float transitionTime = _settings.TransitionTime;

            _UiManager.MoveFromJoinToGameScreen(transitionTime);
            yield return new WaitForSeconds(transitionTime / 2);

            _countdownBar.gameObject.SetActive(false);
            UpdateCameras();

            float timeBetweenTransitonAndGameStart = _settings.TimeBetweenTransitionAndStart;

            for(int i = 0; i < _players.Count; i++)
            {
                _players[i].GameStart(_spawnPointsGame[i].position, timeBetweenTransitonAndGameStart);
            }

            yield return new WaitForSeconds(transitionTime / 2);

            this.DoAfter(_settings.TimeUntilFirstIncrease, IncreaseNumberOfAttacks);
            this.DoAfter(_settings.TimeUntilFirstIncrease + _settings.TimeUntilSecondIncrease, IncreaseNumberOfAttacks);
            this.DoAfter(timeBetweenTransitonAndGameStart, _orbManager.StartOrbs);
            float combinedTime = timeBetweenTransitonAndGameStart + _settings.TimeBetweenAttacks;
            this.DoAfter(combinedTime, () => StartCoroutine(StartRecurringOrbAttacks()));
        }

        private void UpdateCameras()
        {
            if(GameRunning)
            {
                _gameCamera.Priority = 10;
                _joinCamera.Priority = 0;
            }
            else
            {
                _gameCamera.Priority = 0;
                _joinCamera.Priority = 10;
            }
        }

        private IEnumerator StartRecurringOrbAttacks()
        {
            while(GameRunning)
            {
                for(int i = 0; i < _startAmountOfAttacks; i++)
                {
                    _orbManager.OrbAttack(_settings.TotalBeamAttackDuration);
                    yield return new WaitForSeconds(1f);
                }

                yield return new WaitForSeconds(_settings.TimeBetweenAttacks);
            }
        }

        private IEnumerator FillProgressBarRoutine()
        {
            float timeSpentFillingBar = 0f;
            float startDelay = _settings.StartDelay;

            while(timeSpentFillingBar < startDelay)
            {
                timeSpentFillingBar += Time.deltaTime;
                _countdownBar.value = Mathf.Lerp(0, 1, timeSpentFillingBar / startDelay);
                yield return null;
            }
            _countdownBar.value = 1;
        }

        private void IncreaseNumberOfAttacks()
        {
            _startAmountOfAttacks++;
        }

        // ----------------------------------------------------------------------------------------
    }
}