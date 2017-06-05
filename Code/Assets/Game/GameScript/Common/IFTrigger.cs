using UnityEngine;
using System.Collections;

public class IFTrigger : MonoBehaviour
{
	public GameObject nextTrigger;

	protected bool activeFlag = false;
	public bool ActiveFlag{get{return activeFlag;}}

    [HideInInspector]
    public IFCheckPoint checkPoint;

    public bool haveNextTrigger()
	{
		return (nextTrigger == null);
	}

	// Use this for initialization
	void Start ()
	{
		for(int i=0; i<gameObject.transform.childCount; ++i)
		{
			Transform npc = gameObject.transform.GetChild(i);
			npc.gameObject.SetActive(false);
			IFNpc an = npc.GetComponent<IFNpc>();
            if (an != null)
            {
                an.Camp = EU_CAMP.EU_BAD;
                an.SetInitPose();
            }
		}

        if (nextTrigger != null)
        {
            nextTrigger.SetActive(false);
            IFTrigger tri = nextTrigger.GetComponent<IFTrigger>();
            if (tri != null)
                tri.checkPoint = checkPoint;
        }
            
	}
    void Update()
    {
        if (activeFlag)
        {
            Active();
        }
        else
        {
            bool bTriggerfinish = true;
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform npc = transform.GetChild(i);
                if (npc.gameObject.activeSelf)
                {
                    bTriggerfinish = false;
                }
            }

            if (bTriggerfinish)
            {
                gameObject.SetActive(false);

                if (nextTrigger)
                {
                    nextTrigger.SetActive(true);
                    checkPoint.CurTrigger = nextTrigger.GetComponent<IFTrigger>();
                    checkPoint.CurTrigger.ResetNpc();
                }
            }
        }
    }

    public void ResetNpc()
    {
        for (int i = 0; i < gameObject.transform.childCount; ++i)
        {
            Transform npc = gameObject.transform.GetChild(i);
            npc.gameObject.SetActive(true);
            IFNpc n = npc.gameObject.GetComponent<IFNpc>();
            n.MyReset();
        }

        activeFlag = true;
        Start();
    }

    /// <summary>
    /// 玩家触发Trigger
    /// </summary>
    /// <param name="other"></param>
	void OnTriggerEnter(Collider other)
	{
		if (activeFlag)
		{
            if (other != null)
            {
                IFGameEntity entity = other.gameObject.GetComponent<IFGameEntity>();
                IFPlayer ply = entity as IFPlayer;
                if (ply == IFGlobal.sLevel.CurPlayer)
                {
                    Active();
                }
            }
		}
	}

    public void Active()
    {
        //Close this trigger.
        activeFlag = false;

        // 出生怪物
        for (int i = 0; i < gameObject.transform.childCount; ++i)
        {
            Transform npc = gameObject.transform.GetChild(i);
            SpawnNpc( ref npc );
        }
    }

    public static void SpawnNpc(ref Transform npc, bool playAction = true, string initAction = "", bool bAdjustLSCCAtckRange = true)
    {
        npc.gameObject.SetActive(true);
        IFNpc n = npc.gameObject.GetComponent<IFNpc>();
        n.MyReset();
        n.enabled = true;
        IFGlobal.npcLib.Add(n);
        CharacterController cc = npc.gameObject.GetComponent<CharacterController>();
        if ( cc )
        {
            cc.enabled = true;
        }
    }
}
