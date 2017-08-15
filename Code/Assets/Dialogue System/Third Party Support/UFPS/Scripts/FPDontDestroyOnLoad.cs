using UnityEngine;

namespace PixelCrushers.DialogueSystem.UFPS {

	/// <summary>
	/// Marks a GameObject as DontDestroyOnLoad.
	/// </summary>
	[AddComponentMenu("Dialogue System/Third Party/UFPS/Dont Destroy On Load")]
	public class FPDontDestroyOnLoad : MonoBehaviour {

		void Awake() {
			GameObject.DontDestroyOnLoad(gameObject);
		}
		
	}
		
}