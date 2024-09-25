using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;
using Color = UnityEngine.Color;

namespace BSA
{
	public class Shockwave : MonoBehaviour
	{
		// --- Fields -------------------------------------------------------------------------------------------------

		[SerializeField] private GameObject _waveObject;
		[SerializeField] private ParticleSystem _particleWave;
		[SerializeField] private VisualEffect _buildUp;
		[SerializeField] Renderer _groundMaterial;
		[SerializeField] float _buildUpTime = 0.5f;
		[SerializeField] float _fadeAwayLength = 2f;
		[SerializeField] float _lifeTime = 4f;
		[SerializeField] float _attackRadius = 4f;
		[SerializeField, Range(0f, 90f)] float _maxAttackAngle = 30f;
		[SerializeField] LayerMask _collisionLayer;
		[SerializeField] bool _testPlay = false;

		private float _currentRadius = 0f;

        // --- Properties ---------------------------------------------------------------------------------------------

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void OnDrawGizmos()
        {
            if(_currentRadius > 0f)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(transform.position, _currentRadius);
			}

			Gizmos.color = Color.cyan;
			Vector3 directionA = Quaternion.Euler(0f, -_maxAttackAngle, 0f) * transform.forward;
			Vector3 directionB = Quaternion.Euler(0f, _maxAttackAngle, 0f) * transform.forward;
			Gizmos.DrawRay(transform.position, directionA * _attackRadius);
			Gizmos.DrawRay(transform.position, directionB * _attackRadius);			
        }

        private void Awake()
		{

        }

        private void Update()
        {
            if(_testPlay)
			{
                StartCoroutine(AbilityRoutine());
				_testPlay = false;
			}
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------
		public void AbilityPressed()
		{
			_buildUp.Play();
            // If the GameObject is persistend and does not get respawned every time the ability is used it needs to be positioned here.
            // Possibly a parameter of type Transform for the new Parent
            // Something like this:
            // transform.parent = transformParamenter
            // transform.position = transformParamenter.position
            // transform.forward = transformParamenter.forward
        }

        public void AbilityCanceled()
		{
			_buildUp.Stop();
		}

		public void AbilityActivated()
		{
			_buildUp.Stop();
			transform.parent = null;
			StartCoroutine(AbilityRoutine());
		}

		public void ChangeColor(Color newColor)
		{
			_buildUp.SetVector4("Color", newColor);
			var main = _particleWave.main;
			main.startColor = newColor;
			Renderer particle = _particleWave.gameObject.GetComponent<Renderer>();
            particle.material.SetColor("_EmissionColor", newColor * 4);
            _groundMaterial.material.SetColor("_Color", newColor);
			// Uncomment this if you want the ground to glow after an ability
			//_groundMaterial.material.SetColor("_Emission", newColor * 4);
		}
		// --- Protected/Private Methods ------------------------------------------------------------------------------
		private IEnumerator AbilityRoutine()
		{
			//_waveObject.SetActive(true);
			_particleWave.Play();
			_groundMaterial.material.SetFloat("_TotalAlpha", 0);
			float scale = transform.localScale.x;

			//Collider[] colliders = new Collider[GameManager.Settings.NumberOfOrbs];
			Collider[] colliders = new Collider[10];
			HashSet<Collider> positiveHits = new();

			int alphaSlider = Shader.PropertyToID("_AlphaSlider");

            yield return this.AutoLerp(0f, 1f, _buildUpTime, SetRadius);

			void SetRadius(float t)
			{
                _currentRadius = t * _attackRadius;
                _groundMaterial.material.SetFloat(alphaSlider, t * .5f);
				Vector3 center = transform.position;

                int numColliders = Physics.OverlapSphereNonAlloc(center, _currentRadius, colliders, _collisionLayer);
                for(int i = 0; i < numColliders; i++)
                {
					if(positiveHits.Contains(colliders[i]))
						continue;

                    if(colliders[i].TryGetComponent(out OrbMovement orb))
                    {
						Vector3 toTarget = orb.transform.position - center;
						float distance = toTarget.magnitude;
						Vector3 straightTarget = center + transform.forward * distance;
						Vector3 closestPoint = colliders[i].ClosestPoint(straightTarget);
						Vector3 direction = (closestPoint - center).normalized;

						float angle = Vector3.SignedAngle(direction, transform.forward, Vector3.up);

                        //Debug.Log("Found Orb");
                        if(Mathf.Abs(angle) <= _maxAttackAngle)
						{
							Debug.DrawLine(center, closestPoint, Color.green, 1f);
							//Debug.Log("Reflected Orb");
							float distanceT = 1f - Mathf.InverseLerp(.5f, _attackRadius, distance);
							float speedMultiplier = Mathf.Lerp(1f, 5f, distanceT);

							orb.Reflect(transform.forward, speedMultiplier);
							positiveHits.Add(colliders[i]);
                        }
                    }
                }
            }

			//_waveObject.SetActive(false);

			yield return new WaitForSeconds(_lifeTime);

			// _AlphaSlider

			// _TotalAlpha
			int totalAlpha = Shader.PropertyToID("_TotalAlpha");
			yield return this.AutoLerp(0f, 1f, _fadeAwayLength, SetGroundAlpha);

			void SetGroundAlpha(float alpha)
			{
                _groundMaterial.material.SetFloat(totalAlpha, alpha);
            }
        }

        // ----------------------------------------------------------------------------------------
    }
}