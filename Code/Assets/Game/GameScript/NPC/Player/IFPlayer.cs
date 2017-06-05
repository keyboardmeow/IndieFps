using System.Collections.Generic;
using UnityEngine;

public class IFPlayer : IFNpc
{
    public List<IFWeapon> mWeapons = new List<IFWeapon>();

    private IFWeapon m_CurWeapon = null;

    public IFWeapon CurWeapon
    {
        get { return m_CurWeapon; }
        set { m_CurWeapon = value; }
    }

    override protected bool MyAwake()
    {
        base.MyAwake();
        Camp = EU_CAMP.EU_GOOD;
        CurHP = Hp;

        return base.MyAwake();
    }

    //更新
    override protected bool MyUpdate(float delta)
    {
        return base.MyUpdate(delta);
    }

    override public bool MyReset()
    {
        base.MyReset();
        return true;
    }
}