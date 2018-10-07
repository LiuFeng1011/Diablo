using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBaseMapObj : InGameBaseObj {

    public override enObjType GetObjType()
    {
        return enObjType.map;

    }

    //是否结束逻辑【是否需要攻击】
    public virtual bool HandleFuntion(InGameBaseObj obj){
        return false;
    }

    //
    public virtual bool Hurt(InGameBaseObj obj)
    {
        return false;
    }
}
