using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BSA
{
	public class BeamManager : MonoBehaviour
	{
		// --- Fields -------------------------------------------------------------------------------------------------
		[SerializeField] private Transform _container;
		[SerializeField] private CapsuleCollider _collider;
		[SerializeField] private MeshRenderer _indicator;
		[SerializeField] private Material _beamMaterial;
		[SerializeField] private Material _indicatorMaterial;
		
		private Vector3 _startPos = new Vector3(0, -5, 0);
		// --- Properties ---------------------------------------------------------------------------------------------
		
		// --- Events -------------------------------------------------------------------------------------------------

		// --- Unity Functions ----------------------------------------------------------------------------------------
		private void Awake()
		{
	
		}

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement player))
			{
				player.Hit();
			}
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------

		public void SetNewPoision(Transform pointOne, Transform pointTwo)
		{
            Vector3 pointOnePos = pointOne.position;
            Vector3 pointTwoPos = pointTwo.position;

			_container.position = new Vector3(pointOne.position.x, pointTwo.position.y, pointOne.position.z) ;

            Vector3 vectorBetween = pointOnePos - pointTwoPos;
            Vector3 normal = Vector3.Cross(vectorBetween, Vector3.up);
            _container.forward = normal;

			float targetScale = Vector3.Distance(pointOnePos, pointTwoPos);
			StartCoroutine(ScaleUpRoutine(.5f, targetScale));
        }

		public void ResetPosition()
		{
            _container.position = _startPos;
            _container.localScale = new Vector3(0.1f, 1, 0.1f);
			_collider.enabled = false;
			_indicator.material = _indicatorMaterial;
        }
		// --- Protected/Private Methods ------------------------------------------------------------------------------
		private IEnumerator ScaleUpRoutine(float scaleDuration, float targetScale)
		{
			//Debug.Log(targetScale);
			float scalingSinceSeconds = 0.0f;
            float currentScale = transform.localScale.x;
			float targetScaleVector = targetScale;
			while (scalingSinceSeconds < scaleDuration)
			{
                //Debug.Log(scalingSinceSeconds);
                _container.localScale = new Vector3(Mathf.Lerp(currentScale, targetScale, scalingSinceSeconds / scaleDuration), 1, 1);
                scalingSinceSeconds += Time.deltaTime;
				yield return null;
			}

            _container.localScale = new Vector3(targetScale, 1, 1);
			this.DoAfter(1f, StartBeam);
			
		}

		private void StartBeam()
		{
			_collider.enabled = true;
			_indicator.material = _beamMaterial;
		}
		// ----------------------------------------------------------------------------------------
	}
}