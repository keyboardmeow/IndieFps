using UnityEngine;
using System.Collections;

public class IFEnemy : IFNpc
{

    // Use this for initialization
    override public bool MyReset()
    {
        Camp = EU_CAMP.EU_BAD;

        return base.MyReset();
    }

    // Update is called once per frame
    override protected bool MyUpdate(float delta)
    {

        return base.MyUpdate(delta);
    }

    override protected void OnDead()
    {
        gameObject.SetActive(false);
    }

    override protected void OnInjure()
    {
        
    }

}
