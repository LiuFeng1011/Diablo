using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public enum enCharacterProperty
{
    non,

    life = 1 ,//生命
    liferevive = 2,  /*  生命回复    */

    atkForce = 3,//攻击力

    atkSpeed = 4,    /*  攻击速度  */
    moveSpeed = 5,   /*  移动速度  */

    comborate = 6,   /*  暴击几率    */
    comboval = 7,    /*  暴击伤害    */
    armor = 8,   /*  护甲  */
    avoid = 9,   /*  闪避  */
    returnhurt = 10,  /*  反弹伤害    */
    steallife = 11,   /*  偷取生命    */
    golddrop = 12,    /*  金币掉率加成  */
    equipdrop = 13,   /*  魔法物品掉率  */

    lifeAddition = 15,   /*  生命加成  */
    armorAddition = 16,   /*  护甲加成  */

    atkForceAddition = 19,      /*  攻击力加成  */
    maxval,
}

[System.Serializable]
public class CharacterProperty{
    
    public float[] propertyValues = new float[(int)enCharacterProperty.maxval];

    public void SetProperty(enCharacterProperty type, float val){
        propertyValues[(int)type] = val;
    }

    public void AdditionProperty(enCharacterProperty type, float val)
    {
        propertyValues[(int)type] =+ val;
    }

    public float GetProperty(enCharacterProperty type){
        return propertyValues[(int)type];
    }

    public void Log(){
        List<PropertyConf> confs = ConfigManager.propertyConfManager.datas;
        string propertyString = "";
        for (int i = 0; i < confs.Count; i++)
        {
            PropertyConf conf = confs[i];

            propertyString += string.Format(conf.des, propertyValues[conf.id]);
            propertyString += "\n";
        }
        Debug.Log(propertyString);
    }

    public void PropertyAddition(enCharacterProperty target,enCharacterProperty source){

        propertyValues[(int)target] +=
            propertyValues[(int)target] *
            propertyValues[(int)source] * 0.01f;
        
    }
}

public class InGameBaseCharacter : InGameBaseObj
{

    public enum AnimatorState
    {
        Idle = 0,
        Run,
        Attack,
        Dead,
        Walk,
        Skill
    }

    RoleData data;

    public BaseActionManager actionManager = null;

    public Animator anim;

    BaseEnemyAI ai;

    //==================property======================
    public int level;
    [HideInInspector] public CharacterProperty propertys;
    public float life = 10;

    protected CharacterConf conf;

    public string charactername = "";

    public Vector3 boxSize;

    Vector3 lastPos = Vector3.zero;

    List<EquipData> equipList = new List<EquipData>();

    //生命回复时间
    float lifeReviveTime = 0;
    float lifeReviveCount = 0f;

    public override void Init(int instanceId, int confid, enMSCamp camp)
    {
        base.Init(instanceId, confid, camp);

        EventManager.Register(this,
                              EventID.EVENT_GAME_CHARACTER_HURT);

        propertys = new CharacterProperty();

        conf = ConfigManager.characterConfManager.dic[confid];

        //名字
        charactername = conf.name;

        //AI管理器
        actionManager = new BaseActionManager();
        actionManager.Init(this, Vector3.zero);

        //动画管理器
        anim = this.GetComponent<Animator>();

        //血条高度
        boxSize = transform.GetComponent<BoxCollider>().size * transform.localScale.x;

        //注册到UI界面
        InGameManager.GetInstance().inGameUIManager.AddRole(this);

        this.level = conf.level;
        ResetAllProperty(true);

        lastPos = transform.position;
    }

    public override enObjType GetObjType()
    {
        return enObjType.character;

    }

    public virtual void SetData(RoleData data)
    {
        charactername = data.name;

        this.data = data;

        this.ResetAllProperty();
    }

    public RoleData GetData()
    {
        return data;
    }

    public void InitProperty()
    {
        life = propertys.GetProperty(enCharacterProperty.life);//生命
    }

    public virtual void SetBaseProperty()
    {
        //从配置表中获得人物基础属性 
        for (int i = 1; i < propertys.propertyValues.Length; i++)
        {
            if (!ConfigManager.propertyConfManager.dataMap.ContainsKey(i))
            {
                continue;
            }
            PropertyConf pconf = ConfigManager.propertyConfManager.dataMap[i];
            if (pconf.formula != 2)
            {
                if (pconf.formula == 1)
                {
                    propertys.propertyValues[i] = (1f - conf.propertyValues[i] / 100f);
                }
                else
                {
                    propertys.propertyValues[i] = conf.propertyValues[i];
                }
            }
            else
            {
                propertys.propertyValues[i] = 0;
            }
        }

    }

