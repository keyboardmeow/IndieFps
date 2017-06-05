using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public enum ELevelState
{
    LS_Invalid,//无效状态
    LS_LevelInfo,//关卡信息展示
    LS_LevelShop,//关卡商店
    LS_LevelPlaying,//关卡游玩中
    LS_LevelFail,//关卡失败
    LS_LevelSuccess,//关卡成功
}

public class IFLevel : IFActor
{
    /// <summary>
    /// 关卡状态，除了LevelState之外不要在外部直接调用eLevelState，包括IFLevel类本身
    /// </summary>
    protected ELevelState eLevelState = ELevelState.LS_Invalid;
    public ELevelState LevelState
    {
        get
        {
            return eLevelState;
        }

        set
        {
            if (eLevelState != value)
            {
                eLevelState = value;

                if(IFUIManager._Instance)
                {
                    switch (eLevelState)
                    {
                        case ELevelState.LS_LevelInfo:
                            IFUIManager._Instance.ChangePanel(UIPanelState.UIPS_LevelInfoPanel);
                            break;
                        case ELevelState.LS_LevelShop:
                            IFUIManager._Instance.ChangePanel(UIPanelState.UIPS_ShopPanel);
                            break;
                        case ELevelState.LS_LevelPlaying:
                            IFUIManager._Instance.ChangePanel(UIPanelState.UIPS_NoPanel);
                            break;
                        case ELevelState.LS_LevelFail:
                            IFUIManager._Instance.ChangePanel(UIPanelState.UIPS_FailPanel);
                            break;
                        case ELevelState.LS_LevelSuccess:
                            IFUIManager._Instance.ChangePanel(UIPanelState.UIPS_SettlePanel);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// CheckPoint
    /// </summary>
    public IFCheckPoint firstCheckPoint;

    protected IFCheckPoint curCheckPoint;
    public IFCheckPoint CurCheckPoint { get { return curCheckPoint; } set { curCheckPoint = value; } }

    /// <summary>
    /// Player
    /// </summary>
    public IFPlayer CurPlayer = null;

    /// <summary>
    /// 内存清理定时器
    /// </summary>
    private float m_fClearTimer = 0f;

    /// <summary>
    /// 敌人能力系数
    /// </summary>
    public float EnemyHPBase = 1f;

    public float EnemyATKBase = 1f;

    public float EnemySpeedBase = 1f;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    override protected bool MyAwake()
    {
        IFGlobal.sLevel = this;

        LevelState = ELevelState.LS_LevelPlaying;

        initLevelProperties();

        return base.MyAwake();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    override public bool MyDestroy()
    {
        IFGlobal.sLevel = null;
        LevelState = ELevelState.LS_Invalid;
        return base.MyDestroy();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="delta"></param>
    /// <returns></returns>
    override protected bool MyUpdate(float delta)
    {
        switch (LevelState)
        {
            case ELevelState.LS_LevelInfo:
                onLevelInfo();
                break;
            case ELevelState.LS_LevelShop:
                onLevelShop();
                break;
            case ELevelState.LS_LevelPlaying:
                onLevelPlaying();
                break;
            case ELevelState.LS_LevelFail:
                onLevelFail();
                break;
            case ELevelState.LS_LevelSuccess:
                onLevelSuccess();
                break;
        }

        //垃圾回收
        tickClear(delta);
        return base.MyUpdate(delta);
    }

    /// <summary>
    /// 
    /// </summary>
    private void onLevelInfo()
    {
        
    }
    /// <summary>
    /// 
    /// </summary>
    public void onLevelShop()
    {
        
    }
    /// <summary>
    /// 
    /// </summary>
    private void onLevelPlaying()
    {
        bool bCheckPointAllFinish = false;
        if (CurCheckPoint)
        {
            if (!CurCheckPoint.ActiveFlag)
            {
                if (CurCheckPoint.nextCheckPoint)
                    CurCheckPoint.nextCheckPoint.GetComponent<IFCheckPoint>().startCheckPoint();
                else
                    bCheckPointAllFinish = true;
            }
        }
        else
        {
            if (firstCheckPoint)
            {
                firstCheckPoint.startCheckPoint();
            }
        }

        CheckIsGameFinish(bCheckPointAllFinish);
    }
    /// <summary>
    /// 
    /// </summary>
    private void onLevelFail()
    {
        
    }
    /// <summary>
    /// 
    /// </summary>
    private void onLevelSuccess()
    {
        
    }

    private void CheckIsGameFinish( bool bCheckPointAllFinish )
    {
        if (CurPlayer.CurHP <= 0)
        {
            LevelState = ELevelState.LS_LevelFail;
            return;
        }
    }

    /// <summary>
    /// 垃圾回收
    /// </summary>
    /// <param name="delta"></param>
    protected void tickClear(float delta)
    {
        IFGlobal.tickAutoDestroyObject(delta);

        m_fClearTimer += Time.deltaTime;
        if (m_fClearTimer > 60.0f)
        {
            GC.Collect();
            m_fClearTimer = 0.0f;
        }
    }

    /// <summary>
    /// 初始化玩家的属性
    /// </summary>
    protected void initLevelProperties()
    {

    }

}
