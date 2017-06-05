using UnityEngine;

public class IFGameObject : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        MyStart();
    }


    // Update is called once per frame
    void Update()
    {
        MyUpdate(Time.deltaTime);
    }

    //初始化
    void Awake()
    {
        MyAwake();
    }

    void LateUpdate()
    {
        MyLateUpdate();
    }

    //构造初始化数据
    virtual protected bool MyAwake()
    {
        return true;
    }

    //对象启动时前
    virtual protected bool MyStart()
    {
        MyReset();
        return true;
    }

    //更新数据
    virtual protected bool MyUpdate(float delta)
    {
        return true;
    }

    //重置数据
    virtual public bool MyReset()
    {
        return true;
    }

    //重置数据
    virtual public bool MyHide()
    {
        return true;
    }
    //销毁数据
    virtual public bool MyDestroy()
    {
        Destroy(this.gameObject);
        return true;
    }

    virtual public void MyLateUpdate()
    {

    }
}
