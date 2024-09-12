using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace BSA
{
    public class TriggerCallbacks : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private UnityEvent<Collider> _triggerEnter;
        [SerializeField] private UnityEvent<Collider> _triggerExit;

        // --- Properties ---------------------------------------------------------------------------------------------
        public UnityEvent<Collider> TriggerEnter => _triggerEnter;
        public UnityEvent<Collider> TriggerExit => _triggerExit;

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------       
        private void OnTriggerEnter(Collider other)
        {
            _triggerEnter.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            _triggerExit.Invoke(other);
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------

        // --- Protected/Private Methods ------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
    }
}