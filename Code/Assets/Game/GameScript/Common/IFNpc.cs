using UnityEngine.UI;
using UnityEngine;

public class IFNpc : IFActor
{
    /// <summary>
    /// 属性：hp atkRange searchRange atkFrequence atk
    /// </summary>
    /// 
    //float m_fTestTime = 0.0f;
    public float m_fHp = 100.0f;

    public float m_fAtk = 0.0f;

    public float m_fCurHP = 0.0f;

    protected float phyDefBite;//破甲

    protected float phyDef;//护甲

    protected float magDefBite;//法穿

    protected float magDef;//法抗

    protected float critic;//暴击

    protected float criticReduce;//韧性

    protected float doom;//命中

    protected float miss;//闪避

    protected float absorb;//吸血

    protected float damageBack;//反弹

    protected float reel;//眩晕

    protected float reelReduce;//抗晕

    protected int atkType;//攻击方式 魔法？物理？

    //搜索范围
    public float searchRange = 10f;

    //攻击范围
    public float atkRange = 2f;

    //攻击频率
    public float atkFrequency = 0.1f;

    protected float atkTimer = 0f;
    public float ATKTimer { get { return atkTimer; } set { atkTimer = value; } }

    //移动速度
    public float moveSpeed = 1.0f;

    private Animator m_anim;
    public Animator Anim
    {
        get { return m_anim; }
        set { m_anim = value; }
    }

    protected float mMoveSpeedBase = 0.0f;
    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    public int AtkType { get { return atkType; } set { atkType = value; } }
    public float PhyDefBite { get { return phyDefBite; } set { phyDefBite = value; } }//破甲
    public float PhyDef { get { return phyDef; } set { phyDef = value; } }//护甲
    public float MagDefBite { get { return magDefBite; } set { magDefBite = value; } }//法穿
    public float MagDef { get { return magDef; } set { magDef = value; } }//法抗
    public float Critic { get { return critic; } set { critic = value; } }//暴击（敌人没有抗暴击的话，超过500就必定出暴击了）
    public float CriticReduce { get { return criticReduce; } set { criticReduce = value; } }//韧性
    public float Doom { get { return doom; } set { doom = value; } }//命中
    public float Miss { get { return miss; } set { miss = value; } }//闪避
    public float Absorb { get { return absorb; } set { absorb = value; } }//吸血
    public float DamageBack { get { return damageBack; } set { damageBack = value; } }//反弹
    public float Reel { get { return reel; } set { reel = value; } }//眩晕
    public float ReelReduce { get { return reelReduce; } set { reelReduce = value; } }//抗晕
    public float CurHP
    {
        get { return m_fCurHP; }
        set { m_fCurHP = value; }
    }
    public float Atk
    {
        get { return m_fAtk; }
        set { m_fAtk = value; }
    }
    public float Hp
    {
        get { return m_fHp; }
        set { m_fHp = value; }
    }
    //////////////////////////////////////////////////////////////////////////

    //角色控制器
    protected CharacterController m_controller;
    public CharacterController _ActorControl { get { return m_controller; } }

    //移动方向
    protected Vector3 moveDir = Vector3.zero;
    public Vector3 MoveDir { get { return moveDir; } set { moveDir = value; } }

    IFNpc m_attackTarget = null;
    public IFNpc AttackTarget { set { m_attackTarget = value; } get { return m_attackTarget; } }
    public float AttackTargetDistSqr { get { return m_attackTarget != null ? (m_attackTarget.transform.position - transform.position).sqrMagnitude : -1f; } }

    private Vector3 atkDir = new Vector3();
    public Vector3 AttackTargetDir
    {
        get
        {
            if (m_attackTarget == null)
                return transform.forward;

            atkDir = m_attackTarget.transform.position - transform.position;
            atkDir.y = 0;
            atkDir.Normalize();
            return atkDir;
        }
    }

    protected bool itemNPC = false;
    public bool ItemNPC
    {
        get { return itemNPC; }
        set { itemNPC = value; }
    }

    //击退
    public float StopPower = 0f;

    protected float pushBackTimer = 0f;

    protected float pushBackPower = 0f;

    protected Vector3 pushBackDir = Vector3.zero;

    // NPC初始的位置和旋转，用于NPC复活时回到出生地
    protected Vector3 initPos;
    protected Quaternion initRot;
    protected Vector3 initScale;
    protected bool bInitPoseSet = false;

    //The shot sound.
    public Object dmgSound;
    public Object deadSound;
    public Object criticalHitSound;

