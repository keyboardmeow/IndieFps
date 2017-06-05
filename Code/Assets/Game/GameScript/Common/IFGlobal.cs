using UnityEngine;
using System.Collections.Generic;

public class IFGlobal:System.Object
{
    static public float g_PI360 = 0.0174533f;
	static public float G_PI360{get{return g_PI360;}}
	static public float g_2PI = Mathf.PI*2f;
	static public float G_2PI{get{return g_2PI;}}
	static public IFLevel sLevel = null;

    static private SystemLanguage g_Language = SystemLanguage.English;
    static public SystemLanguage G_Language { get { return g_Language; } set { g_Language = value; } }

    static public bool inCameraSpace(Vector3 inPos)
	{
		if (Camera.main == null)return false;
		Vector3 _inCameraSpacePos = Camera.main.WorldToViewportPoint (inPos);
		if(_inCameraSpacePos.z >0f && _inCameraSpacePos.x >0f && _inCameraSpacePos.x <1f && _inCameraSpacePos.y >0f && _inCameraSpacePos.y <1f)
			return true;

		return false;
	}

    /// <summary>
    /// 游戏资源对象池
    /// </summary>
    static public Dictionary<string ,IFFreeLib> g_staticGameData = new Dictionary<string ,IFFreeLib>();
    // 当前所有的NPC
	public static List<IFNpc> npcLib = new List<IFNpc>();
    // 当前所有的可交互物件
    public static List<IFNpc> interActiveLib = new List<IFNpc>();
    // 定时释放的由对象池创建的对象
    public static List<IFDestroyObjTimer> autoDestroyObj = new List<IFDestroyObjTimer>();

	//清除休眠的对象
	public static void clearAllActor()
	{
		g_staticGameData.Clear ();
        interActiveLib.Clear();
		npcLib.Clear ();
        for (int i = 0; i < autoDestroyObj.Count; ++i)
        {
            pushFreeEntity(autoDestroyObj[i].mGameObj);
        }
        autoDestroyObj.Clear();
	}

    //获得一个空闲的对象
    public static GameObject getActorEntity(UnityEngine.Object Prefab)
	{
		GameObject rv = null;
		IFFreeLib _freeLib = null;
        if (IFGlobal.g_staticGameData.Count > 0)
        {
            if (IFGlobal.g_staticGameData.TryGetValue(Prefab.name, out _freeLib) == true)
                if (_freeLib.freeActorLib.Count > 0)
                    rv = _freeLib.freeActorLib.Dequeue();
        }

		if (rv == null)
		{
			rv = (GameObject)GameObject.Instantiate(Prefab, Vector3.zero, Quaternion.identity);
			rv.name = Prefab.name;
		}

		rv.SetActive(true);
		IFGameObject go = rv.GetComponent<IFGameObject> ();
		if (go != null)
			go.MyReset();

		return rv;
	}
	
	//传入一个空闲的对象
	public static void pushFreeEntity(GameObject inActor)
	{
		inActor.SetActive(false);

		IFGameObject _GEntity = inActor.GetComponent<IFGameObject> ();
		if (_GEntity != null)
			_GEntity.MyHide ();

		IFFreeLib _freeLib = null;
		if (IFGlobal.g_staticGameData.TryGetValue (inActor.name, out _freeLib) == false)
		{
			_freeLib = new IFFreeLib();
			IFGlobal.g_staticGameData.Add(inActor.name, _freeLib);
		}
		_freeLib.freeActorLib.Enqueue (inActor);
	}

	static public IFNpc findItemNPC(EU_CAMP camp, Transform src, float inAqrRange)
	{
		float bestdist = 100000000f;
		IFNpc bestnpc = null;
		
		for (int i=0; i<npcLib.Count; ++i)
		{
			IFNpc npc = npcLib[i] as IFNpc;
			if(npc.ItemNPC && npc.CurHP > 0f && npc.Camp != camp)
			{
				float ds = (npc.transform.position - src.position).sqrMagnitude;
				if(ds < inAqrRange && ds < bestdist)
				{
					bestnpc = npc;
					bestdist = ds;
				}
			}
		}

		return bestnpc;
	}

	static public IFNpc findEnemyTarget(EU_CAMP camp, Transform src, float inAqrRange)
	{
		float bestdist = 100000000f;
		IFNpc bestnpc = null;
		
		for (int i=0; i<npcLib.Count; ++i)
		{
			IFNpc npc = npcLib[i];
			if(npc && npc.CurHP > 0f && npc.Camp != camp && !npc.ItemNPC)//
			{
                float ds = (npc.transform.position - src.position).sqrMagnitude;
                if (ds < inAqrRange && ds < bestdist)
                {
                    bestnpc = npc;
                    bestdist = ds;
                }
			}
		}
		
		return bestnpc;
	}

	static public IFNpc findTarget(EU_CAMP camp, Transform src, float inAqrRange)
	{
		float bestdist = 100000000f;
		IFNpc bestnpc = null;

		for (int i=0; i<npcLib.Count; ++i)
		{
			IFNpc npc = npcLib[i];
            if (npc && npc.CurHP > 0f && npc.Camp != camp)//!npc.itemNPC
			{
                float ds = (npc.transform.position - src.position).sqrMagnitude;                    
				if(ds < inAqrRange * inAqrRange && ds < bestdist)
				{
					bestnpc = npc;
					bestdist = ds;
				}
			}
		}

		return bestnpc;
	}

	static public bool haveTarget(EU_CAMP camp)
	{
		for (int i=0; i<npcLib.Count; ++i)
		{
			IFNpc npc = npcLib[i];
            if (!npc.ItemNPC && npc.Camp != camp && npc.CurHP > 0f)
			{
				return true;
			}
		}
		
		return false;
	}

    static public void tickAutoDestroyObject( float delta )
    {
        for (int i = 0; i < autoDestroyObj.Count; )
        {
            autoDestroyObj[i].mTimer -= delta;
            if (autoDestroyObj[i].mTimer <= 0)
            {
                pushFreeEntity(autoDestroyObj[i].mGameObj);
                autoDestroyObj.RemoveAt(i);
            }
            else
            {
                ++i;
            }
        }
    }

    // 移除一个对象
    static public void removeNpc(ref IFActor npc)
    {
        for (int i = 0; i < npcLib.Count; )
        {
            IFNpc n = npcLib[i];
            if (n == npc)
            {
                npcLib.RemoveAt(i);
            }
            else
            {
                if (n.AttackTarget == npc)
                    n.AttackTarget = null;
                ++i;
            }
        }
    }

    public static string GetPlayerPrefabPath(string prefabName)
    {
        string path = "Prefab/Player/prefab_";

        path += prefabName;
        return path;
    }

    public static string GetEffectPrefabPath(string prefabName)
    {
        string path = "Prefab/Effect/prefab_";

        path += prefabName;
        return path;
    }

    public static string GetWeaponPrefabPath(string prefabName)
    {
        string path = "Prefab/Weapon/prefab_";

        path += prefabName;
        return path;
    }

    public static string GetEnemyPrefabPath(string prefabName)
    {
        string path = "Prefab/Enemy/prefab_";

        path += prefabName;
        return path;
    }
    public static string GetScenePrefabPath(string prefabName)
    {
        string path = "Prefab/Scene/prefab_";

        path += prefabName;
        return path;
    }
}