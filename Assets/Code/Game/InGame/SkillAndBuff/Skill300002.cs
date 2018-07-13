using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill300002 : BaseSkill {

    Vector3 moveVector;
    Vector3 startPos;

    public override void SkillInit(InGameBaseCharacter source, InGameBaseCharacter target)
    {
        base.SkillInit(source,target);

        Vector3 sourcePos = source.transform.position + new Vector3(0,source.boxSize.y / 2,0);
        Vector3 targetPos = target.transform.position + new Vector3(0, target.boxSize.y / 2, 0);

        moveVector = (targetPos - sourcePos).normalized;

        startPos = sourcePos;
        transform.position = startPos;

        transform.forward = moveVector;
    }

    public override bool ObjUpdate()
    {
        if(source == null) {
            SetDie(true);
            return false;
        }
        //Debug.Log("move vec : " + moveVector);
        transform.position = transform.position + moveVector * Time.deltaTime * 5;

        if(Vector3.Distance(startPos,transform.position) > 20){
            SetDie(true);
            return false;
        }

        return base.ObjUpdate();

    }

    private void OnCollisionEnter(Collision collision)
    {
        HitObj(collision.gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        HitObj(collision.gameObject);
    }

    void HitObj(GameObject obj){

        InGameBaseObj baseobj  = obj.GetComponent<InGameBaseObj>();
        if (baseobj == null)
        {
            SetDie(true);
            return;
        }
        if (!source.IsEnemy(baseobj))
        {
            return;
        }
        if(!(baseobj.GetObjType() == enObjType.role || baseobj.GetObjType() == enObjType.enemy)){
            return;
        }

        InGameBaseCharacter character = (InGameBaseCharacter)baseobj;


        character.Hurt(source,
                    source.GetAtkForce(),
                    source.propertys.GetProperty(enCharacterProperty.comborate),
                    source.propertys.GetProperty(enCharacterProperty.comboval),
                    false);

        SetDie(true);
    }
}
