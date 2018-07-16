using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBaseObj : BaseUnityObject {

    public enum enObjType{
        nothing,
        character,
        equip,
        skill
    }

    public int instanceId = -1;
    public int confid = -1;
    public enMSCamp camp = enMSCamp.en_camp_neutral;
    bool isDie = false;

    public virtual enObjType GetObjType()
    {
        return enObjType.nothing;

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
        Debug.Log("set die");
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

}
