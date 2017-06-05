using UnityEngine;

public class IFPlaySound : IFGameEntity {

	public AudioSource soundPlayer = null;
	public AudioClip[] sound;

	//生命周期
	public bool playSound(Transform inFather)
	{
		if(sound.Length > 0)
		{
			AudioClip inSound = sound [Random.Range (0, sound.Length)];
			if(inSound != null)
			{
				transform.parent = inFather;
				transform.localPosition = Vector3.zero;
                soundPlayer.rolloffMode = AudioRolloffMode.Linear;
				soundPlayer.clip = inSound;
				soundPlayer.Play ();
			}
		}

		return true;
	}
	
	//更新数据
	override protected bool MyUpdate(float delta)
	{
		if (soundPlayer.isPlaying == false)
		{
			IFGlobal.pushFreeEntity (gameObject);
			return false;
		}
		return base.MyUpdate(delta);
	}

	//重置数据
	override public bool MyHide()
	{
		transform.parent = null;
		soundPlayer.clip = null;
		return base.MyHide();
	}
}
