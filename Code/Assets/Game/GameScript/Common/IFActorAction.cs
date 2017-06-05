using UnityEngine;
using System.Collections;

public class IFActorAction : IFGameObject
{
    public string actionName = "auto";
    protected float timer;
    public float Timer { get { return timer; } }

    protected bool runing = false;
    public bool isRuning { get { return runing; } set { runing = value; } }

    virtual public void initActionName()
    {

    }

    override protected bool MyStart()
    {
        enabled = false;
        return base.MyStart();
    }

    override protected bool MyAwake()
    {
        initAction();
        return base.MyAwake();
    }

    virtual public bool initAction()
    {
        return true;
    }

    virtual public bool playAction(ref IFActor inActor)
    {
        runing = true;
        timer = 0f;
        this.enabled = true;
        return true;
    }

    virtual public bool stopAction(ref IFActor inActor)
    {
        this.enabled = false;
        runing = false;
        return true;
    }

    virtual public bool runAction(float delta, ref IFActor inActor)
    {
        timer += delta;
        return true;
    }

    override public void MyLateUpdate()
    {

    }
}
