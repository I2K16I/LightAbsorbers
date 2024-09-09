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
		[SerializeField] private Transform _beam;
		[SerializeField] private CapsuleCollider _collider;
		[SerializeField] private MeshRenderer _indicator;
		[SerializeField] private Material _beamMaterial;
		[SerializeField] private Material _indicatorMaterial;
		private Settings _settings;

		// --- Properties ---------------------------------------------------------------------------------------------
		
		// --- Events -------------------------------------------------------------------------------------------------

		// --- Unity Functions ----------------------------------------------------------------------------------------
		private void Awake()
		{
			_settings = GameManager.Settings;
		}

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.TryGetComponent(out PlayerMovement player))
			{
				player.Hit();
			}
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------

		public void SetNewProperties(Transform pointOne, Transform pointTwo)
		{
            Vector3 pointOnePos = pointOne.position;
            Vector3 pointTwoPos = pointTwo.position;

			transform.position = new Vector3(pointOne.position.x, pointTwo.position.y, pointOne.position.z) ;

            Vector3 vectorBetween = pointOnePos - pointTwoPos;
            Vector3 normal = Vector3.Cross(vectorBetween, Vector3.up);
            transform.forward = normal;

			float targetScale = Vector3.Distance(pointOnePos, pointTwoPos);
			StartCoroutine(ScaleUpRoutine(targetScale));
        }

		// --- Protected/Private Methods ------------------------------------------------------------------------------
		private IEnumerator ScaleUpRoutine(float targetScale)
		{
			
			//Debug.Log(targetScale);
			float scalingSinceSeconds = 0.0f;
			float scaleDuration = _settings.BeamScaleUpDuration;
			float lifetime = _settings.AttackDuration;
			float windUpTime = _settings.WindUpTime;

			if(windUpTime + scaleDuration > lifetime)
			{
				scaleDuration = lifetime/8;
				windUpTime = lifetime / 8;
			}

            Vector3 scale = transform.localScale;
            float start = scale.x;

			//while (scalingSinceSeconds < scaleDuration)

			while(scalingSinceSeconds < scaleDuration)
                {
				//Debug.Log(scalingSinceSeconds);
				scale.x = Mathf.Lerp(start, targetScale, scalingSinceSeconds / scaleDuration);
                transform.localScale = scale;
                scalingSinceSeconds += Time.deltaTime;
				yield return null;
			}

			scale.x = targetScale;
            transform.localScale = scale;

			yield return new WaitForSeconds(windUpTime);

			// Enable colliders
            _collider.enabled = true;
            _indicator.material = _beamMaterial;
			

			// Attention: the lifetime could potentially be less then 0, even though it't not allowed to be!
            yield return new WaitForSeconds(lifetime-(windUpTime + scaleDuration));

			// Destroy self
            Destroy(gameObject);
        }

		// ----------------------------------------------------------------------------------------
	}
}