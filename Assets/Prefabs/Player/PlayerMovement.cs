using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BSA
{
    public class PlayerMovement : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private CharacterController _controller;
        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] private float _gravity = 1f;
        [SerializeField] private float _turnTime = .5f;
        [SerializeField] private MeshRenderer _renderer;

        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _lookDirection = Vector3.zero;

        private bool _isTurning = false;
        private bool _canMove = false;
        private float _turnVelocity = 0.00f;

        // --- Properties ---------------------------------------------------------------------------------------------
        // Getter property syntax
        //public CharacterController Controller1 { get { return _controller; } }
        //public CharacterController Controller2 { get => _controller; }
        //public CharacterController Controller3 => _controller;
        public bool IsReady { get; private set; } = false;
        public bool IsAlive { get; private set; } = true;
        public int MaterialId { get; set; }
        public int PositionId { get; set; } 

        public Material Material { get; set; }

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Awake()
        {
            _lookDirection = transform.forward;
        }

        private void FixedUpdate()
        {
            if(!IsAlive)
            {
                return;
            }

            // Móve
            if(_canMove)
            {
                Vector3 movement = _moveDirection * (Time.fixedDeltaTime * _moveSpeed);
                movement += Vector3.down * (_gravity * Time.fixedDeltaTime);
                _controller.Move(movement);
            }

            // Rotate
            if(_isTurning)
            {
                float angle = Vector3.Angle(transform.forward, _lookDirection);
                if(angle > 0.05f)
                {
                    float targetAngle = Vector3.SignedAngle(Vector3.forward, _lookDirection, Vector3.up);
                    float newAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnVelocity, _turnTime);
                    transform.eulerAngles = new Vector3(0f, newAngle, 0f);
                }
                else
                {
                    transform.forward = _lookDirection;
                    _isTurning = false;
                }
            }
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------
        public void OnMove(InputAction.CallbackContext context)
        {
            if(_canMove == false)
                return;

            Vector2 input = context.ReadValue<Vector2>();
            _moveDirection = new Vector3(input.x, 0, input.y);

            if(_moveDirection != Vector3.zero)
            {
                _moveDirection.Normalize();

                _isTurning = true;
                _lookDirection = _moveDirection;
            }
        }

        public void OnReady(InputAction.CallbackContext context)
        {
            if(GameManager.Instance.GameRunning == false && context.performed)
            {
                IsReady = true;
                GameManager.Instance.CheckGameStart(this);
            }
        }
    
        public void OnLeave(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                if(IsReady)
                {
                    IsReady = false;
                    GameManager.Instance.CheckGameStart(this);
                } else if(GameManager.Instance.GameRunning == false)
                {
                    Destroy(this.gameObject);
                }
            }
        }

        public void OnDeviceLost()
        {
            Debug.Log("Device was lost");
        }

        public void OnDeviceRegained()
        {
            Debug.Log("Device was regained");
        }

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public void Hit()
        {
            IsAlive = false;
            GameManager.Instance.CheckGameEnd();
            // Hier soll noch dem GameManager gesagt werden dass ein Spieler getroffen wurde

        }

        public void GameStart(Vector3 spawnPosition, float delay)
        {
            transform.position = spawnPosition;
            this.DoAfter(delay, AllowMovement);
            //Invoke(nameof(AllowMovement), delay);
        }

        public void EndGame()
        {
            _turnTime = 1f;
            _canMove = false;
            _isTurning = true;
            _lookDirection = new Vector3(0, 0, -1);
        }

        public void ChangeMaterial()
        {
            _renderer.material = Material;
        }

        // --- Protected/Private Methods ------------------------------------------------------------------------------
        private void AllowMovement()
        {
            _canMove = true;
        }        

        // ----------------------------------------------------------------------------------------
    }
}