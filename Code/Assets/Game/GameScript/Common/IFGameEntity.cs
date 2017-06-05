using UnityEngine;
using System.Collections;

public class IFGameEntity : IFGameObject
{
	//阵营
	protected EU_CAMP camp = EU_CAMP.EU_NONE;
	public EU_CAMP Camp{get{return camp;}set{camp = value;}}

	protected bool show = false;
	public bool  Show{get{return show;}}

	//重置数据
	override public bool MyReset()
	{
		show = true;
		return base.MyReset();
	}
	
	//重置数据
	override public bool MyHide()
	{
		show = false;
		return base.MyHide();
	}

    
}
//==================================================================================================