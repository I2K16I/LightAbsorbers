using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BSA
{
	public class OrbSpawner : MonoBehaviour
	{
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private bool _showSpawnRadius = true;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _SpawnCount = 8;
        [SerializeField] private float _spawnRadius = 1f;

		// --- Properties ---------------------------------------------------------------------------------------------
		
		// --- Events -------------------------------------------------------------------------------------------------

		// --- Unity Functions ----------------------------------------------------------------------------------------
		private void Awake()
		{
			Vector3 spawnPosition = transform.position;
            Vector3 offset = Vector3.zero;

            for(int i  = 0; i < _SpawnCount; ++i)
            {
                offset = new Vector3(Random.Range(-_spawnRadius, _spawnRadius), 0, Random.Range(-_spawnRadius, _spawnRadius));
                spawnPosition += offset;
                Instantiate(_prefab, spawnPosition, Quaternion.identity);
                spawnPosition = transform.position;
            }
		}

        void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere at the transform's position
            if (_showSpawnRadius)
            {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * _spawnRadius);
            }
        }
        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------

        // --- Protected/Private Methods ------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------
    }
}