using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem.UFPS {

	/// <summary>
	/// This component works with the PersistentDataManager and UFPS to keep track
	/// of a UFPS player's position, health, and inventory when saving and loading
	/// games. To use it, add it to the player object.
	/// </summary>
	[RequireComponent(typeof(FPPlayerLuaBridge))]
	[AddComponentMenu("Dialogue System/Third Party/UFPS/Persistent Player Data")]
	public class FPPersistentPlayerData : MonoBehaviour {
		
		/// <summary>
		/// (Optional) Normally, this component uses the game object's name as the name of the 
		/// actor in the Lua Actor[] table. If your actor is named differently in the Lua Actor[] 
		/// table (e.g., the actor has a different name in Chat Mapper or the DialogueDatabase), 
		/// then set this property to the Lua name.
		/// </summary>
		public string overrideActorName = "Player";

		/// <summary>
		/// Set <c>true</c> to record the player's position when saving persistent data, or
		/// <c>false</c> to skip it.
		/// </summary>
		public bool recordPosition = true;

		/// <summary>
		/// Set <c>true</c> to force the player to wield the weapon after loading a game.
		/// Some custom weapons won't aim properly unless they've played the wield animation.
		/// </summary>
		public bool forceWield = true;

		private FPPlayerLuaBridge bridge;
		private vp_FPController fpController = null;
		private vp_FPCamera fpCamera = null;
		
		private string ActorName { 
			get { return string.IsNullOrEmpty(overrideActorName) ? gameObject.name : overrideActorName; } 
		}
		
		void Awake() {
			bridge = GetComponent<FPPlayerLuaBridge>();
			fpController = GetComponentInChildren<vp_FPController>();
			fpCamera = GetComponentInChildren<vp_FPCamera>();
		}

		/// <summary>
		/// Listens for the OnRecordPersistentData message and records the game object's position 
		/// and rotation into the Lua Actor[] table.
		/// </summary>
		public void OnRecordPersistentData() {
			if (bridge != null) bridge.SyncFPToLua();
			if (recordPosition) RecordPositionToLua();
		}
		
		/// <summary>
		/// Listens for the OnApplyPersistentData message and retrieves the game object's position 
		/// and rotation from the Lua Actor[] table.
		/// </summary>
		public void OnApplyPersistentData() {
			if (bridge != null) {
				bridge.forceWield = forceWield;
				bridge.SyncLuaToFP();
			}
			if (recordPosition) ApplyLuaToPosition();
		}
		
		private void RecordPositionToLua() {
			Lua.Run(string.Format("Actor[\"{0}\"].Position = \"{1}\"", DialogueLua.StringToTableIndex(ActorName), GetPositionString()), DialogueDebug.LogInfo);
		}
		
		private void ApplyLuaToPosition() {
			string s = Lua.Run(string.Format("return Actor[\"{0}\"].Position", DialogueLua.StringToTableIndex(ActorName)), DialogueDebug.LogInfo).AsString;
			ApplyPositionString(s);
		}
		
		private string GetPositionString() {
			return string.Format("{0:N4},{1:N4},{2:N4},{3:N4},{4:N4},{5:N4},{6:N4}", 
				transform.position.x, transform.position.y, transform.position.z,
				transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
		}
		
		private void ApplyPositionString(string s) {
			if (string.IsNullOrEmpty(s) || s.Equals("nil")) return;
			string[] tokens = s.Split(',');
			if (tokens.Length != 7) return;
			float[] values = new float[7];
			for (int i = 0; i < 7; i++) {
				values[i] = 0;
				float.TryParse(tokens[i], out values[i]);
			}
			Vector3 pos = new Vector3(values[0], values[1], values[2]);
			Quaternion rot = new Quaternion(values[3], values[4], values[5], values[6]);
			if (fpController != null) fpController.SetPosition(pos);
			if (fpCamera != null) fpCamera.SetRotation(rot.eulerAngles);
		}
		
	}

}
