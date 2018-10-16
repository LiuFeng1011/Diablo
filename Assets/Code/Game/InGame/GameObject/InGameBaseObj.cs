using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBaseObj : BaseUnityObject {

    public enum enObjType{
        nothing,
        character,
        equip,
        skill,
        map,
    }

    [HideInInspector] public int instanceId = -1;
    public int confid = -1;

    [HideInInspector] public int parentid;
    public enMSCamp camp = enMSCamp.en_camp_neutral;
    bool isDie = false;

    public virtual enObjType GetObjType()
    {
        return enObjType.nothing;

    }

    //场景对象互动距离
    public virtual int ActionDistance(){
        return 1;
    }

    public virtual void Init(int instanceId, int confid,enMSCamp camp){
        this.instanceId = instanceId;
        this.confid = confid;
        this.camp = camp;
    }

    public virtual bool ObjUpdate(){
        return true;
    }


    public virtual void SetDie(bool selfdie){
        isDie = true;
    }

    public virtual bool IsDie()
    {
        return isDie;
    }

    public virtual void Die(){
        Destroy(gameObject);
    }

    public bool IsEnemy(InGameBaseObj target){
        int val = GameConst.CAMP_ATK[(int)camp, (int)target.camp];
        return val == 1;
    }

    public virtual void Serialize(DataStream writer)
    {
        
    }

    public virtual void Deserialize(DataStream reader)
    {
        //myData.Deserialize(reader);
        //parent = reader.ReadSInt32();
    }

}
