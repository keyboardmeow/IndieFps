using UnityEngine;

public class IFWeapon : IFGameEntity
{
    //构造初始化数据
    override protected bool MyAwake()
    {

        return base.MyAwake();
    }

    //更新数据
    override protected bool MyUpdate(float delta)
    {
        return true;
    }

    virtual public bool fire(ref Vector3 inShotDir, Transform inPos, EU_CAMP inCamp, float ATKRange, float fSpeed, IFNpc owner, bool bIncreaseFireBulletNum = true)
    {
        return false;
    }

    virtual protected void OnWeaponFireEnd()
    {

    }

}