    public virtual void SetEquipProperty(){

        for (int i = 0; i < equipList.Count; i ++){
            EquipData equipdata = equipList[i];
            //EquipConf econf = ConfigManager.equipConfManager.dic[equipdata.equipid];
            for (int j = 0; j < equipdata.propertyList.Count; j++)
            {
                EquipProperty p = equipdata.propertyList[j];
                PropertyConf pconf = ConfigManager.propertyConfManager.dataMap[p.id];

                string propertyText = string.Format(pconf.des, p.val) + "\n";
                if (pconf.formula != 2)
                {
                    if (pconf.formula == 1)
                    {
                        propertys.propertyValues[p.id] *= (1f - p.val / 100f);
                    }
                    else
                    {
                        propertys.propertyValues[p.id] += p.val;
                    }
                }
                else
                {
                    propertys.propertyValues[p.id] += p.val;
                }

            }

        }


    }

    public virtual void ResetAllProperty(bool isinit)
    {
        SetBaseProperty();
        SetEquipProperty();

        for (int i = 1; i < propertys.propertyValues.Length; i++)
        {
            if (!ConfigManager.propertyConfManager.dataMap.ContainsKey(i))
            {
                continue;
            }
            PropertyConf pconf = ConfigManager.propertyConfManager.dataMap[i];
            if (pconf.formula == 1)
            {
                propertys.propertyValues[i] = (1 - propertys.propertyValues[i]) * 100;
            }
            else if (pconf.formula == 2)
            {
                propertys.propertyValues[i] = conf.propertyValues[i] + conf.propertyValues[i] * (1f - 100f / (100f + propertys.propertyValues[i]));
            }
        }

        propertys.PropertyAddition(enCharacterProperty.life, enCharacterProperty.lifeAddition);
        propertys.PropertyAddition(enCharacterProperty.armor, enCharacterProperty.armorAddition);
        propertys.PropertyAddition(enCharacterProperty.atkForce, enCharacterProperty.atkForceAddition);

        if (isinit)
        {
            InitProperty();
        }
    }


    //计算属性
    public virtual void ResetAllProperty()
    {
        ResetAllProperty(false);
        //this.propertys.Log();
    }

    public override bool ObjUpdate()
    {
        if (IsDie()) return false;
        base.ObjUpdate();
        if (actionManager != null)
        {
            actionManager.Update();
        }

        if(ai != null){
            ai.Update();
        }

        //生命回复
        if (life < propertys.GetProperty(enCharacterProperty.life))
        {
            lifeReviveTime += Time.deltaTime;
            if (lifeReviveTime >= 1)
            {
                lifeReviveTime -= 1;
                lifeReviveCount += propertys.GetProperty(enCharacterProperty.liferevive);
                if(lifeReviveCount > 1){
                    int addval = (int)lifeReviveCount;
                    this.ChangeLife(addval, false);
                    lifeReviveCount -= addval;
                }
            }
        }

        this.SetZPos();
        return true;
    }

    public virtual void AddAI(){
        ai = new BaseEnemyAI();
        ai.Init(this);
    }

    public virtual bool AddEquip(EquipData equip){
        for (int i = 0; i < equipList.Count; i ++){
            if(equipList[i].instanceid == equip.instanceid){
                return false;
            }
        }
        equipList.Add(equip);
        return true;
    }

