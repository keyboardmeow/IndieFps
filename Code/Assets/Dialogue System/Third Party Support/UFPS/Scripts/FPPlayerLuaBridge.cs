using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.UFPS {
	
	/// <summary>
	/// This is a utility component to help synchronize the Dialogue System's Lua environment with the
	/// UFPS player. It synchronizes health and inventory.
	/// </summary>
	[System.Serializable]
	[AddComponentMenu("Dialogue System/Third Party/UFPS/Player Lua Bridge")]
	public class FPPlayerLuaBridge : MonoBehaviour {

		/// <summary>
		/// The usable item types. Assign all item types that the player can possibly use here.
		/// These item types will be synchronized with Lua.
		/// </summary>
		public List<vp_ItemType> usableItemTypes = new List<vp_ItemType>();

		// References to UFPS components:
		public vp_FPPlayerEventHandler fpPlayer { get; private set; }
		public vp_Inventory fpInventory { get; private set; }
		public vp_FPWeaponHandler fpWeaponHandler { get; private set; }

		// Specifies whether to force wield the weapon (play its wield animation)
		// when restoring from the saved Lua state:
		public bool forceWield { get; set; }

		// Cached values for Lua data lookup:
		private string luaActorName = null;
		private string luaActorIndex = null;

		// Set this true to log debug info:
		public bool debug = false;

		public virtual void Awake() {
			fpPlayer = GetComponentInChildren<vp_FPPlayerEventHandler>();
			fpInventory = GetComponentInChildren<vp_Inventory>();
			fpWeaponHandler = GetComponentInChildren<vp_FPWeaponHandler>();
			forceWield = false;
		}

		public virtual void Start() {
			GetUsableItemTypes();
		}
		
		/// <summary>
		/// Deactivates the current weapon. Used when starting a conversation.
		/// </summary>
		public virtual void DeactivateCurrentWeapon() {
			if ((fpWeaponHandler != null) && (fpWeaponHandler.CurrentWeapon != null)) fpWeaponHandler.CurrentWeapon.Deactivate();
		}
		
		/// <summary>
		/// Activates the current weapon. Used when ending a conversation.
		/// </summary>
		public virtual void ActivateCurrentWeapon() {
			if ((fpWeaponHandler != null) && (fpWeaponHandler.CurrentWeapon != null)) fpWeaponHandler.CurrentWeapon.Activate();
		}
		
		/// <summary>
		/// Records the UFPS player's data into the Dialogue System's Lua environment.
		/// Records the player's health and inventory (weapons and items).
		/// </summary>
		public virtual void SyncFPToLua() {
			SyncFPStatsToLua();
			SyncFPInventoryToLua();
			SyncFPWeaponHandlerToLua();
		}
		
		/// <summary>
		/// Updates the UFPS player's data with the current values in the Dialogue System's
		/// Lua environment. Updates the player's health and inventory (weapons and items).
		/// Also uses fpPlayer.Register() to hook into CanStart_Weapon() to prevent UFPS
		/// from playing equip/load animations if the inventory has changed.
		/// </summary>
		public virtual void SyncLuaToFP() {
			SyncLuaToFPStats();
			fpPlayer.Register(this);
			try {
				SyncLuaToFPInventory();
			} finally {
				fpPlayer.Unregister(this);
			}
			SyncLuaToFPWeaponHandler();
		}

		/// <summary>
		/// Syncs UFPS's player stats to Lua.
		/// </summary>
		public void SyncFPStatsToLua() {
			if (fpPlayer != null) {
				SetPlayerField("Health", fpPlayer.Health.Get());
			}
		}

		/// <summary>
		/// Syncs Lua to UFPS's player stats.
		/// </summary>
		public void SyncLuaToFPStats() {
			if (fpPlayer != null) {
				float health = GetPlayerField("Health").AsFloat;
				if (debug) Debug.Log(string.Format("fpPlayer.Health.Set({0})", health));
				fpPlayer.Health.Set(health);
			}
		}

		/// <summary>
		/// Syncs the UFPS inventory to Lua.
		/// </summary>
		public void SyncFPInventoryToLua() {
			if (fpInventory != null) {

				// Set inventory space variables:
				SetPlayerField("TotalSpace", fpInventory.TotalSpace);
				SetPlayerField("UsedSpace", fpInventory.UsedSpace);
				SetPlayerField("RemainingSpace", fpInventory.RemainingSpace);

				// Set values of all usable item types:
				foreach (vp_ItemType itemType in usableItemTypes) {
					if (itemType != null) {
						int count = (itemType is vp_UnitType) ? fpInventory.GetUnitCount(itemType as vp_UnitType) : fpInventory.GetItemCount(itemType);
						DialogueLua.SetVariable(itemType.name, count);
						if (itemType is vp_UnitBankType) {
							foreach (var unitBankInstance in fpInventory.UnitBankInstances) {
								if (unitBankInstance.Type == itemType) {
									DialogueLua.SetVariable(itemType.name + "_Units", unitBankInstance.Count);
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Syncs Lua to the UFPS inventory.
		/// </summary>
		public void SyncLuaToFPInventory() {
			if (fpInventory != null) {

				// Update UFPS inventory if any counts have changed:
				foreach (vp_ItemType itemType in usableItemTypes) {
					if (itemType != null) {
						int newCount = DialogueLua.GetVariable(itemType.name).AsInt;
						int oldCount = fpInventory.GetItemCount(itemType);
						if (debug) Debug.Log(string.Format("Item: {0} oldCount={1} newCount={2}", itemType.name, oldCount, newCount));
						if (itemType is vp_UnitBankType) {

							// Unit Bank (a loadable weapon such as a pistol):
							if (debug) Debug.Log("    is UnitBankType");
							vp_UnitBankType unitBankType = itemType as vp_UnitBankType;
							int unitsLoaded = DialogueLua.GetVariable(itemType.name + "_Units").AsInt;
							if (newCount < oldCount) { // Remove unit banks.
								if (debug) Debug.Log(string.Format("    TryRemoveUnitBanks({0}, {1}", unitBankType.name, oldCount - newCount));
								fpInventory.TryRemoveUnitBanks(unitBankType, oldCount - newCount);
 							} else if (newCount > oldCount) { // Add unit banks.
								for (int i = 0; i < newCount - oldCount; i++) {
									if (debug) Debug.Log(string.Format("    TryGiveUnitBank({0}, 0, 0)", unitBankType.name));
									if (debug) Debug.Log(string.Format("    TryGiveUnitBank({0}, {1}, 0)", unitBankType.name, unitsLoaded));
									fpInventory.TryGiveUnitBank(unitBankType, unitsLoaded, 0);
								}
							} else {
								foreach (var unitBankInstance in fpInventory.UnitBankInstances) {
									if (unitBankInstance.Type == itemType) {
										unitBankInstance.Count = unitsLoaded;
									}
								}
							}

						} else if (itemType is vp_UnitType) {

							// Unit Type (ammo):
							if (debug) Debug.Log("    is UnitType (ignored; will set internal unit bank)");
							vp_UnitType unitType = itemType as vp_UnitType;
							if (debug) Debug.Log(string.Format("    TrySetUnitCount({0}, {1})", unitType.name, newCount));
							fpInventory.TrySetUnitCount(unitType, newCount);

						} else {

							// Item Type (ammo-less weapon such as a mace):
							if (debug) Debug.Log("    is ItemType");
							if (newCount < oldCount) { // Remove items.
								if (debug) Debug.Log(string.Format("    TryRemoveItems({0}, {1})", itemType.name, oldCount - newCount));
								fpInventory.TryRemoveItems(itemType, oldCount - newCount);
							} else if (newCount > oldCount) { // Add items.
								if (debug) Debug.Log(string.Format("    TryGiveItems({0}, {1})", itemType.name, newCount - oldCount));
								fpInventory.TryGiveItems(itemType, newCount - oldCount);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Syncs the player's current weapon index to Lua.
		/// </summary>
		public void SyncFPWeaponHandlerToLua() {
			if (fpWeaponHandler == null) return;
			SetPlayerField("CurrentWeaponIndex", fpWeaponHandler.CurrentWeaponIndex);
		}

		/// <summary>
		/// Syncs the current weapon index stored in Lua to be the player's current weapon.
		/// Since the bridge has temporarily disabled equip/load animations, we can set the 
		/// weapon immediately. (In an earlier version of this script that lacked the code
		/// to disable equip/load animations, we invoked SetWeapon() after 0.5s.)
		/// </summary>
		public void SyncLuaToFPWeaponHandler() {
			if (fpWeaponHandler == null) return;
			int currentWeaponIndex = GetPlayerField("CurrentWeaponIndex").AsInt;
			fpWeaponHandler.ReloadAutomatically = false;
			//--- Was: fpWeaponHandler.SetWeapon(currentWeaponIndex);
			fpPlayer.SetWeapon.TryStart(currentWeaponIndex);
			if (forceWield && (fpWeaponHandler.CurrentWeapon != null)) {
				fpWeaponHandler.CurrentWeapon.GetComponent<vp_FPWeapon>().Wield(true);
			}
			Invoke("EnableReloadAutomatically", 0.5f);
		}

		/// <summary>
		/// Enables automatic reloads. Called after a short time to allow the weapon handler's
		/// OnStop_SetWeapon routine to finish first.
		/// </summary>
		private void EnableReloadAutomatically() {
			if (fpWeaponHandler == null) return;
			fpWeaponHandler.ReloadAutomatically = true;	
		}

		/// <summary>
		/// The bridge temporarily registers this method with UFPS when applying the Lua
		/// inventory data back into UFPS. It prevents UFPS from playing equip/load animations.
		/// </summary>
		/// <returns><c>false</c>.</returns>
		protected virtual bool CanStart_SetWeapon() {
			return false;
		}
		
		/// <summary>
		/// Gets the name of the player in the dialogue database
		/// </summary>
		/// <value>The name of the player in the dialogue database.</value>
		public string LuaPlayerName {
			get {
				if (string.IsNullOrEmpty(luaActorName)) {
					foreach (Actor actor in DialogueManager.MasterDatabase.actors) {
						if (actor.IsPlayer) {
							luaActorName = actor.Name;
							if (string.IsNullOrEmpty(luaActorName)) luaActorName = "Player";
							break;
						}
					}
				}
				return luaActorName;
			}
		}

		/// <summary>
		/// Gets the string index of the player in the <c>Actor[]</c> table
		/// </summary>
		/// <value>The string index for Lua.</value>
		public string LuaPlayerIndex {
			get {
				if (string.IsNullOrEmpty(luaActorIndex)) {
					luaActorIndex = DialogueLua.StringToTableIndex(LuaPlayerName);
				}
				return luaActorIndex;
			}
		}

		/// <summary>
		/// Sets a player field in the Actor table.
		/// </summary>
		/// <param name="fieldName">Field name.</param>
		/// <param name="value">Value.</param>
		public void SetPlayerField(string fieldName, object value) {
			if (debug) Debug.Log(string.Format("SetPlayerField({0}, {1}", fieldName, value));
			DialogueLua.SetActorField(LuaPlayerIndex, fieldName, value);
		}

		/// <summary>
		/// Gets a player field in the Actor table.
		/// </summary>
		/// <returns>The player field.</returns>
		/// <param name="fieldName">Field name.</param>
		public Lua.Result GetPlayerField(string fieldName) {
			Lua.Result value = DialogueLua.GetActorField(LuaPlayerIndex, fieldName);
			if (debug) Debug.Log(string.Format("GetPlayerField({0}, {1}) returned [{2}]", LuaPlayerIndex, fieldName, value));
			return value;
		}

		/// <summary>
		/// Adds any missing items from the inventory's item caps into the usable
		/// item type list, in case the developer forgot to add them manually.
		/// </summary>
		private void GetUsableItemTypes() {
			if (fpInventory == null) return;
			foreach (var itemCap in fpInventory.m_ItemCapInstances) {
				if (!usableItemTypes.Contains(itemCap.Type)) {
					usableItemTypes.Add(itemCap.Type);
				}
			}
		}

	}

}
