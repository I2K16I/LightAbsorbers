using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BSA
{
	[CreateAssetMenu(fileName = "Settings", menuName = "Shock Absorbers/Settings", order = 10)]
	public class Settings : ScriptableObject
	{
		// --- Fields -------------------------------------------------------------------------------------------------
		[Header("Attack Settings")]
		[Tooltip("The duration of each attack, this does include the Beam Scale Up Duration. Meaning 6 Seconds Attack duration and 1 Seconds Scale up = 5 Seconds of actual attack duration. If the Beam Scale Duration and Wind Up Time exeed the attack duration they will be set to 1/8th of this value.")]
		[SerializeField] private float _beamActiveDuration = 6.0f;
		[Tooltip("This value decides how fast the beam builds up. A value over 1 Second is not adviced. If this Value and the Wind Up Time exeed the attack duration they will be set to 1/8th of the Attack Duration.")]
		[SerializeField] private float _beamScaleUpDuration = 1.0f;
		[Tooltip("This value is the time (s) between the beams scaling to the target length and them starting to dmg players. This gives players a chance to react. If the Beam Scale Duration and this value exeed the attack duration they will be set to 1/8th of the Attack Duration.")]
		[SerializeField] private float _beamWindUpTime = 1.0f;
		[Tooltip("The time between the start of an attack-wave and the start of the next wave of attacks.")]
		[SerializeField] private float _timeBetweenAttacks = 8.0f;
		[Tooltip("The amount of attacks the game start with.")]
		[SerializeField] private int _startAmountOfAttacks = 1;
		[Tooltip("This is the number of seconds before the number of attacks increases by 1. Example in the tooltip below.")]
		[SerializeField] private float _increaseAttacksAfter = 20f;
        [Tooltip("This is the number of seconds before the number of attacks increases by 1 again. Example: Start Amount of Attacks: 1 & Increase Attacks After: 20 & IncreaseAttacks Again After: 20 -> 0s = 1 Attack, 20s = 2 Attacks, 40s = 3 Attacks.")]
        [SerializeField] private float _increaseAttacksAgainAfter = 20f;

		[Header("Transition & Delays")]
        [Tooltip("This value determins how fast the game starts after all Players are ready")]
        [SerializeField] private float _startDelay = 1.5f;
        [Tooltip("This value controlls the length of the transition")]
        [SerializeField] private float _transitionTime = 1f;
        [Tooltip("This value represents the number of seconds before the players can start moving after the transition is done")]
        [SerializeField] private float _timeBetweenTransitionAndStart = 2f;
		[SerializeField] private float _playerDeathAnimationLength = 3f;

		[Header("Orb Settings")]
		[SerializeField] private int _numberOfOrbs = 8;
		[SerializeField] private float _orbStartSpeed = 1f;
		[SerializeField] private float _orbEndSpeed = 3f;
		[SerializeField] private float _timeTillEndSpeed = 60f;
		[Tooltip("The time between speed increses. 1 meaning once a second. Lower then 1 means more than once a second, highter means less than one time a second.")]
		[SerializeField] private float _updateFrequency = 1f;
        [SerializeField] private float _orbSize = 1f;

		[Header("Debug Settings")]
        [SerializeField] private bool _invincibleMode = false;

        // --- Properties ---------------------------------------------------------------------------------------------
		public float TimeBetweenAttacks => _timeBetweenAttacks; 
		public int StartAmountOfAttacks => _startAmountOfAttacks; 
		public float TimeUntilFirstIncrease => _increaseAttacksAfter; 
		public float TimeUntilSecondIncrease => _increaseAttacksAgainAfter;
		public float BeamScaleUpDuration => _beamScaleUpDuration;
		public float BeamWindUpTime => _beamWindUpTime;
        public float BeamActiveDuration => _beamActiveDuration;
		public float TotalBeamAttackDuration => _beamScaleUpDuration + _beamWindUpTime + _beamActiveDuration;


        public float StartDelay => _startDelay;
		public float TransitionTime => _transitionTime;
		public float TimeBetweenTransitionAndStart => _timeBetweenTransitionAndStart;
		public float PlayerDeathAnimationLength => _playerDeathAnimationLength;

		public int NumberOfOrbs => _numberOfOrbs;
		public float OrbStartSpeed => _orbStartSpeed;
		public float OrbEndSpeed => _orbEndSpeed;
		public float TimeTillEndSpeed => _timeTillEndSpeed;
		public float UpdateFrequency => _updateFrequency;
		public float OrbSize => _orbSize;

		public bool InvincibleMode => _invincibleMode;

		// --- Events -------------------------------------------------------------------------------------------------

		// --- Unity Functions ----------------------------------------------------------------------------------------
		private void OnEnable()
		{
			
		}		

		// --- Interface implementations ------------------------------------------------------------------------------

		// --- Event callbacks ----------------------------------------------------------------------------------------

		// --- Public/Internal Methods --------------------------------------------------------------------------------

		// --- Protected/Private Methods ------------------------------------------------------------------------------
		
		// ----------------------------------------------------------------------------------------
	}
}