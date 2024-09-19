using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BSA
{
    public enum CameraType
    {
        None = 0,
        Join = 1,
        Game = 2,
        Win = 3
    }

    public class CameraCollector : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private CinemachineVirtualCamera _camera;
        [SerializeField] private CameraType _type;

        private static List<CameraCollector> _cameras = new();

        // --- Properties ---------------------------------------------------------------------------------------------
        public CameraType Type => _type;
        public CinemachineVirtualCamera Camera => _camera;


        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void OnEnable()
        {
            _cameras.Add(this);
        }

        private void OnDisable()
        {
            _cameras.Remove(this);
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public static bool TryGetCamera(CameraType type, out CameraCollector camera)
        {
            camera = _cameras.FirstOrDefault(c => c.Type == type);
            return camera != null;
        }
             
        // --- Protected/Private Methods ------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
    }
}