using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace PixelCrushers.DialogueSystem.UFPS {
	
	/// <summary>
	/// This script synchronizes the Dialogue System's Lua environment with the UFPS player 
	/// whenever a conversation starts or ends. To use it, add it to the player object.
	/// 
	/// It currently synchronizes: health and inventory.
	/// 
	/// Every weapon (e.g., "Pistol") has an associated Lua variable (e.g., 
	/// <c>Variable["Pistol"]</c>) that is equal to the count or ammo count (if it uses ammo)
	/// if the player has the weapon or <c>0</c> if the player doesn't. During conversations, 
	/// your dialogue entries can check and/or set the values using Lua conditions and scripts.
	/// </summary>
	[RequireComponent(typeof(FPPlayerLuaBridge))]
	[AddComponentMenu("Dialogue System/Third Party/UFPS/Sync Lua Player On Conversation")]
	public class FPSyncLuaPlayerOnConversation : MonoBehaviour {

		/// <summary>
		/// Set <c>true</c> to disable the player's weapon during conversations.
		/// </summary>
		public bool disableWeaponDuringConversations = true;

		private FPPlayerLuaBridge bridge = null;

		void Awake() {
			bridge = GetComponent<FPPlayerLuaBridge>();
		}
		
		public void OnConversationStart(Transform actor) {
			if (bridge == null) return;
			bridge.SyncFPToLua();
			if (disableWeaponDuringConversations) bridge.DeactivateCurrentWeapon();
		}

		public void OnConversationEnd(Transform actor) {
			if (bridge == null) return;
			bridge.SyncLuaToFP();
			if (disableWeaponDuringConversations) bridge.ActivateCurrentWeapon();
		}
		
	}

}
