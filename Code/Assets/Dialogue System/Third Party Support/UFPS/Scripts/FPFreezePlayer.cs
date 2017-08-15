using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem.UFPS {
	
	/// <summary>
	/// This component provides methods to freeze and unfreeze the UFPS player. 
	/// It also freezes the player during conversations.
	/// </summary>
	[AddComponentMenu("Dialogue System/Third Party/UFPS/Freeze Player")]
	public class FPFreezePlayer : MonoBehaviour {
		
        [Tooltip("Hide the UFPS HUD when freezing player")]
		public bool hideHUD = true;

        [Tooltip("Freeze the player during conversations involving the player")]
		public bool freezeDuringConversations = true;

        [Tooltip("When the scene starts, lock the cursor to enable mouse look without having to click in the window first")]
        public bool lockCursorOnStart = true;
		
		// If you don't assign these properties, the Awake() method will automatically assign them.
		public vp_FPController fpController = null;
		public vp_FPCamera fpCamera = null;
		public vp_FPPlayerEventHandler fpPlayerEventHandler = null;
		public vp_FPInput fpInput = null;
		public vp_SimpleHUD fpHUD = null;
		public MonoBehaviour fpCrosshair = null;
		
		private bool wasCrosshairVisible;
		private bool wasCursorLocked;
		
		void Awake() {
			if (fpController == null) fpController = GetComponentInChildren<vp_FPController>();
			if (fpCamera == null) fpCamera = GetComponentInChildren<vp_FPCamera>();
			if (fpPlayerEventHandler == null) fpPlayerEventHandler = GetComponentInChildren<vp_FPPlayerEventHandler>();
			if (fpInput == null) fpInput = GetComponentInChildren<vp_FPInput>();
			if (fpHUD == null) fpHUD = GetComponentInChildren<vp_SimpleHUD>();
			if (fpCrosshair == null) fpCrosshair = GetComponentInChildren<vp_SimpleCrosshair>();
		}

        IEnumerator Start() {
            yield return null;
            if (lockCursorOnStart) vp_Utility.LockCursor = true;
        }

        /// <summary>
        /// When a conversation starts, freeze the UFPS player (i.e., gameplay) and show the cursor.
        /// </summary>
        /// <param name='actor'>
        /// Actor participating in the conversation.
        /// </param>
        public void OnConversationStart(Transform actor) {
			if (freezeDuringConversations) Freeze();
		}
		
		/// <summary>
		/// When a conversation ends, unfreeze the UFPS player (i.e., gameplay) and restore the 
		/// previous cursor state.
		/// </summary>
		/// <param name='actor'>
		/// Actor participating in the conversation.
		/// </param>
		public void OnConversationEnd(Transform actor) {
			if (freezeDuringConversations) Unfreeze();
		}
		
		/// <summary>
		/// Freeze the UFPS player (i.e., gameplay) and show the cursor.
		/// </summary>
		public void Freeze() {
			wasCursorLocked = vp_Utility.LockCursor;
			wasCrosshairVisible = (fpCrosshair != null) && fpCrosshair.enabled;
			if (fpCrosshair != null) fpCrosshair.enabled = false;
			if (fpController != null) fpController.Stop();
			if (fpPlayerEventHandler != null) {
				fpPlayerEventHandler.Attack.TryStop();
				fpPlayerEventHandler.InputAllowGameplay.Set(false);
			}
			if (fpCamera != null) fpCamera.SetState("Freeze", true);
			if (fpInput != null) fpInput.MouseCursorForced = true;
			if (hideHUD && (fpHUD != null)) fpHUD.enabled = false;
			StartCoroutine(ShowCursorAfterOneFrame());
		}
		
		/// <summary>
		/// Unfreeze the UFPS player (i.e. gameplay) and restore the previous cursor state.
		/// </summary>
		public void Unfreeze() {
			if ((fpCrosshair != null) && wasCrosshairVisible) fpCrosshair.enabled = true;
			if (fpPlayerEventHandler != null) fpPlayerEventHandler.InputAllowGameplay.Set(true);
			if (fpCamera != null) fpCamera.SetState("Freeze", false);
			if (fpInput != null) fpInput.MouseCursorForced = false;
			if (hideHUD && (fpHUD != null)) fpHUD.enabled = true;
			vp_Utility.LockCursor = wasCursorLocked;
		}
		
		/// <summary>
		/// Shows the cursor after one frame. We wait one frame to allow UFPS to do any closeout
		/// that might try to regain cursor control.
		/// </summary>
		/// <returns>
		/// <c>null</c> (coroutine).
		/// </returns>
		private IEnumerator ShowCursorAfterOneFrame() {
			yield return null;
			vp_Utility.LockCursor = false;
		}

	}
	
}