using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace BSA
{
    public class PlayerMovement : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private CharacterController _controller;
        [SerializeField] private float _moveSpeed = 1f;
        [Tooltip("This value is a multiplier for the move speed. If the ability slows you, set the value to >1.")]
        [SerializeField] private float _abilityMoveSpeedMult = 0.5f;
        [SerializeField] private float _gravity = 1f;
        [SerializeField] private float _turnTime = .5f;
        [SerializeField] private SkinnedMeshRenderer _capeRenderer;
        [SerializeField] private SkinnedMeshRenderer _bodyRenderer;
        [SerializeField] private MeshRenderer _headRenderer;
        [SerializeField] private Light _light;
        [SerializeField] private Cloth _cloth;
        [SerializeField] private Transform _body;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _minTimeForAbility = 1f;
        [SerializeField] private bool _canMove = false;
        private Vector3 _moveDirection = Vector3.zero;
        private float _turnVelocity = 0.00f;
        private bool _performingAbility = false;
        private float _moveMult = 1f;


        [Header("Ability")]
        [SerializeField] private float _abilityCooldown = 5f;
        private bool _isOnCooldown = false;
        [SerializeField] private Shockwave _ability;
        private Transform _abilityTransform;
        [SerializeField] private Transform _abilitySpawnPoint;

        // --- Properties ---------------------------------------------------------------------------------------------
        // Getter property syntax
        //public CharacterController Controller1 { get { return _controller; } }
        //public CharacterController Controller2 { get => _controller; }
        //public CharacterController Controller3 => _controller;
        public bool IsReady { get; private set; } = false;
        public bool IsAlive { get; private set; } = true;
        public int MaterialId { get; set; }
        public int PositionId { get; set; } 

        public Color MainColor { get; set; }
        public Color CapeColor { get; set; }
        public Color MetalColor { get; set; }

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Awake()
        {
            _abilityTransform = _ability.transform;
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
                Vector3 movement = _moveDirection * (Time.fixedDeltaTime * _moveSpeed * _moveMult);
                movement += Vector3.down * (_gravity * Time.fixedDeltaTime);
                _controller.Move(movement);
            }

            // Rotate
            if(_moveDirection != Vector3.zero)
            {
                float angle = Vector3.Angle(transform.forward, _moveDirection);
                if(angle > 0.05f)
                {
                    float targetAngle = Vector3.SignedAngle(Vector3.forward, _moveDirection, Vector3.up);
                    float newAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnVelocity, _turnTime);
                    transform.eulerAngles = new Vector3(0f, newAngle, 0f);
                }
                else
                {
                    transform.forward = _moveDirection;
                    //_isTurning = false;
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

            //if(_moveDirection != Vector3.zero)
            //{
            //    _moveDirection.Normalize();

            //    _isTurning = true;
            //    _lookDirection = _moveDirection;
            //}
            //else
            //{
            //    _isTurning = false;
            //}
        }

        public void OnReady(InputAction.CallbackContext context)
        {
            if(GameManager.Instance.State == GameState.Preparation && context.performed)
            {
                IsReady = true;
                GameManager.Instance.CheckGameStart(this);
            }
        }
    
        public void OnLeave(InputAction.CallbackContext context)
        {
            if(context.performed == false)
            {
                return;
            }
            if(GameManager.Instance.State != GameState.Preparation)
            {
                return;
            }
            if(IsReady)
            {
                IsReady = false;
                GameManager.Instance.CheckGameStart(this);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void OnAbility(InputAction.CallbackContext context)
        {
            if(GameManager.Instance.State != GameState.Running || IsAlive == false || _isOnCooldown)
                return;

            if(context.performed)
            {
                //Debug.Log($"{name} started ability", this);
                _performingAbility = true;
                _moveMult = _abilityMoveSpeedMult;
                StartCoroutine(AbilityRoutine());
            }
            else if(context.canceled)
            {
                _moveMult = 1;
                _performingAbility = false;
            }
        }

        public void OnDeviceLost()
        {
            GameManager.Instance.DeviceLost(PositionId);
            Debug.Log("Device was lost");
        }

        public void OnDeviceRegained()
        {
            GameManager.Instance.DeviceRegained();
            Debug.Log("Device was regained");
        }

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public void Hit()
        {
            if(GameManager.Settings.InvincibleMode)
                return;

            IsAlive = false;
            _animator.SetBool("isHit", true);
            this.DoAfter(.5f, () => _animator.SetBool("isHit", false));
            //_animator.SetTrigger("gotHit");
            FloatToGround();
            GameManager.Instance.CheckGameEnd();
        }

        public void GameStart(Vector3 spawnPosition, float delay)
        {
            _cloth.worldAccelerationScale = 0f;
            transform.position = spawnPosition;
            this.DoAfter(delay, AllowMovement);
            //Invoke(nameof(AllowMovement), delay);
        }

        public void EndGame()
        {
            _turnTime = 1f;
            _canMove = false;
            _moveDirection = Vector3.back;
        }

        public void ChangeMaterial()
        {
            //Color temp = Material.GetColor("_ColorUp");
            Color newColor = MainColor;
            _ability.ChangeColor(newColor);
            _capeRenderer.material.SetColor("_Color", CapeColor);
            _capeRenderer.material.SetColor("_MetalicColor", MetalColor);
            _headRenderer.material.SetColor("_EmissionColor", newColor * 4);
            _light.color = newColor;
            newColor.a = 0.35f;
            _bodyRenderer.material.color = newColor;
        }



        // --- Protected/Private Methods ------------------------------------------------------------------------------
        private void AllowMovement()
        {
            _cloth.worldAccelerationScale = 0.2f;
            _canMove = true;
        }        

        private IEnumerator AbilityRoutine()
        {
            _animator.SetBool("AbilityCharging", true);
            _animator.SetBool("AbilityCancel", false);

            _abilityTransform.parent = transform;
            _abilityTransform.position = _abilitySpawnPoint.position;
            _abilityTransform.forward = transform.forward;
            _ability.AbilityPressed();

            float time = 0f;

            while(_performingAbility)
            {
                time += Time.deltaTime;
                yield return null;
            }

            if(time < _minTimeForAbility)
            {
                _animator.SetBool("AbilityCancel", true);
                _ability.AbilityCanceled();
            } else
            {
                _isOnCooldown = true;
                _ability.AbilityActivated();
                this.DoAfter(_abilityCooldown, () => _isOnCooldown = false);
            }
            _animator.SetBool("AbilityCharging", false);

        }

        private void FloatToGround()
        {

            Vector3 currentPos = _body.localPosition;
            float y = currentPos.y;

            this.AutoLerp(y, y - 1.2f, GameManager.Settings.PlayerDeathAnimationLength, FloatDown);

            void FloatDown(float yPos)
            {
                currentPos.y = yPos;
                _body.localPosition = currentPos;
            }

        }

        // ----------------------------------------------------------------------------------------
    }
}