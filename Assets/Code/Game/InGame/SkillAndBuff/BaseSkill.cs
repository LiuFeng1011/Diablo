using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSkill : InGameBaseObj {

    public InGameBaseCharacter source;
    public InGameBaseCharacter target;


    public static BaseSkill CreateSkill(
        int skillid,
        InGameBaseCharacter source,
        InGameBaseCharacter target){

        BaseSkill skill = InGameManager.GetInstance().inGameObjManager.AddSkill(skillid,source.camp) as BaseSkill;

        skill.SkillInit(source,target);
        return skill;
    }

    public virtual void SkillInit(InGameBaseCharacter source,InGameBaseCharacter target){
        this.source = source;
        this.target = target;
    }


    public override enObjType GetObjType()
    {
        return enObjType.skill;
    }


    public override void Init(int instanceId, int confid, enMSCamp camp)
    {
        base.Init(instanceId, confid,camp);
    }

    public override bool ObjUpdate()
    {
        return base.ObjUpdate();
    }

}
