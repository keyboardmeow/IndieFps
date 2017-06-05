using UnityEngine;

// 游戏场景中，定时释放的由对象池创建的游戏对象
public class IFDestroyObjTimer
{
    public float mTimer = 0;
    public GameObject mGameObj;
    public IFDestroyObjTimer(GameObject go, float times)
    {
        mTimer = times;
        mGameObj = go;
    }
}
