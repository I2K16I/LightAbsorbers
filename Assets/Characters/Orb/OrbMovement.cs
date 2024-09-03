using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BSA
{
    public class OrbMovement : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private Rigidbody _rigidBody;
        [SerializeField] private float _moveSpeed = 1.0f;

        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _directionBuffer = Vector3.zero;

        // --- Properties ---------------------------------------------------------------------------------------------
        public bool IsPaused { get; private set; }

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Awake()
        {
            float rngRotation = Random.Range(0f, 360f);
            transform.Rotate(0f, rngRotation, 0f);
            _moveDirection = transform.forward;

        }

        private void FixedUpdate()
        {
            _moveDirection.y = 0f;
            //transform.position += _moveDirection * Time.fixedDeltaTime * _moveSpeed;
            _rigidBody.velocity = _moveDirection * _moveSpeed;
            Debug.DrawRay(transform.position, _moveDirection * 2f, Color.red);
        }

        private void OnCollisionEnter(Collision collision)
        {
            bool isReflected = true;
            //Rigidbody rb = collision.rigidbody;
            //Debug.Log($"{name} hit object {collision.gameObject.name}", collision.gameObject);

            if (collision.gameObject.TryGetComponent(out PlayerMovement player))
            {
                //Debug.Log($"Hit player {player.name}");
                isReflected = false;
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

        // --- Protected/Private Methods ------------------------------------------------------------------------------
        private void TurnOrbDirection(Vector3 normal)
        {
            // Debug.Log(Vector3.SignedAngle(transform.forward, colForward, Vector3.up));
            normal.y = 0f;
            float angle = Vector3.SignedAngle(-_moveDirection, normal, Vector3.up);
            //transform.Rotate(new Vector3(0, turnDegree, 0));
            _moveDirection = Quaternion.Euler(0f, 2f * angle, 0f) * -_moveDirection;
        }

        // ----------------------------------------------------------------------------------------
    }
}