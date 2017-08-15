using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem.UFPS {

	/// <summary>
	/// Performs various actions when the GameObject receives the "Die" message.
	/// </summary>
	[AddComponentMenu("Dialogue System/Third Party/UFPS/Dialogue System On Die")]
	public class DialogueSystemOnDie : MonoBehaviour {

		/// <summary>
		/// Set this <c>true</c> to destroy the object on "Die".
		/// </summary>
		public bool destroy = false;

		/// <summary>
		/// The sequence to play on "Die". The speaker is the Dialogue Manager object,
		/// and the listener is this object.
		/// </summary>
		public string sequence = string.Empty;

		/// <summary>
		/// The lua code to run on "Die".
		/// </summary>
		public string luaCode = string.Empty;

		/// <summary>
		/// The variable to record when the object dies. When the object is
		/// reloaded, if this variable is <c>true</c> then it will deactivate
		/// itself (or destroy itself if destroy is true).
		/// </summary>
		public string variableToRecordDeath = string.Empty;

		/// <summary>
		/// The variable to increment on "Die"
		/// </summary>
		public string variableToIncrement = string.Empty;

		/// <summary>
		/// The increment amount. To decrement, use a negative number.
		/// </summary>
		public int increment = 1;

		/// <summary>
		/// The minimum value.
		/// </summary>
		public int min = 0;

		/// <summary>
		/// The maximum value.
		/// </summary>
		public int max = 100;

		/// <summary>
		/// Run actions when the object receives the "Die" message.
		/// - Increment variableToIncrement
		/// - Record death in variableToRecordDeath
		/// - Destroy the object if specified
		/// </summary>
		public virtual void Die() {

			// Run Lua code:
			if (!string.IsNullOrEmpty(luaCode)) {
				Lua.Run(luaCode, DialogueDebug.LogInfo);
			}

			// Play sequence:
			if (!string.IsNullOrEmpty(sequence)) {
				DialogueManager.PlaySequence(sequence, DialogueManager.Instance.transform, transform);
			}

			// Increment the variableToIncrement:
			if (!string.IsNullOrEmpty(variableToIncrement)) {
				int oldValue = DialogueLua.GetVariable(variableToIncrement).AsInt;
				int newValue = Mathf.Clamp(oldValue + increment, min, max);
				DialogueLua.SetVariable(variableToIncrement, newValue);
				DialogueManager.Instance.BroadcastMessage("UpdateTracker", SendMessageOptions.DontRequireReceiver);
			}

			// Record that the object is dead in variableToRecordDeath:
			if (!string.IsNullOrEmpty(variableToRecordDeath)) {
				DialogueLua.SetVariable(variableToRecordDeath, true);
			}

			// Destroy the object if specified:
			if (destroy) {
				Destroy(gameObject);
			}
		}

		/// <summary>
		/// When applying persistent data, check the variable. If it's <c>true</c>,
		/// the GameObject has been destroyed previously, so destroy it now.
		/// </summary>
		public void OnApplyPersistentData() {
			if (!string.IsNullOrEmpty(variableToRecordDeath) && DialogueLua.GetVariable(variableToRecordDeath).AsBool) {
				if (destroy) {
					// Before destroying the object, make it think that the level is
					// being unloaded. This will disable any persistent data scripts
					// that use OnDestroy, since it's not really being destroyed
					// during gameplay in this case.
					gameObject.BroadcastMessage("OnLevelWillBeUnloaded", SendMessageOptions.DontRequireReceiver);
					Destroy(gameObject);
				} else {
					gameObject.SetActive(false);
				}
			}
		}
		
	}

}