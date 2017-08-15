using UnityEngine;

namespace PixelCrushers.DialogueSystem.UFPS {

	/// <summary>
	/// Derived from vp_Interactable, this component makes the NPC interactable
	/// within the UFPS framework. When the player interacts with the NPC, this 
	/// component sends an OnUse message to the NPC. If the NPC has a Conversation
	/// Trigger configured to listen for OnUse, it will start a conversation.
	/// </summary>
	[AddComponentMenu("Dialogue System/Third Party/UFPS/Interactable Dialogue")]
	public class FPInteractableDialogue : vp_Interactable {
		
		private Texture savedCrosshair;
		
		public virtual void Awake() {
			savedCrosshair = m_InteractCrosshair;
		}

		/// <summary>
		/// Sends an OnUse message to this object. Add a Dialogue System Conversation Trigger to 
		/// the object that will listen for OnUse and start a conversation.
		/// </summary>
		public override bool TryInteract(vp_PlayerEventHandler player) {
			if (!enabled) return false;
			gameObject.SendMessage("OnUse", player.transform, SendMessageOptions.DontRequireReceiver);
			return true;
		}
		
		/// <summary>
		/// Sets the interactivity. Just enables or disables the dialogue crosshair texture.
		/// </summary>
		/// <param name="canInteract">If set to <c>true</c> can interact.</param>
		public void SetInteractivity(bool canInteract) {
			m_InteractCrosshair = canInteract ? savedCrosshair : null;
		}
		
	}
		
}