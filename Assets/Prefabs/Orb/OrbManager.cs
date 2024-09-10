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
        [SerializeField] private List<OuterOrb> _outerOrbs;
        [SerializeField] private BeamSpawner _beamSpawner;

        private readonly List<int> attackList = new();
        private readonly List<OrbMovement> _orbs = new();


        // --- Properties ---------------------------------------------------------------------------------------------

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Awake()
        {
            Vector3 spawnPosition = transform.position;
            Vector3 offset = Vector3.zero;

            for(int i = 0; i < GameManager.Settings.NumberOfOrbs; ++i)
            {
                offset = new Vector3(Random.Range(-_spawnRadius, _spawnRadius), 0, Random.Range(-_spawnRadius, _spawnRadius));
                spawnPosition += offset;
                _orbs.Add(Instantiate(_prefab, spawnPosition, Quaternion.identity).GetComponent<OrbMovement>());
                spawnPosition = transform.position;
            }
        }

        void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere at the transform's position
            if(_showSpawnRadius)
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
            for(int i = 0; i < _orbs.Count; ++i)
            {
                _orbs[i].StartOrb();
            }
        }

        public void OrbAttack(float duration)
        {
            if(TrySelectOrbs(out OuterOrb orbOne, out OrbMovement orbTwo, out OuterOrb orbThree))
            {
                orbTwo.PauseMovement();
                orbOne.IsAlreadyAttacking = true;
                orbThree.IsAlreadyAttacking = true;
                orbOne.AttackId = attackList.Count;
                orbThree.AttackId = attackList.Count;
                attackList.Add(attackList.Count);

                _beamSpawner.SpawnBeamAt(orbOne.transform, orbTwo.Center);
                _beamSpawner.SpawnBeamAt(orbThree.transform, orbTwo.Center);

                this.DoAfter(duration, () => EndOrbAttack(orbTwo));
            }
            else
            {
                Debug.LogWarning($"Orb attack failed! Couldn't find orb!");
            }
        }

        public void EndGame()
        {
            for(int i = 0; i < _orbs.Count; ++i)
            {
                _orbs[i].PauseMovement();
            }
            StartCoroutine(DisableOrbsOverTime());
        }
        // --- Protected/Private Methods ------------------------------------------------------------------------------
        private IEnumerator DisableOrbsOverTime()
        {
            for(int i = 0; i < _orbs.Count; ++i)
            {
                _orbs[i].gameObject.SetActive(false);
                yield return new WaitForSeconds(.5f);
            }
        }

        private bool TrySelectOrbs(out OuterOrb orbOne, out OrbMovement orbTwo, out OuterOrb orbThree)
        {
            orbOne = null;
            orbTwo = null;
            orbThree = null;
            int attempts = 0;

            for(int i = 0; i < 1; i++)
            {
                if(attempts == 30)
                {
                    Debug.LogWarning("Kein Orb gefunden");
                    return false;
                }

                orbOne = _outerOrbs[Random.Range(0, _outerOrbs.Count - 1)];
                orbThree = _outerOrbs[Random.Range(0, _outerOrbs.Count - 1)];

                if(SameOrAdjacent(orbOne, orbThree))
                {
                    i--;
                    attempts++;
                }
                else if(orbOne.IsAlreadyAttacking || orbThree.IsAlreadyAttacking)
                {
                    i--;
                    attempts++;
                }
            }

            OrbMovement[] unpausedOrbs = _orbs.Where(o => o.IsPaused == false).ToArray();
            if(unpausedOrbs.Length == 0)
            {
                return false;
            }

            orbTwo = unpausedOrbs[Random.Range(0, unpausedOrbs.Count() - 1)];

            for(int i = 0; i < 1; i++)
            {
                if(orbTwo.IsPaused)
                {
                    i--;
                }
            }

            orbOne.IsAlreadyAttacking = true;
            orbThree.IsAlreadyAttacking = true;
            return true;
        }

        private bool SameOrAdjacent(OuterOrb numOne, OuterOrb numTwo)
        {
            int indexOne = _outerOrbs.IndexOf(numOne);
            int indexTwo = _outerOrbs.IndexOf(numTwo);
            int dif = Mathf.Abs(indexTwo - indexOne);

            if(dif <= 1 || dif == _outerOrbs.Count - 1)
            {
                return true;
            }
            return false;
        }

        private void EndOrbAttack(OrbMovement orb)
        {
            orb.ResumeMovement();
            int attackToEnd = attackList.ElementAt(0);

            foreach(OuterOrb outerOrb in _outerOrbs.Where(orb => orb.AttackId == attackToEnd))
            {
                outerOrb.AttackId = -1;
                outerOrb.IsAlreadyAttacking = false;
            }

            attackList.Remove(attackToEnd);
        }
        // ----------------------------------------------------------------------------------------
    }
}