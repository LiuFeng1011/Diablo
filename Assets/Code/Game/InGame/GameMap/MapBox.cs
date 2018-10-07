using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBox : InGameBaseMapObj {

    //是否结束逻辑【是否需要攻击】
    public override bool HandleFuntion(InGameBaseObj obj)
    {
        return true;
    }

    //
    public override bool Hurt(InGameBaseObj obj)
    {
        MapObjConf conf = ConfigManager.mapObjConfManager.map[this.confid];
        InGameManager.GetInstance().inGameLevelManager.gameMap.DelMapObj(transform.position,conf);
        transform.position = new Vector3(-9999, -9999, 0);
        return true;
    }
}
