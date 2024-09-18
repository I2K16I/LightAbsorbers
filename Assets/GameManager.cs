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

        private int _consecutiveOrbAttacks = 1;
        private readonly List<PlayerMovement> _players = new();
        private Coroutine _startRoutineInstance = null;
        private Coroutine _increaseAttacksRoutine = null;
        private SceneManager _sceneManager;

        [Header("Debugging")]
        [SerializeField] private bool _isInTestScene = false;
        [SerializeField] private bool _canStartSolo = false;


        // --- Properties ---------------------------------------------------------------------------------------------
        public static GameManager Instance { get; private set; }

        public static Settings Settings => Instance._settings;
        public int PlayerCount { get { return _players.Count; } }

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

            State = GameState.Preparation;

            _playerInputManager.onPlayerJoined += OnPlayerJoined;
            _playerInputManager.onPlayerLeft += OnPlayerLeft;
            _consecutiveOrbAttacks = _settings.StartAmountOfAttacks;
            if(_isInTestScene == false)
            {
                UpdateCameras();
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
            movement.PositionId = index;
            player.name = $"Player_{index:00}";

            _bannerManager.PlayerJoined(movement);
            _players.Add(movement);
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

            if(State == GameState.Finished)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }

            _bannerManager.ColorBanner(player);

            if(_players.All(p => p.IsReady))
            {
                if(_players.Count(p => p.IsReady) > 1 || _canStartSolo)
                {
                    _countdownBar.gameObject.SetActive(true);
                    this.AutoLerp(0f, 1f, _settings.StartDelay, fill => _countdownBar.value = fill, EasingType.Smoother);
                    _startRoutineInstance = StartCoroutine(StartGameCoroutine());
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
                PlayerMovement winner = alivePlayers.First();

                if(_increaseAttacksRoutine != null)
                {
                    StopCoroutine(_increaseAttacksRoutine);
                }

                State = GameState.Finished;
                _orbManager.EndGame();
                _winningCamera.transform.position = winner.transform.position + _winningCameraOffset;
                UpdateCameras();
                winner.EndGame();
                // Show victory screen or whatever
            }
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
            State = GameState.Running;
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

            yield return new WaitForSeconds(timeBetweenTransitonAndGameStart);
            _orbManager.StartOrbs();

            _increaseAttacksRoutine = StartCoroutine(IncreaseAttacksRoutine());
            StartCoroutine(StartRecurringOrbAttacks());
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
            _joinCamera.Priority = 0;
            _gameCamera.Priority = 0;
            _winningCamera.Priority = 0;

            switch(State)
            {
                case GameState.Preparation:
                    _joinCamera.Priority = 10;
                    break;
                case GameState.Running:
                    _gameCamera.Priority = 10;
                    break;
                case GameState.Finished:
                    _winningCamera.Priority = 10;
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
                    _orbManager.OrbAttack(_settings.TotalBeamAttackDuration);
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        // ----------------------------------------------------------------------------------------
    }
}