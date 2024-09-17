using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BSA
{
	public class Shockwave : MonoBehaviour
	{
		// --- Fields -------------------------------------------------------------------------------------------------

		[SerializeField] private GameObject _waveObject;
		[SerializeField] private ParticleSystem _particleWave;
		[SerializeField] Transform _waveTransform;
		[SerializeField] Renderer _groundMaterial;
		[SerializeField] float _buildUpTime = 0.5f;
		[SerializeField] float _fadeAwayLength = 2f;
		[SerializeField] float _lifeTime = 4f;
		[SerializeField] bool _testPlay = false;

		// --- Properties ---------------------------------------------------------------------------------------------
		
		// --- Events -------------------------------------------------------------------------------------------------

		// --- Unity Functions ----------------------------------------------------------------------------------------
		private void Awake()
		{
			//StartCoroutine(AbilityRoutine());
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

        // --- Protected/Private Methods ------------------------------------------------------------------------------
        private IEnumerator AbilityRoutine()
		{
			//_waveObject.SetActive(true);
			_particleWave.Play();

            double startTime = Time.timeAsDouble;
            Vector3 currentSize = _waveTransform.localScale;
            float y = currentSize.x;
			float progress;
			Collider[] colliders;
            float t = 0f;
            while(t < 1f)
            {

                //Debug.Log(scalingSinceSeconds);
                yield return null;
                t = Mathf.Clamp01((float)(Time.timeAsDouble - startTime) / _buildUpTime);
                //scalingSinceSeconds += Time.deltaTime;
				progress = Mathf.Lerp(y, 1, t);
				//currentSize.x = progress;
				//currentSize.z = progress;
				_groundMaterial.material.SetFloat("_AlphaSlider", progress / 2);
				//_waveTransform.localScale = currentSize;
            }
			//_waveObject.SetActive(false);

			yield return new WaitForSeconds(_lifeTime);

			// _AlphaSlider

			// _TotalAlpha
			startTime = Time.timeAsDouble;
			t = 0f;

			while (t < 1f)
			{
				yield return null;
                t = Mathf.Clamp01((float)(Time.timeAsDouble - startTime) / _fadeAwayLength);
				_groundMaterial.material.SetFloat("_TotalAlpha", Mathf.Lerp(0, 1, t));
            }

        }
        // ----------------------------------------------------------------------------------------
    }
}