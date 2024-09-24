using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BSA
{
	[System.Serializable]
	public class Options
	{
		// --- Fields -------------------------------------------------------------------------------------------------
		public int masterVolume = 5;
		public int musicVolume = 8;
		public int sfxVolume = 8;

		public bool useRumble = true;
		public bool beamsReflectOrbs = true;
		public bool bordersKillPlayers = true;

		// --- Properties ---------------------------------------------------------------------------------------------

		// --- Events -------------------------------------------------------------------------------------------------

		// --- Constructors -------------------------------------------------------------------------------------------
		public Options()
		{
			

		}

		// --- Interface implementations ------------------------------------------------------------------------------

		// --- Event callbacks ----------------------------------------------------------------------------------------

		// --- Public/Internal Methods --------------------------------------------------------------------------------

		// --- Protected/Private Methods ------------------------------------------------------------------------------
		
		// ----------------------------------------------------------------------------------------
	}
}