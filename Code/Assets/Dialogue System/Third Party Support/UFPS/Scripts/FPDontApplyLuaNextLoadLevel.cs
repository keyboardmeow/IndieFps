using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.UFPS;

namespace PixelCrushers.DialogueSystem.UFPS {

	/// <summary>
	/// Add this script to your scene to re-tick
	/// FPPersistentPlayerData.DontApplyLuaNextLoadLevel.
	/// </summary>
	[AddComponentMenu("Dialogue System/Third Party/UFPS/Don't Apply Lua Next Load Level")]
	public class FPDontApplyLuaNextLoadLevel : MonoBehaviour {

		IEnumerator Start () {
			yield return null;
			yield return null;
			TickDontApplyLuaNextLoadLevel();
		}
		
		public void TickDontApplyLuaNextLoadLevel() {
			FPSyncLuaPlayerOnLoadLevel sync = DialogueManager.Instance.GetComponent<FPSyncLuaPlayerOnLoadLevel>();
			if (sync != null) {
				sync.dontApplyLuaNextLoadLevel = true;
			}
		}
	}

}
