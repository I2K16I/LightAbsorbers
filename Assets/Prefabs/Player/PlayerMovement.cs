using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BSA
{
    public class PlayerMovement : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private CharacterController _controller;
        [SerializeField] private float _moveSpeed = 1f;
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
        private float _defaultTurnTime = 0.1f;
        private Vector3 _defaultLocalBodyPos;
        private Vector3 _spawnPosition = Vector3.zero;
        private Coroutine _floatToGroundRoutine = null;

        [Header("Ability")]
        [Tooltip("This value is a multiplier for the move speed. If the ability slows you, set the value to >1.")]
        [SerializeField] private float _abilityMoveSpeedMult = 0.5f;
        [SerializeField] private float _abilityCooldown = 5f;
        private bool _isOnCooldown = false;
        [SerializeField] private Shockwave _ability;
        private Transform _abilityTransform;
        [SerializeField] private Transform _abilitySpawnPoint;
        [SerializeField] private Image _abilityIndicator;


        // --- Properties ---------------------------------------------------------------------------------------------
        // Getter property syntax
        //public CharacterController Controller1 { get { return _controller; } }
        //public CharacterController Controller2 { get => _controller; }
        //public CharacterController Controller3 => _controller;
        public bool IsReady { get; private set; } = false;
        public bool IsAlive { get; private set; } = true;
        public int MaterialId { get; set; }
        public Gamepad MyGamepad { get; set; }
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

        private void Start()
        {
            if(MyGamepad is DualShockGamepad playstationController)
            {
                //Debug.Log(playstationController.displayName);
                playstationController.SetLightBarColor(MainColor);
            }            
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

        private void OnDestroy()
        {
            if(MyGamepad != null)
            {
                MyGamepad.ResetHaptics();
                if(MyGamepad is DualShockGamepad playstationController)
                {
                    playstationController.SetLightBarColor(Color.black);
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
            if(!context.performed)
                return;

            if((GameManager.Instance.State == GameState.Preparation))
            {
                if(MyGamepad != null && IsReady == false)
                {
                    //Debug.Log($"{name} is ready", this);
                    MyGamepad.SetRumbleForDuration(Rumble.Light, .1f);
                }
                IsReady = true;
                GameManager.Instance.CheckGameStart(this);
            }
            else if(GameManager.Instance.State == GameState.Finished)
            {
                GameManager.Instance.TryRestartGame();
            }

        }

        public void OnLeave(InputAction.CallbackContext context)
        {
            if(context.performed == false)
                return;

            if(GameManager.Instance.State == GameState.Preparation)
            {
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
            else if(GameManager.Instance.State == GameState.Finished)
            {
                GameManager.Instance.ReturnToMain();
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

                if(MyGamepad == null)
                    return;
            }
            else if(context.canceled)
            {
                _moveMult = 1;
                _performingAbility = false;

                if(MyGamepad == null)
                    return;
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

            if(MyGamepad == null)
                return;
            if(MyGamepad is DualShockGamepad playstationController)
            {
                playstationController.SetLightBarColor(MainColor);
            }
        }

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public void Hit()
        {
            if(GameManager.Settings.InvincibleMode || GameManager.Instance.State != GameState.Running)
                return;

            if(MyGamepad != null)
            {
                MyGamepad.SetRumbleForDuration(Rumble.Strong, .4f);
            }

            IsAlive = false;
            _animator.SetBool("isHit", true);
            this.DoAfter(.5f, () => _animator.SetBool("isHit", false));
            //_animator.SetTrigger("gotHit");
            StartCoroutine(FloatToGroundRoutine());
            GameManager.Instance.CheckGameEnd();
        }

        public void GameStart(Vector3 spawnPosition, float delay)
        {
            _cloth.worldAccelerationScale = 0f;
            transform.position = spawnPosition;
            _spawnPosition = spawnPosition;
            _defaultLocalBodyPos = _body.localPosition;
            this.DoAfter(delay, AllowMovement);
            //Invoke(nameof(AllowMovement), delay);
        }

        public void EndGame()
        {
            _performingAbility = false;
            _defaultTurnTime = _turnTime;
            _turnTime = 1f;
            _canMove = false;
            _moveDirection = Vector3.back;

            if(MyGamepad == null)
                return;

            MyGamepad.ResetRumble();
        }

        public void SetToActiveScene()
        {
            _ability.transform.parent = this.transform;
            SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());
        }

        public void ChangeMaterial()
        {
            Color newColor = MainColor;
            _ability.ChangeColor(newColor);
            _capeRenderer.material.SetColor("_Color", CapeColor);
            _capeRenderer.material.SetColor("_MetalicColor", MetalColor);
            _headRenderer.material.SetColor("_EmissionColor", newColor * 4);
            _light.color = newColor;
            newColor.a = 0.35f;
            _abilityIndicator.color = newColor;
            _bodyRenderer.material.color = newColor;
        }

        public void ResetStatus()
        {
            _cloth.worldAccelerationScale = 0f;
            _moveMult = 1f;
            _cloth.gameObject.SetActive(false);
            _canMove = false;
            _turnTime = _defaultTurnTime;
            _moveDirection = Vector3.zero;

            if(_floatToGroundRoutine != null)
            {
                StopCoroutine(_floatToGroundRoutine);
            }

            transform.position = _spawnPosition;
            _body.localPosition = _defaultLocalBodyPos;
            transform.forward = Vector3.back;
            _cloth.gameObject.SetActive(true);

            IsAlive = true;
            _animator.SetBool("isHit", false);
            _animator.Play("Idle");
        }
        public void DisalowMovement()
        {
            _canMove = false;
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
            MyGamepad.SetRumble(Rumble.Light);
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
                MyGamepad.ResetRumble();
            }
            else
            {
                _isOnCooldown = true;
                _abilityIndicator.fillAmount = 0f;
                this.AutoLerp(0f, 1f, _abilityCooldown, t => _abilityIndicator.fillAmount = t);
                _ability.AbilityActivated();
                MyGamepad.SetRumbleForDuration(Rumble.Medium, 0.2f);
                this.DoAfter(_abilityCooldown, () => _isOnCooldown = false);
            }
            _animator.SetBool("AbilityCharging", false);

        }

        private IEnumerator FloatToGroundRoutine()
        {
            Vector3 currentPos = _body.localPosition;
            float y = currentPos.y;

            yield return _floatToGroundRoutine = this.AutoLerp(y, y - 1.2f, GameManager.Settings.PlayerDeathAnimationLength, FloatDown);

            _floatToGroundRoutine = null;

            void FloatDown(float yPos)
            {
                currentPos.y = yPos;
                _body.localPosition = currentPos;
            }
        }

        

        // ----------------------------------------------------------------------------------------
    }
}