    protected GameObject fireEffect = null;
    protected float fireDmgTimer = 0f;

    protected float onFireTime = 0f;
    public float OnFireTime
    {
        get { return onFireTime; }
        set
        {
            if (CurHP > 0f)
            {
                onFireTime = value;
                if (fireEffect == null)
                {
                    //fireEffect = IFGlobal.getActorEntity(IFGlobal.sLevel.PlyFire);
                }
            }
        }
    }

    public GameObject criticalHitFlash = null;

    public GameObject deadFlash = null;

    public void SetInitPose()
    {
        initPos = transform.position;
        initRot = transform.rotation;
        initScale = transform.localScale;
        bInitPoseSet = true;
    }

    public Slider hpSlider = null;

    //掉落物品
    public GameObject[] dropItems;

    public GameObject mDropWeapon = null;

    //成功掉落任一物品的概率,单位是百分比，默认概率是10%
    public int dropItemRatio = 10;

    public int mDropItemWeaponRatio = 10;

    public GameObject CalcDropItem()
    {
        if (mDropWeapon != null && Random.Range(0, 100) < mDropItemWeaponRatio)
        {
            return mDropWeapon;
        }

        if (dropItems.Length > 0 && Random.Range(0, 100) < dropItemRatio)
        {
            return dropItems[Random.Range(0, dropItems.Length)];
        }

        return null;
    }

    public Transform findTransform(Transform p, string tn)
    {
        if (p.gameObject.activeSelf)
        {
            if (p.gameObject.name == tn)
                return p;

            int gcc = p.childCount;
            for (int i = 0; i < gcc; ++i)
            {
                Transform ret = findTransform(p.GetChild(i), tn);
                if (ret != null)
                    return ret;
            }
        }

        return null;
    }

    override protected bool MyAwake()
    {
        m_controller = GetComponent<CharacterController>();

        mMoveSpeedBase = MoveSpeed;

        IFGlobal.npcLib.Add(this);

        return base.MyAwake();
    }

    override protected bool MyUpdate(float delta)
    {
        if (hpSlider && hpSlider.value != m_fCurHP)
        {
            hpSlider.value = m_fCurHP;
        }

        //身上着火伤害计算
        onFireTime -= delta;
        if (onFireTime <= 0f || CurHP <= 0f)
        {
            onFireTime = 0f;
            if (fireEffect != null)
            {
                IFGlobal.pushFreeEntity(fireEffect);
                fireEffect = null;
            }
        }
        else
        {
            fireDmgTimer -= delta;
            if (fireDmgTimer <= 0f)
            {
                fireDmgTimer = 1f;
                IFNpc killer = null;
                Vector3 tp = transform.position;
                float fdmg = Hp * 0.2f;
                if (fdmg > 1000f)
                    fdmg = 1000f;

                OnDamage(fdmg, ref killer, false, ref tp);
            }
        }

        if (fireEffect != null)
        {
            Vector3 fepos = transform.position;
            fepos.y += 1f;
            fireEffect.transform.position = fepos;
        }

        bool ret = base.MyUpdate(delta);

        if (_ActorControl && pushBackTimer > 0f && m_fCurHP > 0f)
        {
            pushBackTimer -= delta;
            Vector3 _movePos = pushBackDir * (pushBackPower * delta);
            _movePos.y = -1f;
            _ActorControl.Move(_movePos);
        }

        return ret;
    }

    //启动
    override protected bool MyStart()
    {
        bool ret = base.MyStart();

        return ret;
    }

    override public bool MyReset()
    {
        AttackTarget = null;
        m_fCurHP = m_fHp;
        if (hpSlider)
        {
            hpSlider.maxValue = m_fHp;
        }

        if (bInitPoseSet)
        {
            transform.position = initPos;
            transform.rotation = initRot;
            transform.localScale = initScale;
        }

        Anim = GetComponent<Animator>();
        return base.MyReset();
    }

    override public bool MyHide()
    {
        AttackTarget = null;
        return base.MyHide();
    }

    public float ATKFinal(ref IFNpc tgt, float coe)
    {
        float t = 1f;
        if (atkType == 1)
        {
            float x = phyDefBite - tgt.phyDef;
            if (x >= 0f)
                t = 1f + x / 100f;
            else
                t = 1f / (1f - x / 100f);
        }
        else if (atkType == 2)
        {
            float x = magDefBite - tgt.magDef;
            if (x >= 0f)
                t = 1f + x / 100f;
            else
                t = 1f / (1f - x / 100f);
        }

        float r = Random.Range(0.8f, 1.2f);
        int a = (int)(r * coe * t * Atk);
        if (a < 1)
            a = 1;

        return (float)a;
    }

