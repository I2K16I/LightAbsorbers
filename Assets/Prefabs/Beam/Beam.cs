using System.Collections;
using UnityEngine;

namespace BSA
{
    public class Beam : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private TriggerCallbacks _beamMeshTrigger;
        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private CapsuleCollider _reflectorCollider;
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
            _collider.enabled = false;
        }

        private void OnEnable()
        {
            _beamMeshTrigger.TriggerEnter.AddListener(OnBeamTriggerEnter);
        }

        private void OnDisable()
        {
            _beamMeshTrigger.TriggerEnter.RemoveListener(OnBeamTriggerEnter);
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------
        public void OnBeamTriggerEnter(Collider other)
        {
            if(other.gameObject.TryGetComponent(out PlayerMovement player))
            {
                player.Hit();
            }
        }

        // --- Public/Internal Methods --------------------------------------------------------------------------------

        public void PerformAttack(Transform outerOrb, Transform innerOrb)
        {
            Vector3 outerOrbPos = outerOrb.position;
            Vector3 innerOrbPos = innerOrb.position;

            transform.position = outerOrbPos;

            Vector3 vectorBetween = outerOrbPos - innerOrbPos;
            transform.right = vectorBetween;
            //Vector3 normal = Vector3.Cross(vectorBetween, Vector3.up);
            //transform.forward = normal;

            float targetScale = Vector3.Distance(outerOrbPos, innerOrbPos);
            StartCoroutine(ScaleUpRoutine(targetScale));
        }

        // --- Protected/Private Methods ------------------------------------------------------------------------------
        private IEnumerator ScaleUpRoutine(float targetScale)
        {
            //Debug.Log(targetScale);
            float scaleDuration = _settings.BeamScaleUpDuration;
            float windUpTime = _settings.BeamWindUpTime;
            float lifetime = _settings.BeamActiveDuration;

            Vector3 scale = transform.localScale;
            scale.x = 0f;
            transform.localScale = scale;

            double startTime = Time.timeAsDouble;
            
            float t = 0f;
            while(t < 1f)
            {
                //Debug.Log(scalingSinceSeconds);
                yield return null;
                t = Mathf.Clamp01((float)(Time.timeAsDouble - startTime) / scaleDuration);
                
                //scalingSinceSeconds += Time.deltaTime;
                scale.x = Mathf.Lerp(0f, targetScale, t);
                transform.localScale = scale;
            }           

            yield return new WaitForSeconds(windUpTime);

            // Enable colliders
            _collider.enabled = true;
            _indicator.material = _beamMaterial;

            if(GameManager.Settings.ReflectingBeams)
            {
                _reflectorCollider.enabled = true;
            }

            yield return new WaitForSeconds(lifetime);

            // Destroy self
            Destroy(gameObject);
        }

        // ----------------------------------------------------------------------------------------
    }
}