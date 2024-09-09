using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BSA
{
	public class OrbManager : MonoBehaviour
	{
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private bool _showSpawnRadius = true;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private float _spawnRadius = 1f;
        [SerializeField] private GameObject[] _outerOrbs;
        [SerializeField] private BeamSpawner _beamSpawner;

        private List<OuterOrbs> _outerOrbScripts = new List<OuterOrbs>();
        private List<int> attackList = new List<int>();
        private OrbMovement[] _orbs;


		// --- Properties ---------------------------------------------------------------------------------------------
		
		// --- Events -------------------------------------------------------------------------------------------------

		// --- Unity Functions ----------------------------------------------------------------------------------------
		private void Awake()
		{
            _orbs = new OrbMovement[GameManager.Settings.NumberOfOrbs];
			Vector3 spawnPosition = transform.position;
            Vector3 offset = Vector3.zero;

            for(int i  = 0; i < GameManager.Settings.NumberOfOrbs; ++i)
            {
                offset = new Vector3(Random.Range(-_spawnRadius, _spawnRadius), 0, Random.Range(-_spawnRadius, _spawnRadius));
                spawnPosition += offset;
                _orbs[i] = Instantiate(_prefab, spawnPosition, Quaternion.identity).GetComponent<OrbMovement>();
                spawnPosition = transform.position;
            }

            foreach(GameObject outerOrbObject in _outerOrbs)
            {
                _outerOrbScripts.Add(outerOrbObject.GetComponent<OuterOrbs>());
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
        public void StartOrbs()
        {
            for (int i = 0; i < _orbs.Length; ++i)
            {
                _orbs[i].StartOrbs();
            }
        }

        public void OrbAttack(float duration)
        {
            int orbOne;
            OrbMovement orbTwo;
            int orbThree;
            int IndexOfMovingOrb;

 
            SelectOrbs(out orbOne, out orbTwo, out orbThree, out IndexOfMovingOrb);

            _orbs[IndexOfMovingOrb].PauseMovement();
            _outerOrbScripts[orbOne].IsAlreadyAttacking = true;
            _outerOrbScripts[orbThree].IsAlreadyAttacking = true;
            _outerOrbScripts[orbOne].AttackId = attackList.Count;
            _outerOrbScripts[orbThree].AttackId = attackList.Count;
            attackList.Add(attackList.Count);

            _beamSpawner.SpawnBeamAt(_outerOrbs[orbOne].transform, orbTwo.transform);
            _beamSpawner.SpawnBeamAt(_outerOrbs[orbThree].transform, orbTwo.transform);

            this.DoAfter(duration, () => EndOrbAttack(IndexOfMovingOrb));


            }

        public void EndGame()
        {
            for (int i = 0; i < _orbs.Length; ++i)
            {
                _orbs[i].PauseMovement();
            }
            StartCoroutine(DisableOrbsOverTime()); 
        }
        // --- Protected/Private Methods ------------------------------------------------------------------------------
        private IEnumerator DisableOrbsOverTime()
        {
            for (int i = 0; i < _orbs.Length; ++i)
            {
                _orbs[i].gameObject.SetActive(false);
                yield return new WaitForSeconds(.5f);
            }
        }

        private void SelectOrbs(out int orbOne, out OrbMovement orbTwo, out int orbThree, out int IndexOfMovingOrb)
        {
  
            IndexOfMovingOrb = 0;
            int outerOrbOne = 0;
            int outerOrbTwo = 0;
            for(int i = 0; i < 1; i++)
            {
                outerOrbOne = Random.Range(0, _outerOrbs.Length - 1);
                outerOrbTwo = Random.Range(0, _outerOrbs.Length - 1);
                if(SameOrAdjacent(outerOrbOne, outerOrbTwo, _outerOrbs.Length-1))
                {
                    i--;
                }
                else if(_outerOrbScripts[outerOrbOne].IsAlreadyAttacking || _outerOrbScripts[outerOrbTwo].IsAlreadyAttacking)
                {
                    i--;
                }
            }
            for(int i = 0; i < 1; i++)
            {
                IndexOfMovingOrb = Random.Range(0, _orbs.Length - 1);
                if(_orbs[IndexOfMovingOrb].IsPaused)
                {
                    i--;
                }
            }
            _outerOrbScripts[outerOrbOne].IsAlreadyAttacking = true;
            _outerOrbScripts[outerOrbTwo].IsAlreadyAttacking = true;
            orbOne = outerOrbOne;
            orbTwo = _orbs[IndexOfMovingOrb];
            orbThree = outerOrbTwo;
            
        }

        private bool SameOrAdjacent(int numOne, int numTwo, int maxLength)
        {
            bool isSameOrAdjacent = false;

            if(numOne == numTwo)
            {
                isSameOrAdjacent = true;
            } else if(numOne == numTwo +1) 
            {
                isSameOrAdjacent = true;
            } else if (numOne == numTwo -1) 
            {
                isSameOrAdjacent = true;
            } else if (numOne == 0 && numTwo == maxLength)
            {
                isSameOrAdjacent = true;
            } else if (numOne == maxLength && numTwo == 0)
            {
                isSameOrAdjacent = true;
            }

            return isSameOrAdjacent;
        }
        private void EndOrbAttack(int IndexOfStoppedOrb)
        {
            _orbs[IndexOfStoppedOrb].ResumeMovement();
            int attackToEnd = attackList.ElementAt(0);

            foreach(OuterOrbs orb in _outerOrbScripts.Where(orb => orb.AttackId == attackToEnd))
            {
                orb.AttackId = -1;
                orb.IsAlreadyAttacking = false;
            }

            attackList.Remove(attackToEnd);
        }
        // ----------------------------------------------------------------------------------------
    }
}