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

    mana = 3,//魔法
    manarevive = 4,//魔法回复


    atkForce = 5,//攻击力

    atkSpeed = 6,    /*  攻击速度  */
    moveSpeed = 7,   /*  移动速度  */

    comborate = 8,   /*  暴击几率    */
    comboval = 9,    /*  暴击伤害    */
    armor = 10,   /*  护甲  */
    avoid = 11,   /*  闪避  */
    returnhurt = 12,  /*  反弹伤害    */
    steallife = 13,   /*  偷取生命    */
    golddrop = 14,    /*  金币掉率加成  */
    equipdrop = 15,   /*  魔法物品掉率  */

    lifeAddition = 16,   /*  生命加成  */
    armorAddition = 17,   /*  护甲加成  */

    atkForceAddition = 18,      /*  攻击力加成  */

    strength = 25,
    agility = 26,
    brains = 27,

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
    public float mana = 10;

    protected CharacterConf conf;

    public string charactername = "";

    public Vector3 boxSize;

    bool isElite = false;


    Vector3 lastPos = Vector3.zero;

    List<EquipData> equipList = new List<EquipData>();

    InGameBaseObj killMe = null;

    //生命回复时间
    float lifeReviveTime = 0;
    float lifeReviveCount = 0f;

    float manaReviveTime = 0;
    float manaReviveCount = 0f;

    public override void Init(int instanceId, int confid, enMSCamp camp)
    {
        base.Init(instanceId, confid, camp);

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
        //InGameManager.GetInstance().inGameUIManager.AddRole(this);

        this.level = conf.level;
        ResetAllProperty(true);

        lastPos = transform.position;

        EventManager.Register(this,
                      EventID.EVENT_GAME_CHARACTER_HURT);


        EventData.CreateEvent(EventID.EVENT_GAME_CHARACTER_BORN).AddData(this).Send();
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
        mana = propertys.GetProperty(enCharacterProperty.mana);//魔法
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

            AHFloat val = conf.propertyValues[i];
            if (this.isElite) val *= 2f;
            if (pconf.formula != 2)
            {
                if (pconf.formula == 1)
                {
                    propertys.propertyValues[i] = (1f - val / 100f);
                }
                else
                {
                    propertys.propertyValues[i] = val;
                }
            }
            else
            {
                propertys.propertyValues[i] = 0;
            }
        }

        float baselife = propertys.propertyValues[(int)enCharacterProperty.life];
        propertys.propertyValues[(int)enCharacterProperty.life] = this.level * this.level * baselife / 2 + baselife;

        float baseforce = propertys.propertyValues[(int)enCharacterProperty.atkForce];
        propertys.propertyValues[(int)enCharacterProperty.atkForce] = this.level * this.level * baseforce / 10 + baseforce;

        //附加属性
        if (data == null) return;
        foreach (KeyValuePair<int, float> kv in data.additionPropertyList)
        {
            AddPropertyVal(kv.Key,kv.Value,1);
        }

    }

    public virtual void SetEquipProperty(){

        for (int i = 0; i < equipList.Count; i ++){
            EquipData equipdata = equipList[i];
            //EquipConf econf = ConfigManager.equipConfManager.dic[equipdata.equipid];
            for (int j = 0; j < equipdata.propertyList.Count; j++)
            {
                EquipProperty p = equipdata.propertyList[j];
                AddPropertyVal(p.id, p.val, 1);
            }
        }
    }

    //主属性加成
    public void SetMainPropertyAddition(){
        
        List<PropertyConf> pconflist = ConfigManager.propertyConfManager.datas;

        for (int i = 0; i < pconflist.Count; i ++){
            PropertyConf _conf = pconflist[i];
            if(_conf.mainProerty < 0){
                continue;
            }

            AddPropertyVal(_conf.id, propertys.propertyValues[_conf.mainProerty] * _conf.levelval, 1);
        }

    }

    //计算属性
    public virtual void ResetAllProperty(bool isinit = false)
    {
        SetBaseProperty();
        SetEquipProperty();
        SetMainPropertyAddition();

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

    public void AddPropertyVal(int id,float val,float times){

        PropertyConf pconf = ConfigManager.propertyConfManager.dataMap[id];
        val *= times;
        if (pconf.formula != 2)
        {
            if (pconf.formula == 1)
            {
                propertys.propertyValues[id] *= (1f - val / 100f);
            }
            else
            {
                propertys.propertyValues[id] += val;
            }
        }
        else
        {
            propertys.propertyValues[id] += val;
        }
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

                if(lifeReviveCount >= 1){
                    int addval = (int)lifeReviveCount;
                    this.ChangeLife(null,addval, false);
                    lifeReviveCount -= addval;
                }
            }
        }

        //魔法回复
        if (mana < propertys.GetProperty(enCharacterProperty.mana))
        {
            manaReviveTime += Time.deltaTime;
            if (manaReviveTime >= 1)
            {
                manaReviveTime -= 1;
                manaReviveCount += propertys.GetProperty(enCharacterProperty.manarevive);

                if (manaReviveCount >= 1)
                {
                    int addval = (int)manaReviveCount;
                    this.ChangeMana(addval);
                    manaReviveCount -= addval;
                }
            }
        }

        this.SetZPos();
        return true;
    }

    public void SetIsElite(bool isElite){
        this.isElite = isElite;
        this.ResetAllProperty(true);
    }
    public bool GetIsElite(){
        return isElite;
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
        if (anim == null) return;
        if (state != AnimatorState.Dead &&  IsDie()) return;
        //if (state == AnimatorState.atk)anim.Play(""+state);
        //anim.SetInteger("state", (int)state);
        anim.SetBool(lastAnimatorName, false);
        string actionname = Enum.GetName(typeof(AnimatorState), state);
        anim.SetBool(actionname, true);
        lastAnimatorName = actionname;
        anim.speed = speed;
    }

    public virtual void StartRun(Vector3 targetPos)
    {
        actionManager.StartAction(null, targetPos,0);
    }


    public void StartAtk(InGameBaseObj target)
    {
        actionManager.StartAction(target, target.transform.position,target.ActionDistance());
        //agent.SetDestination(transform.position);
    }

    public virtual void StopAction()
    {
        actionManager.StopAction();
    }

    //死亡
    public override void Die()
    {
        actionManager.Destory();

        //InGameManager.GetInstance().inGameUIManager.DelRole(this.instanceId);
        //base.Die();
        SetAnimatorState(AnimatorState.Dead, 1);

        if (camp == enMSCamp.en_camp_enemy)
        {
            if(killMe.GetObjType() == enObjType.character){
                //爆装备
                InGameBaseCharacter source = (InGameBaseCharacter)killMe;
                if(UnityEngine.Random.Range(0,100)< conf.outodds){
                    EquipSystem.GetInstance().OutEquip(gameObject, level,
                                                   source.propertys.GetProperty(enCharacterProperty.equipdrop) +
                                                   this.conf.equipdrop);
                }
                EventData.CreateEvent(EventID.EVENT_DATA_KILLENEMY).AddData(source,this).Send();
            }
        }

        transform.GetComponent<BoxCollider>().enabled = false;
        transform.GetComponent<SphereCollider>().enabled = false;
        Invoke("Delself",3);

    }

    public void Delself(){
        Destroy(gameObject);
    }

    public bool ChangeMana(int val){
        if(mana + val < 0){
            return false;
        }
        mana += val;

        if(mana > propertys.GetProperty(enCharacterProperty.mana)){
            mana = propertys.GetProperty(enCharacterProperty.mana);
        }
        return true;
    }

    //生命值变化
    public bool ChangeLife(InGameBaseObj source,int val,bool iscombo){
        if (val == 0) return false;
        float maxlife = propertys.GetProperty(enCharacterProperty.life);
        life = Mathf.Clamp(life + val,0,maxlife);

        if (life <= 0){
            SetDie(false);
            killMe = source;
            EventData.CreateEvent(EventID.EVENT_GAME_CHARACTER_DIE).AddData(this).Send();

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

        EventData.CreateEvent(EventID.EVENT_DATA_CHANGELIFE).
                 AddData(this).Send();
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

        ((InGameBaseCharacter)source).ChangeLife(this,-(int)returnval, false);
        ((InGameBaseCharacter)source).AtkHurt(source,(int)overval);

        EventData.CreateEvent(EventID.EVENT_GAME_CHARACTER_HURT).
                 AddData(source, this,-(int)overval).Send();
        
        return ChangeLife(source,-(int)Math.Ceiling(overval),iscombo);
    }

    //对敌人造成了伤害，在这里处理吸血等状态
    public void AtkHurt(InGameBaseObj source,int val){
        float pro = propertys.GetProperty(enCharacterProperty.steallife) / 100f;
        float stealval = val * pro;

        this.ChangeLife(source,(int)stealval,false);
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

        if(target.camp == enMSCamp.en_camp_item){
            
            return (float)target.ActionDistance() + 0.2f;
        }

        float ret = this.conf.atkdis + 1.3f;

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

                float dis = Vector2.Distance(target.transform.position, this.transform.position);

                if(dis < 4){
                    
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

        if(actionManager != null){
            actionManager.Destory();
        }

        EventManager.Remove(this);
    }
}