    public virtual bool CancleEquip(int instanceid){
        for (int i = 0; i < equipList.Count; i++)
        {
            if (equipList[i].instanceid == instanceid)
            {
                equipList.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public virtual void Move(Vector3 targetpos){
        targetpos = GameCommon.GetWorldPos(targetpos);
        Vector3 v = (targetpos - new Vector3(transform.position.x,transform.position.y)).normalized;


        transform.position += v * Time.deltaTime * this.GetMoveSpeed();

        SetDir(v.x);
    }

    public void SetDir(float dir){

        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (dir > 0?-1:1), transform.localScale.y, transform.localScale.z);
    
    }

    public void SetZPos (){

        GameCommon.SetObjZIndex(gameObject,3);
    }

    string lastAnimatorName = "";
    public void SetAnimatorState(AnimatorState state,float speed){

        if (state != AnimatorState.Dead &&  IsDie()) return;
        //if (state == AnimatorState.atk)anim.Play(""+state);
        //anim.SetInteger("state", (int)state);
        //anim.speed = speed;
        anim.SetBool(lastAnimatorName, false);
        string actionname = Enum.GetName(typeof(AnimatorState), state);
        anim.SetBool(actionname, true);
        lastAnimatorName = actionname;
    }

    public virtual void StartRun(Vector3 targetPos)
    {
        actionManager.StartAction(null, targetPos);
    }


    public void StartAtk(InGameBaseObj target)
    {
        actionManager.StartAction(target, target.transform.position);
        //agent.SetDestination(transform.position);
    }

    public virtual void StopAction()
    {
        actionManager.StopAction();
    }

    public override void Die()
    {
        actionManager.Destory();

        InGameManager.GetInstance().inGameUIManager.DelRole(this.instanceId);
        //base.Die();
        SetAnimatorState(AnimatorState.Dead, 1);

        if(camp == enMSCamp.en_camp_enemy){
            EquipSystem.GetInstance().OutEquip(gameObject, level);
        }

        transform.GetComponent<BoxCollider>().enabled = false;
        Invoke("Delself",3);

    }

    public void Delself(){
        Destroy(gameObject);
    }

    public bool ChangeLife(int val,bool iscombo){
        if (val == 0) return false;
        life = Mathf.Clamp(life + val,0,propertys.GetProperty(enCharacterProperty.life));

        if (life <= 0){
            SetDie(false);

            EventData.CreateEvent(EventID.EVENT_GAME_CHARACTER_DIE).AddData(this.instanceId, this.level).Send();

        }
        Color c;
        if(val > 0){
            c = Color.green;
        }else{
            if(iscombo){
                c = Color.red;
            }else{
                c = Color.yellow;
            }
        }
        ChangeLifeLabel.CreateChangeLifeLabel(transform.position + 
                                              new Vector3(0,this.boxSize.y+0.2f,0),c,val+"");

        return true;
    }

    /// <summary>
    /// Hurt the specified source, val, comborate, comboval and strike.
    /// </summary>
    /// <returns>The hurt.</returns>
    /// <param name="source">Source.</param>
    /// <param name="val">Value.</param>
    /// <param name="comborate">Comborate.</param>
    /// <param name="comboval">Comboval.</param>
    /// <param name="strike">是否穿透攻击.</param>
    public bool Hurt(InGameBaseObj source, int val, float comborate, float comboval,bool strike){
        float overval = val;


        if(UnityEngine.Random.Range(0f,100f) < propertys.GetProperty(enCharacterProperty.avoid)){

            ChangeLifeLabel.CreateChangeLifeLabel(transform.position +
                                              new Vector3(0, this.boxSize.y + 0.2f, 0), Color.yellow,  "闪避");
            return false;
        }

        bool iscombo = false;
        if(UnityEngine.Random.Range(0f, 100f) < comborate){
            overval += overval * comboval / 100f;
            iscombo = true;
        }

        if(!strike){
            float armor = propertys.GetProperty(enCharacterProperty.armor) * 0.06f;
            armor = (armor / (armor + 1));
            overval -= overval * armor;
            if (overval < 0) overval = 0;
        }

        float returnval = overval * (propertys.GetProperty(enCharacterProperty.returnhurt) / 100f);
        overval -= returnval;

        ((InGameBaseCharacter)source).ChangeLife(-(int)returnval, false);
        ((InGameBaseCharacter)source).AtkHurt((int)overval);

        EventData.CreateEvent(EventID.EVENT_GAME_CHARACTER_HURT).
                 AddData(source, this,-(int)overval).Send();
        
        return ChangeLife(-(int)Math.Ceiling(overval),iscombo);
    }

    public void AtkHurt(int val){
        float pro = propertys.GetProperty(enCharacterProperty.steallife) / 100f;
        float stealval = val * pro;

        this.ChangeLife((int)stealval,false);
    }

    public int GetAtkForce(){
        float val = propertys.GetProperty(enCharacterProperty.atkForce);
        val *= UnityEngine.Random.Range(0.8f, 1.2f);
        return (int)val;
    }

    public float GetAtkSpeed()
    {
        
        return propertys.GetProperty(enCharacterProperty.atkSpeed);
    }

    public float GetMoveSpeed(){
        return propertys.GetProperty(enCharacterProperty.moveSpeed);
    }

    public int GetBaseSkillID(){

        return this.conf.atkskill;
    }

    public InGameBaseCharacter.AnimatorState GetAtkAnimator(){

        return InGameBaseCharacter.AnimatorState.Attack;
    }

    public float GetAtkDis(InGameBaseObj target)
    {
        //float ret = boxSize.x + 0.3f;
        //if(target.GetObjType() == enObjType.character){
        //    ret += ((InGameBaseCharacter)target).boxSize.x;
        //    ret += this.conf.atkdis;
        //}
        float ret = this.conf.atkdis + 1;

        return ret;
    }
    public override void HandleEvent(EventData resp)
    {
        if (IsDie()) return ;
        base.HandleEvent(resp);

        switch(resp.eid){
            case EventID.EVENT_GAME_CHARACTER_HURT:
                if(actionManager.IsAction()){
                    return;
                }
                InGameBaseCharacter target = (InGameBaseCharacter)resp.sUserData[1];
                InGameBaseCharacter source = (InGameBaseCharacter)resp.sUserData[0];

                if(!this.IsEnemy(source)){
                    return;
                }

                float dis = Vector3.Distance(target.transform.position, this.transform.position);

                if(dis < 5){
                    this.StartAtk((InGameBaseCharacter)resp.sUserData[0]);
                }

                break;
        }

    }

    private void OnDestroy()
    {

        if (ai != null)
        {
            ai.Destory();
        }

        EventManager.Remove(this);
    }
}
