using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace BSA
{
    public class GameManager : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private CinemachineVirtualCamera _winningCamera;
        [SerializeField] private Vector3 _winningCameraOffset;
        [SerializeField] private float _timeBetweenReadyAndStart = 2f;
        [SerializeField] private PlayerInputManager _playerInputManager;
        [SerializeField] private OrbSpawner _orbSpawner;
        [SerializeField] private CinemachineVirtualCamera _joinCamera;
        [SerializeField] private Transform[] _spawnPointsJoin;
        [SerializeField] private CinemachineVirtualCamera _gameCamera;
        [SerializeField] private Transform[] _spawnPointsGame;

        private readonly List<PlayerMovement> _players = new();
        
        // --- Properties ---------------------------------------------------------------------------------------------
        public static GameManager Instance { get; private set; }

        public bool GameStarted { get; private set; } = false;

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

            movement.transform.position = _spawnPointsJoin[index].position;
            _players.Add(movement);
        }

        private void OnPlayerLeft(UnityEngine.InputSystem.PlayerInput player)
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            int index = _players.IndexOf(movement);
            _players.Remove(movement);
            //Destroy(movement.gameObject);

            if(GameStarted)
            {
                CheckGameEnd();
            }
            else
            {
                // Make players right of the player that exited move to the left
                for(int i = index; i < _players.Count; i++)
                {
                    _players[i].transform.position = _spawnPointsJoin[i].position;
                }
            }
        }

        // --- Public/Internal Methods --------------------------------------------------------------------------------

        public void CheckGameStart()
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
            if(_players.All(p => p.IsReady) && _players.Count(p => p.IsReady) > 1)
            {
                StartGame();
            }

            // If so, start game / countdown
        }

        public void CheckGameEnd()
        {
            // If only one player is still alive, end the game
            PlayerMovement winner = _players.SingleOrDefault(p => p.IsAlive);
            if(winner != null)
            {
                _orbSpawner.EndGame();
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
            GameStarted = true;
            _playerInputManager.DisableJoining();

            // Activate the game Camera and disable the join Screen Camera
            UpdateCameras();

            for(int i = 0; i < _players.Count; i++)
            {
                _players[i].GameStart(_spawnPointsGame[i].position, _timeBetweenReadyAndStart);
            }

            this.DoAfter(_timeBetweenReadyAndStart, _orbSpawner.StartOrbs);
            // Start Game (enable movement for players and orbs)
        }

        private void UpdateCameras()
        {
            if(GameStarted)
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

        // ----------------------------------------------------------------------------------------
    }
}