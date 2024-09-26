using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BSA
{
    public class OrbManager : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] private OrbMovement _prefab;
        [SerializeField] private List<OuterOrb> _outerOrbs;
        [SerializeField] private List<Transform> _spawnPoints;
        [SerializeField] private BeamSpawner _beamSpawner;

        private readonly List<OrbMovement> _orbs = new();


        // --- Properties ---------------------------------------------------------------------------------------------

        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Awake()
        {
            GameManager.Instance.OrbManager = this;

            int amountOfOrbsToCreate = OptionsManager.Options.numberOfOrbs + 1;
            if(amountOfOrbsToCreate > _spawnPoints.Count)
            {
                amountOfOrbsToCreate = _spawnPoints.Count;
            }

            for(int i = 0; i < amountOfOrbsToCreate; ++i)
            {
                _orbs.Add(Instantiate(_prefab, _spawnPoints[i].position, Quaternion.identity));
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

        public void OrbAttack(float attackDuration)
        {
            if(TrySelectOrbs(out OuterOrb outerOrbA, out OuterOrb outerOrbB, out OrbMovement innerOrb))
            {
                innerOrb.PauseMovement();
                innerOrb.Colider.enabled = false;
                outerOrbA.IsAttacking = true;
                outerOrbB.IsAttacking = true;
                _beamSpawner.SpawnBeamAt(outerOrbA.Center, innerOrb.Center);
                _beamSpawner.SpawnBeamAt(outerOrbB.Center, innerOrb.Center);

                this.DoAfter(attackDuration, () =>
                {
                    innerOrb.ResumeMovement();
                    innerOrb.Colider.enabled = true;
                    outerOrbA.IsAttacking = false;
                    outerOrbB.IsAttacking = false;
                });
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
            yield return new WaitForSeconds(1f);
            for(int i = 0; i < _orbs.Count; ++i)
            {
                _orbs[i].gameObject.SetActive(false);
                yield return new WaitForSeconds(.5f);
            }
        }

        /// <summary>
        /// Tries to select two <see cref="OuterOrb"/> and a single <see cref="OrbMovement"/> for performing an attack.
        /// The outer orbs cannot already be attacking and cannot be adjacent.
        /// The inner orb must still be moving.
        /// Will return <c>false</c>, if these conditions cannot be met.
        /// </summary>
        /// <param name="outerOrbA">The first outer orb selected</param>
        /// <param name="outerOrbB">The second outer orb selected</param>
        /// <param name="innerOrb">The inner orb selected</param>
        private bool TrySelectOrbs(out OuterOrb outerOrbA, out OuterOrb outerOrbB, out OrbMovement innerOrb)
        {
            outerOrbA = null;
            innerOrb = null;
            outerOrbB = null;

            // Filter out all Orbs already attacking
            List<OuterOrb> filteredOuterOrbs = _outerOrbs.Where(o => o.IsAttacking == false).ToList();
            if(filteredOuterOrbs.Count < 2)
                return false;

            // Pick one at random
            outerOrbA = filteredOuterOrbs.GetRandomElement();
            filteredOuterOrbs.Remove(outerOrbA);

            // filter all orbs adjacent to the first pick
            OuterOrb firstPick = outerOrbA;
            filteredOuterOrbs = filteredOuterOrbs.Where(o => SameOrAdjacent(o, firstPick) == false).ToList();
            if(filteredOuterOrbs.Count == 0)
                return false;

            // Pick second outer orb
            outerOrbB = filteredOuterOrbs.GetRandomElement();

            // Pick random flying unpaused orb
            OrbMovement[] unpausedOrbs = _orbs.Where(o => o.IsPaused == false).ToArray();
            if(unpausedOrbs.Length == 0)
                return false;

            innerOrb = unpausedOrbs.GetRandomElement();
            return true;
        }

        private bool SameOrAdjacent(OuterOrb numOne, OuterOrb numTwo)
        {
            int indexOne = _outerOrbs.IndexOf(numOne);
            int indexTwo = _outerOrbs.IndexOf(numTwo);
            int dif = Mathf.Abs(indexTwo - indexOne);

            return dif <= 1 || dif == _outerOrbs.Count - 1;
        }

        // ----------------------------------------------------------------------------------------
    }
}