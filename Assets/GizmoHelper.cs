using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace BSA
{
	public class GizmoHelper : MonoBehaviour
	{
		// --- Fields -------------------------------------------------------------------------------------------------
		[SerializeField] private bool _drawForward = false;
        [SerializeField] private float _forwardLength = 5.0f;
		// --- Properties ---------------------------------------------------------------------------------------------
		
		// --- Events -------------------------------------------------------------------------------------------------

		// --- Unity Functions ----------------------------------------------------------------------------------------
		private void Awake()
		{
			
		}

        void OnDrawGizmosSelected()
        {          
            if(_drawForward)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, transform.position + transform.forward* _forwardLength);
            }             
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------

        // --- Protected/Private Methods ------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
    }
}