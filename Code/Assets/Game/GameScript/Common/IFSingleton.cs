using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class IFSingletonRoot : MonoBehaviour
{
	private static GameObject singletonRoot = null;
	public static GameObject GetSingletonRootObj()
	{
		if (singletonRoot == null)
		{
			singletonRoot = new GameObject("_SingletonRoot");
			DontDestroyOnLoad(singletonRoot);
		}
		return singletonRoot;
	}
}

/// <summary>
/// 继承MonoBehavior的组件式单件类
/// </summary>
public class IFSingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T mInstance;
    private static bool mApplicationIsQuitting = false;
	private static object mLock = new object();


    public static T Instance
    {
        get
        {
            if (mApplicationIsQuitting)
            {
#if UNITY_EDITOR
                IFLogger.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed on application quit." + " Won't create again - returning null.");
#endif
                return null;
            }

            lock (mLock)
            {
                if (mInstance == null)
                {
                    mInstance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        IFLogger.LogError("[Singleton] Something went really wrong " + " - there should never be more than 1 singleton!" + " Reopenning the scene might fix it.");
                        return mInstance;
                    }

                    if (mInstance == null)
                    {
                        GameObject singleton = new GameObject();

                        mInstance = singleton.AddComponent<T>();

                        singleton.name = "(Singleton)_" + typeof(T).ToString();

                        //DontDestroyOnLoad(singleton);

#if UNITY_EDITOR
                        IFLogger.Log("[Singleton] Instance of " + typeof(T) + " is needed, so '" + singleton + "' was created with DontDestroyOnLoad.");
#endif
                    }
                    else
                    {
#if UNITY_EDITOR
                        IFLogger.Log("[Singleton] Using instance already created: " + mInstance.gameObject.name);
#endif
                    }
                }
                return mInstance;
            }
        }
    }

    public static void CreateInstane()
    {
        if (Instance == null)
            Debug.LogError("Create Timer Instance Failed!");
    }

	protected virtual void Awake()
	{
		if (!mInstance) 
			mInstance = this as T;

        transform.parent = IFSingletonRoot.GetSingletonRootObj().transform;
	}

	/// <summary>
	/// 当Unity退出时，会随机回收对象。但是这个过程中如果仍有任何脚本对象调用，就会再创建一个对象。
	/// 为了避免这种情况，用mApplicationIsQuitting来控制对象的创建过程。
	/// </summary>
	protected virtual void OnDestroy()
	{
		mApplicationIsQuitting = true;
		mInstance = null;
	}

}

public class IFSingleton<T> where T : class, new()
{
    private static T mInstance;
    private static object mLock = new object();

    public static T Instance
    {
        get
        {
            lock (mLock)
            {
                if (mInstance == null)
                    mInstance = new T();
                return mInstance;
            }
        }
    }
}