    virtual public void SubtractHp(float dmgHp)
    {
        if (m_fCurHP > 0)
            m_fCurHP -= dmgHp;
    }

    //受伤回调
    virtual public bool OnDamage(float dmgHP, ref IFNpc killer, bool sound, ref Vector3 pos)
    {
        bool haveeffect = true;
        if (dmgHP < 0f)
        {
            dmgHP = -dmgHP;
            haveeffect = false;
        }

        Vector3 hitpos = transform.position;
        if (killer != null && haveeffect)
        {
            AttackTarget = killer;

            //眩晕
            float realcoe = 1f - Mathf.Abs(1f / (1f + (killer.reel - reelReduce) * 0.01f));
            if (realcoe > 0f && Random.Range(0f, 1f) <= realcoe)
            {
                if (IFGlobal.inCameraSpace(hitpos))
                {
                    //眩晕
                }
            }

            float tmpMiss = miss > 100 ? 100 : miss;
            float tmpdoom = killer.doom > 100 ? 100 : killer.doom;
            float m = 1f / (1f + (tmpMiss - tmpdoom) / 100f);
            if (m < 1f)
            {
                m *= 100f;
                if (Random.Range(0, 100) > (int)m)
                {
                    if (IFGlobal.inCameraSpace(hitpos))
                    {
                        //闪避

                    }
                    return true;
                }
            }
        }

        if (m_fCurHP > 0)
        {
            playEffectAction("hitFlash");
            float totdmg = 0f;
            totdmg += dmgHP;
            SubtractHp(dmgHP);

            if (killer != null && haveeffect)
            {
                float c = (killer.critic - criticReduce) / 500f;
                if (c > 1f || (c > 0f && Random.Range(0f, 1f) <= c))
                {
                    totdmg += dmgHP;
                    SubtractHp(dmgHP);
                    //暴击
                    IFLogger.Log("出暴击了");
                    playEffectAction("criticalHitFlash");
                    if (sound && criticalHitSound != null)
                    {
                        IFPlaySound _sound = IFGlobal.getActorEntity(criticalHitSound).GetComponent<IFPlaySound>();
                        if (_sound != null)
                            _sound.playSound(transform);
                    }
                }
            }

            if (totdmg > 0f && killer != null && killer.CurHP > 0f && haveeffect)
            {
                //吸血
                float abhp = killer.Absorb * totdmg / 10000f;
                if (abhp > 0f)
                {
                    if (abhp < 1f)
                        abhp = 1f;

                    killer.CurHP = killer.CurHP + abhp;
                    if (killer.CurHP > killer.Hp)
                        killer.CurHP = killer.Hp;

                    if (IFGlobal.inCameraSpace(hitpos))
                    {
                        //吸血恢复
                    }
                }

                //反弹
                float dbhp = damageBack * totdmg / 10000f;
                if (dbhp > 0f)
                {
                    if (dbhp < 1f)
                        dbhp = 1f;

                    IFNpc abkiller = this;
                    Vector3 abpos = abkiller.transform.position;
                    killer.OnDamage(-dbhp, ref abkiller, sound, ref abpos);
                }
            }

            if (m_fCurHP <= 0f)
            {
                m_fCurHP = 0f;
                if (sound)
                {
                    if (deadSound != null)
                    {
                        IFPlaySound _sound = IFGlobal.getActorEntity(deadSound).GetComponent<IFPlaySound>();
                        if (_sound != null)
                            _sound.playSound(transform);
                    }
                }

                OnDead();
            }
            else
            {
                if (sound && dmgSound != null)
                {
                    IFPlaySound _sound = IFGlobal.getActorEntity(dmgSound).GetComponent<IFPlaySound>();
                    if (_sound != null)
                        _sound.playSound(transform);
                }

                OnInjure();

                if (AttackTarget == null && killer != null)
                    AttackTarget = killer;

                if (killer != null && killer.StopPower > 0f)
                {
                    pushBackPower = killer.StopPower;
                    pushBackTimer = 0.1f;
                    pushBackDir = transform.position - killer.transform.position;
                    pushBackDir.Normalize();
                }
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    virtual protected void OnDead()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    virtual protected void OnInjure()
    {

    }

    public void SoundPlay(Object objSound)
    {
        if (objSound != null)
        {
            IFPlaySound _sound = IFGlobal.getActorEntity(objSound).GetComponent<IFPlaySound>();
            if (_sound != null)
                _sound.playSound(transform);
        }
    }
}