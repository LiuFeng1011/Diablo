using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBaseEnemy : InGameBaseCharacter {

    BaseEnemyAI ai;

    public override enObjType GetObjType()
    {
        return enObjType.character;

    }

    public override void Init(int instanceId, int confid, enMSCamp camp)
    {
        base.Init(instanceId,confid,camp);

        ai = new BaseEnemyAI();
        ai.Init(this);

    }
    public override bool ObjUpdate()
    {
        base.ObjUpdate();
        if (IsDie()) return false;
        ai.Update();
        return true;
    }


    public override void Die()
    { 
        base.Die();
        ai.Destory();

        transform.GetComponent<BoxCollider>().enabled = false;

        Invoke("Delself", 2);

        

    }
}
