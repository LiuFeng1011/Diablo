using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill300001 : BaseSkill {


    public override bool ObjUpdate()
    {
        //THROW ATK
        SetDie(true);

        target.Hurt(source,
                    source.GetAtkForce(),
                    source.propertys.GetProperty(enCharacterProperty.comborate),
                    source.propertys.GetProperty(enCharacterProperty.comboval),
                    false);

        return base.ObjUpdate();

    }

}
