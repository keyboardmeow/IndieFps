using UnityEngine;
using System.Collections;

public class IFCheckPoint : MonoBehaviour
{
    public IFLevel level = null;

    static float finishTimer = 0f;

    private float finishDelayTime = 0.0f;

    public GameObject nextCheckPoint = null;

    public GameObject  startTrigger = null;

    protected IFTrigger curTrigger = null;
    public IFTrigger CurTrigger{get{return curTrigger;}set{curTrigger = value;}}

    protected bool activeFlag = false;
    public bool ActiveFlag
    {
        get { return activeFlag; }
        set { activeFlag = value; }
    }

    //初始化
    void Awake()
	{
		finishTimer = 0f;
	}

	// Use this for initialization
	void Start ()
	{
		gameObject.SetActive (false);
        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform trigger = transform.GetChild(i);
            if (trigger.gameObject.activeSelf)
            {
                trigger.gameObject.SetActive(false);
            }
        }

        if (startTrigger != null)
		{
            IFTrigger tri = startTrigger.GetComponent<IFTrigger>();
            if( tri != null )
                tri.checkPoint = this;
		}
	}
	
	// Update is called once per frame
	void Update()
	{
        if (ActiveFlag)
        {
            bool checkpointfinish = true;
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform npc = transform.GetChild(i);
                if (npc.gameObject.activeSelf)
                {
                    checkpointfinish = false;
                    break;
                }
            }

            if (checkpointfinish)
            {
                finishTimer += Time.deltaTime;
                if (finishTimer > finishDelayTime)
                {
                    ActiveFlag = false;
                }
            }
        }
    }

    virtual public bool startCheckPoint()
    {
        IFGlobal.sLevel.CurCheckPoint = this;
        ActiveFlag = true;
        gameObject.SetActive(true);
        if (startTrigger != null)
        {
            startTrigger.SetActive(true);
            curTrigger = startTrigger.GetComponent<IFTrigger>();
            curTrigger.ResetNpc();
        }

        return true;
    }

    /// <summary>
    /// 杀死当前CheckPoint下所有Trigger的怪物
    /// </summary>
    /// <param name="killer"></param>
    public void killAllEnemy(ref IFNpc killer)
    {
        if (ActiveFlag)
        {
            IFTrigger ttri = startTrigger.GetComponent<IFTrigger>();
            do
            {
                if (ttri != null)
                {
                    for (int i = 0; i < ttri.transform.childCount; ++i)
                    {
                        Transform npc = ttri.transform.GetChild(i);
                        IFNpc n = npc.gameObject.GetComponent<IFNpc>();
                        if (n!= null)
                        {
                            Vector3 pos = Vector3.zero;
                            n.OnDamage(n.CurHP, ref killer, true, ref pos);
                        }
                    }
                }

                if (ttri.nextTrigger != null)
                    ttri = ttri.nextTrigger.GetComponent<IFTrigger>();
                else
                    ttri = null;

            } while (ttri != null);
        }
    }
}
