using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BSA
{
    public class GameManager : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [Tooltip("The attack duration one Second of Buildup before the attack starts to deal dmg.")]
        [SerializeField] private float _attackDuration = 6f;
        [Tooltip("This value should be at least double of the attack duration. Lowest possible value shoule be the same as the attack duration.")]
        [SerializeField] private float _timeBetweenAttacks = 12f;
        [Tooltip("This value determins how fast the game starts after all Players are ready")]
        [SerializeField] private float _startDelay = 1.5f;
        [SerializeField] private float _transitionTime = 1f;
        [SerializeField] private TransitionManager _transitionManager;
        [SerializeField] private CinemachineVirtualCamera _winningCamera;
        [SerializeField] private Vector3 _winningCameraOffset;
        [SerializeField] private float _timeBetweenReadyAndStart = 2f;
        [SerializeField] private PlayerInputManager _playerInputManager;
        [SerializeField] private OrbManager _orbManager;
        [SerializeField] private CinemachineVirtualCamera _joinCamera;
        [SerializeField] private Transform[] _spawnPointsJoin;
        [SerializeField] private CinemachineVirtualCamera _gameCamera;
        [SerializeField] private Transform[] _spawnPointsGame;
        [SerializeField] private BannerManager _bannerManager;
        [SerializeField] private StatusSpriteManager[] _statusSpriteManager;
        [SerializeField] private Slider _countdownBar;

        [SerializeField] private float _timeUntilFirstIcreaseOfAttacks = 20f;
        [SerializeField] private float _timeUntilSecondIcreaseOfAttacks = 40f;
        [SerializeField] private int _amountOfAttacks = 1;
        private readonly List<PlayerMovement> _players = new();
        private Coroutine _startRoutineInsttance = null;
        
        // --- Properties ---------------------------------------------------------------------------------------------
        public static GameManager Instance { get; private set; }
        public int PlayerCount { get { return _players.Count; } }

        public bool GameRunning { get; private set; } = false;

        // --- Events -------------------------------------------------------------------------------------------------
        UnityEvent _playerCountChanged = new();

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
            _bannerManager.DeterminColor(movement, index);
            movement.transform.position = _spawnPointsJoin[index].position;
            _players.Add(movement);
            _statusSpriteManager[index].UpdateReadyStatus(movement);
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
                _bannerManager.ReleaseMaterialLock(movement, index, _players);
                // Make players right of the player that exited move to the left
                for(int i = index; i < _spawnPointsJoin.Length; i++)
                {
                    if(i<_players.Count)
                    {
                        _players[i].transform.position = _spawnPointsJoin[i].position;
                        _statusSpriteManager[i].UpdateReadyStatus(_players[i]);
                    } else
                    {
                        _statusSpriteManager[i].SetStatusPlayerLeft();
                    }
                }
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
            _statusSpriteManager[player.PositionId].UpdateReadyStatus(player);

            if(_players.All(p => p.IsReady) && _players.Count(p => p.IsReady) > 1)
            {
                _countdownBar.gameObject.SetActive(true);
                _countdownBar.value = 0.0f;
                StartCoroutine(FillProgressBarRoutine());
                _startRoutineInsttance = StartCoroutine(StartGameCoroutine());
                //this.DoAfter(_startDelay, StartGame);
                //StartGame();
            } else
            {
                _countdownBar.gameObject.SetActive(false);
                if(_startRoutineInsttance != null)
                {
                    StopCoroutine(_startRoutineInsttance);
                    _startRoutineInsttance = null;
                }
            }

            // If so, start game / countdown
        }

        public void CheckGameEnd()
        {
            // If only one player is still alive, end the game
            PlayerMovement winner = _players.SingleOrDefault(p => p.IsAlive);
            if(winner != null)
            {
                GameRunning = false;
                _orbManager.EndGame();
                _winningCamera.gameObject.transform.position = winner.transform.position + _winningCameraOffset;
                _winningCamera.Priority = 11;
                _joinCamera.Priority = 0;
                _gameCamera.Priority = 0;
                winner.EndGame();
            }

            // Show victory screen or whatever
        }

        // --- Protected/Private Methods ------------------------------------------------------------------------------
        //private void UpdateGameStatus()
        //{
        //    if(_players.Count == 1 && GameStarted)
        //    {
        //        PlayerMovement winner = null;

        //        for(int i = 0; i < _players.Count; i++)
        //        {
        //            if(_players[i] != null)
        //            {
        //                winner = _players[i];
        //            }
        //        }

        //        _orbSpawner.EndGame();
        //        winner.EndGame();
        //        // Gewinnen des letzten Spielers
        //    }
        //}

        private void StartGame()
        {
                GameRunning = true;
                _playerInputManager.DisableJoining();

                // Activate the game Camera and disable the join Screen Camera
                UpdateCameras();

                for(int i = 0; i < _players.Count; i++)
                {
                    _players[i].GameStart(_spawnPointsGame[i].position, _timeBetweenReadyAndStart);
                }

                this.DoAfter(_timeBetweenReadyAndStart, _orbManager.StartOrbs);
                float combinedTime = _timeBetweenReadyAndStart + _timeBetweenAttacks;
                this.DoAfter(combinedTime, () => StartCoroutine(StartRecurringOrbAttacks()));
        }

        private IEnumerator StartGameCoroutine()
        {
            yield return new WaitForSeconds(_startDelay);
            GameRunning = true;
            _playerInputManager.DisableJoining();

            _transitionManager.MoveFromJoinToGameScreen(_transitionTime);
            yield return new WaitForSeconds(_transitionTime/2);

            UpdateCameras();


            for(int i = 0; i < _players.Count; i++)
            {
                _players[i].GameStart(_spawnPointsGame[i].position, _timeBetweenReadyAndStart);
            }
            
            yield return new WaitForSeconds(_transitionTime/2);

            this.DoAfter(_timeUntilFirstIcreaseOfAttacks, IncreaseNumberOfAttacks);
            this.DoAfter(_timeUntilSecondIcreaseOfAttacks, IncreaseNumberOfAttacks);
            this.DoAfter(_timeBetweenReadyAndStart, _orbManager.StartOrbs);
            float combinedTime = _timeBetweenReadyAndStart + _timeBetweenAttacks;
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
                for(int i = 0; i < _amountOfAttacks; i++){
                    _orbManager.OrbAttack(_attackDuration);
                }
                yield return new WaitForSeconds(_timeBetweenAttacks);

            }
        }

        private IEnumerator FillProgressBarRoutine()
        {
            float timeSpentFillingBar = 0f;

            while(timeSpentFillingBar < _startDelay)
            {
                timeSpentFillingBar += Time.deltaTime;
                _countdownBar.value = Mathf.Lerp(0, 1, timeSpentFillingBar / _startDelay);
                yield return null;
            }
            _countdownBar.value = 1;
        }

        private void IncreaseNumberOfAttacks()
        {
            _amountOfAttacks++;
        }

        // ----------------------------------------------------------------------------------------
    }
}