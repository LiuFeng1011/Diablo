using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBaseEquip : InGameBaseObj {
    public int level = 1;

    public override enObjType GetObjType()
    {
        return enObjType.equip;
    }

}
