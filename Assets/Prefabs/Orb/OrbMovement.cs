using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace BSA
{
    public class OrbMovement : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private Rigidbody _rigidBody;
        [SerializeField] private float _currentMoveSpeed = 1.0f;
        [SerializeField] private Transform _center;
        [SerializeField] private float _deceleration = .01f;
        [SerializeField] private SphereCollider _collider;
        [SerializeField] private MeshRenderer _orb;

        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _directionBuffer = Vector3.zero;
        private Settings _settings;
        private float _startSpeed;
        private float _endSpeed;
        private float _speedMult = 1f;
        private float _timeTillEndSpeed;
        private Color _startEmission;

        private int _orbLayer;
        private int _refelctedLayer;


        // --- Properties ---------------------------------------------------------------------------------------------
        public bool IsPaused { get; private set; }
        public Transform Center => _center;
        public SphereCollider Colider => _collider;

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Awake()
        {
            _settings = GameManager.Settings;
            _startSpeed = _settings.OrbStartSpeed;
            _endSpeed = _settings.OrbEndSpeed;
            _timeTillEndSpeed = _settings.TimeTillEndSpeed;
            _currentMoveSpeed = _startSpeed;
            _startEmission = _orb.material.GetColor("_EmissionColor");

            float randomAngle = Random.Range(0f, 360f);
            _moveDirection = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;
            IsPaused = true;

            //_moveDirection = transform.forward;
        }

        private void Start()
        {
            _orbLayer = LayerMask.NameToLayer("Orb");
            _refelctedLayer = LayerMask.NameToLayer("OrbNoCollision");
        }

        private void FixedUpdate()
        {
            if (IsPaused)
            {
                _rigidBody.velocity = Vector3.zero;
                return;
            }
            _moveDirection.y = 0f;
            //transform.position += _moveDirection * Time.fixedDeltaTime * _moveSpeed;
            _rigidBody.velocity = _moveDirection * _currentMoveSpeed * _speedMult;
            Debug.DrawRay(transform.position, _moveDirection * 2f, Color.red);
        }

        private void OnCollisionEnter(Collision collision)
        {
            bool isReflected = true;
            //Rigidbody rb = collision.rigidbody;
            //Debug.Log($"{name} hit object {collision.gameObject.name}", collision.gameObject);

            if (collision.gameObject.TryGetComponent(out PlayerMovement player))
            {
                player.Hit();
                isReflected = false;
            } else if (collision.gameObject.TryGetComponent(out Shockwave shockwave))
            {
                isReflected = false;
                //TurnOrbDirection()
            }
            //else if (rb != null)
            //{
            //    if (rb.TryGetComponent(out OrbMovement orb))
            //    {
            //    }
            //}

            // Reflect orb
            if (isReflected && !IsPaused)
            {
                Vector3 normal = collision.GetContact(0).normal;
                TurnOrbDirection(normal);
            }
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public void PauseMovement()
        {
            IsPaused = true;
            _directionBuffer = _moveDirection;
            _moveDirection = Vector3.zero;
        }

        public void ResumeMovement()
        {
            IsPaused = false;
            _moveDirection = _directionBuffer;
            _directionBuffer = Vector3.zero;
        }

        public void StartOrb()
        {
            IsPaused = false;
            this.AutoLerp(_startSpeed, _endSpeed, _timeTillEndSpeed, speed => _currentMoveSpeed = speed);
        }

        public void Reflect(Vector3 direction, float speedMultiplier, Color newColor)
        {
            gameObject.layer = _refelctedLayer;
            _orb.material.SetColor("_EmissionColor", newColor * 4);
            direction.y = 0;
            _speedMult = speedMultiplier;
            //this.AutoLerp(_speedMult, 1f, 3f, reducedSpeed => _speedMult = reducedSpeed);
            _moveDirection = direction;
            StartCoroutine(ReduceSpeedMultRoutine());
        }

        // --- Protected/Private Methods ------------------------------------------------------------------------------
        private void TurnOrbDirection(Vector3 normal)
        {
            // Debug.Log(Vector3.SignedAngle(transform.forward, colForward, Vector3.up));
            normal.y = 0f;
            float angle = Vector3.SignedAngle(-_moveDirection, normal, Vector3.up);
            //transform.Rotate(new Vector3(0, turnDegree, 0));
            _moveDirection = Quaternion.Euler(0f, 2f * angle, 0f) * -_moveDirection;
        }

        private IEnumerator ReduceSpeedMultRoutine()
        {
            do
            {
                yield return new WaitForSeconds(0.02f);
                _speedMult -= _deceleration;

            } while(_speedMult > 1f);

            _speedMult = 1f;
            gameObject.layer = _orbLayer;
            _orb.material.SetColor("_EmissionColor", _startEmission);
        }
        // ----------------------------------------------------------------------------------------
    }
}