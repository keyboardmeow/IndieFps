using UnityEngine;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.UnityGUI;

namespace PixelCrushers.DialogueSystem.UFPS {
	
	/// <summary>
	/// This script provides a rudimentary main menu.
	/// </summary>
	public class ExampleMainMenu : MonoBehaviour {
		
		public GUISkin guiSkin;
		public QuestLogWindow questLogWindow;

		private bool isMenuOpen = false;
		private Rect windowRect = new Rect(0, 0, 500, 550);
		private ScaledRect scaledRect = ScaledRect.FromOrigin(ScaledRectAlignment.MiddleCenter, ScaledValue.FromPixelValue(300), ScaledValue.FromPixelValue(370));
		
		void Start() {
			DialogueManager.ShowAlert("Press Escape for Menu");
		}
		
		void Update() {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				if (!DialogueManager.IsConversationActive && !IsQuestLogOpen()) {
					SetMenuStatus(!isMenuOpen);
				}
			}
		}

		public void OpenMenu() {
			Debug.Log("Dialogue System Example OpenMenu Requested");
			if (!isMenuOpen && !DialogueManager.IsConversationActive && !IsQuestLogOpen()) {
				SetMenuStatus(true);
			}
		}
		
		void OnGUI() {
			if (isMenuOpen && !IsQuestLogOpen()) {
				if (guiSkin != null) {
					GUI.skin = guiSkin;
				}
				windowRect = GUI.Window(0, windowRect, WindowFunction, "Menu");
			}
		}
		
		private void WindowFunction(int windowID) {
			if (GUI.Button(new Rect(10, 60, windowRect.width - 20, 48), "Quest Log")) {
				// [Keep main menu open underneath quest log:] SetMenuStatus(false);
				OpenQuestLog();
			}
			if (GUI.Button(new Rect(10, 110, windowRect.width - 20, 48), "Save Game")) {
				SetMenuStatus(false);
				SaveGame();
			}
			if (GUI.Button(new Rect(10, 160, windowRect.width - 20, 48), "Load Game")) {
				SetMenuStatus(false);
				LoadGame();
			}
			if (GUI.Button(new Rect(10, 210, windowRect.width - 20, 48), "Clear Saved Game")) {
				SetMenuStatus(false);
				ClearSavedGame();
			}
			if (GUI.Button(new Rect(10, 260, windowRect.width - 20, 48), "Restart Game")) {
				SetMenuStatus(false);
				RestartGame();
			}
			if (GUI.Button(new Rect(10, 310, windowRect.width - 20, 48), "Close Menu")) {
				SetMenuStatus(false);
			}
		}
		
		private void SetMenuStatus(bool open) {
			isMenuOpen = open;
			if (open) {
				windowRect = scaledRect.GetPixelRect();
				FreezePlayer();
			} else {
				UnfreezePlayer();
			}
			Time.timeScale = open ? 0 : 1;
		}

		private void FreezePlayer() {
            FPFreezePlayer fpFreezePlayer = FindObjectOfType<FPFreezePlayer>();
            if (fpFreezePlayer == null) {
                Debug.LogWarning("Can't find an FPFreezePlayer component to freeze the player!");
            } else {
                fpFreezePlayer.Freeze();
            }
        }

		private void UnfreezePlayer() {
            FPFreezePlayer fpFreezePlayer = FindObjectOfType<FPFreezePlayer>();
            if (fpFreezePlayer == null) {
                Debug.LogWarning("Can't find an FPFreezePlayer component to unfreeze the player!");
            } else {
                fpFreezePlayer.Unfreeze();
            }
		}
		
		private bool IsQuestLogOpen() {
			return (questLogWindow != null) && questLogWindow.IsOpen;
		}
		
		private void OpenQuestLog() {
			if ((questLogWindow != null) && !IsQuestLogOpen()) {
				questLogWindow.Open();
			}
		}
		
		private void SaveGame() {
			string saveData = PersistentDataManager.GetSaveData();
			PlayerPrefs.SetString("SavedGame", saveData);
			Debug.Log("Save Game Data: " + saveData);
			DialogueManager.ShowAlert("Game Saved to PlayerPrefs");
		}
	
		private void LoadGame() {
			if (PlayerPrefs.HasKey("SavedGame")) {
				string saveData = PlayerPrefs.GetString("SavedGame");
				Debug.Log("Load Game Data: " + saveData);
				LevelManager levelManager = GetComponentInChildren<LevelManager>();
				if (levelManager != null) {
					levelManager.LoadGame(saveData);
				} else {
					PersistentDataManager.ApplySaveData(saveData);
				}
				DialogueManager.ShowAlert("Game Loaded from PlayerPrefs");
			} else {
				DialogueManager.ShowAlert("Save a game first");
			}
		}
			
		private void ClearSavedGame() {
			if (PlayerPrefs.HasKey("SavedGame")) {
				PlayerPrefs.DeleteKey("SavedGame");
				Debug.Log("Cleared saved game data");
			}
			DialogueManager.ShowAlert("Saved Game Cleared From PlayerPrefs");
		}

		private void RestartGame() {
			Debug.Log("Restarting game");
			FPSyncLuaPlayerOnLoadLevel syncOnLoad = DialogueManager.Instance.GetComponent<FPSyncLuaPlayerOnLoadLevel>();
			if (syncOnLoad != null) syncOnLoad.dontApplyLuaNextLoadLevel = true;
			PersistentDataManager.LevelWillBeUnloaded();
			DialogueManager.ResetDatabase(DatabaseResetOptions.RevertToDefault);
			Tools.LoadLevel(0);

		}
		
	}

}
