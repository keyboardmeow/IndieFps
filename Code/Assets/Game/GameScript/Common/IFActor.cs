using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EU_CAMP
{
    EU_NONE,
    EU_GOOD,
    EU_BAD
}

public class IFActor : IFGameEntity
{
    //自身的对象
    protected IFActor m_mySelf;

    //行为列表
    protected Dictionary<string, IFActorAction> m_ActionLib = new Dictionary<string, IFActorAction>();
    virtual protected void InitActionLib()
    {
        if (m_ActionLib.Count != 0)
            return;

        IFActorAction[] ActionList = gameObject.GetComponents<IFActorAction>();
        for (int idx = 0; idx < ActionList.Length; ++idx)
        {
            ActionList[idx].initActionName();
            if ( !m_ActionLib.ContainsKey(ActionList[idx].actionName))
            {
                m_ActionLib.Add(ActionList[idx].actionName, ActionList[idx]);
            }
        }
    }

    //当前行为
    protected IFActorAction m_cunAction;
    public IFActorAction CurAction { get { return m_cunAction; } }
    public string curAction;
    protected string mPauseAction = "";
    protected bool mLockAction = false;
    public bool LockAction
    {
        set { mLockAction = value; }
        get { return mLockAction; }
    }

    // 播放动作，内部接口
    protected IFActorAction _getAction(string actionName)
    {
        InitActionLib();

        IFActorAction act = null;
        if (m_ActionLib.Count > 0 && m_ActionLib.ContainsKey(actionName) == true)
        {
            act = m_ActionLib[actionName];
        }

        return act;
    }

    //切换当前action
    public IFActorAction playAction(string actionName)
    {
        if (mLockAction == true)
            return null;

        if (m_cunAction != null && m_cunAction.actionName.Equals(actionName))
            return null;

        stopAction();

        m_cunAction = _getAction(actionName);
        if (m_cunAction != null)
        {
            m_cunAction.playAction(ref m_mySelf);
        }
        curAction = actionName;

        return m_cunAction;
    }

    //停止当前 action
    public void stopAction()
    {
        if (m_cunAction != null)
        {
            m_cunAction.stopAction(ref m_mySelf);
            m_cunAction = null;
        }
    }

    // 暂停当前action
    public void pauseActtion()
    {
        mPauseAction = curAction;
    }

    // 恢复action
    public void resumeAction()
    {
        if (mPauseAction.Length > 0)
        {
            playAction(mPauseAction);
        }
    }

    public IFActorAction findAction(string actionName)
    {
        InitActionLib();
        return m_ActionLib[actionName];
    }

    protected List<IFActorAction> m_multiActionList = new List<IFActorAction>();


    //切换当前特效action
    public void pushEffectAction(IFActorAction inAction)
    {
        if (inAction.isRuning == true)
            return;

        inAction.playAction(ref m_mySelf);
        m_multiActionList.Add(inAction);
    }

    //播放特效action
    public IFActorAction playEffectAction(string actionName)
    {
        InitActionLib();

        IFActorAction act = _getAction(actionName);
        if (act != null)
        {
            if (act.isRuning == false)
            {
                act.playAction(ref m_mySelf);
                m_multiActionList.Add(act);
            }
        }

        return act;
    }

    //停止所有特效 action
    public void stopEffectActionAll()
    {
        for (int idx = 0; idx < m_multiActionList.Count; ++idx)
        {
            if (m_multiActionList[idx].isRuning == true)
                m_multiActionList[idx].stopAction(ref m_mySelf);
        }
        m_multiActionList.Clear();
    }

    //停止某个特效 action
    public void stopEffectAction(string actionName)
    {
        for (int idx = 0; idx < m_multiActionList.Count;)
        {
            if (m_multiActionList[idx].actionName.Equals(actionName) == true)
            {
                m_multiActionList[idx].stopAction(ref m_mySelf);
                m_multiActionList.RemoveAt(idx);
                // 没有break,因为要停止所有同名的action
            }
            else
            {
                ++idx;
            }
        }
    }

    //特效action run循环
    protected void updateEffectAction(float delta)
    {
        for (int idx = 0; idx < m_multiActionList.Count;)
        {
            if (m_multiActionList[idx].isRuning == false)
            {
                m_multiActionList.RemoveAt(idx);
                continue;
            }
            m_multiActionList[idx].runAction(delta, ref m_mySelf);
            ++idx;
        }
    }

    override protected bool MyAwake()
    {
        m_mySelf = this;
        return base.MyAwake();
    }

    override protected bool MyStart()
    {
        //InitActionLib ();
        //m_cunAction = null;
        return base.MyStart();
    }

    override protected bool MyUpdate(float delta)
    {
        if (m_cunAction != null)
            m_cunAction.runAction(delta, ref m_mySelf);

        updateEffectAction(delta);
        return base.MyUpdate(delta);
    }

    override public bool MyReset()
    {
        gameObject.SetActive(true);
        return base.MyReset();
    }

    override public bool MyHide()
    {
        stopAction();
        gameObject.SetActive(false);
        return base.MyHide();
    }
}
