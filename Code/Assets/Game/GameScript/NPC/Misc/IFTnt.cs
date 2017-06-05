using UnityEngine;
using System.Collections;

public class IFTnt : IFNpc
{
    // Use this for initialization
    override public bool MyReset()
    {
        return base.MyReset();
    }

    // Update is called once per frame
    override protected bool MyUpdate(float delta)
    {

        return base.MyUpdate(delta);
    }

    override protected void OnDead()
    {
        
    }

    override protected void OnInjure()
    {
        
    }

}
