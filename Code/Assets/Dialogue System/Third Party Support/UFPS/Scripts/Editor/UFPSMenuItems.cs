using UnityEngine;
using UnityEditor;

namespace PixelCrushers.DialogueSystem.UFPS
{

    public static class UFPSMenuItems
    {

        [MenuItem("Component/Dialogue System/Third Party/UFPS/Add All Player Scripts")]
        public static void AddAllPlayerScripts()
        {
            var fpPlayer = GameObject.FindObjectOfType<vp_FPPlayerEventHandler>();
            if (fpPlayer == null)
            {
                Debug.LogError("Dialogue System: Can't find a UFPS player (vp_FPPlayerEventHandler) in the scene.");
            }
            else
            {
                if (fpPlayer.GetComponent<FPPlayerLuaBridge>() == null)
                {
                    fpPlayer.gameObject.AddComponent<FPPlayerLuaBridge>();
                }
                if (fpPlayer.GetComponent<FPPersistentPlayerData>() == null)
                {
                    fpPlayer.gameObject.AddComponent<FPPersistentPlayerData>();
                }
                if (fpPlayer.GetComponent<FPFreezePlayer>() == null)
                {
                    fpPlayer.gameObject.AddComponent<FPFreezePlayer>();
                }
                if (fpPlayer.GetComponent<FPSyncLuaPlayerOnConversation>() == null)
                {
                    fpPlayer.gameObject.AddComponent<FPSyncLuaPlayerOnConversation>();
                }
                if (fpPlayer.GetComponent<ShowCursorOnConversation>() == null)
                {
                    fpPlayer.gameObject.AddComponent<ShowCursorOnConversation>();
                }
                var camera = fpPlayer.GetComponentInChildren<Camera>();
                if ((camera != null) && (camera.GetComponent<GUILayer>() == null))
                {
                    camera.gameObject.AddComponent<GUILayer>();
                }
                Debug.Log("Dialogue System: Added integration scripts to UFPS player.", fpPlayer);
            }
        }

        [MenuItem("Window/Dialogue System/Tools/UFPS/Add All Player Scripts")]
        public static void AddAllPlayerScriptsAlternate()
        {
            AddAllPlayerScripts();
        }

    }

}
