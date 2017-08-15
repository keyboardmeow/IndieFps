using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem.UFPS {

	/// <summary>
	/// This script changes to a different level, saving the Dialogue System's
	/// data before changing.
	/// </summary>
	[AddComponentMenu("Dialogue System/Third Party/UFPS/Change Level Trigger")]
	public class ChangeLevelTrigger : MonoBehaviour {

		public string newLevelName;

		public void OnTriggerEnter(Collider other) {
			if (other.CompareTag("Player")) {
				string savegame = PixelCrushers.DialogueSystem.PersistentDataManager.GetSaveData();
				Debug.Log ("Recording: " + savegame);
				Tools.LoadLevel(newLevelName);
			}
		}
	}

}
