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
        moveVector.z = 0;
        startPos = sourcePos;
        transform.position = startPos;

        float angle = GameCommon.GetVecAngle(moveVector);
        Debug.Log(angle);
        transform.eulerAngles = new Vector3(0, 0,angle);
    }

    public override bool ObjUpdate()
    {
        if(source == null) {
            SetDie(true);
            return false;
        }
        //Debug.Log("move vec : " + moveVector);
        transform.position = transform.position + moveVector * Time.deltaTime * 15;
        SetZPos();
        if(Vector2.Distance(startPos,transform.position) > 20){
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
            return;
        }

        if (baseobj.GetObjType() != enObjType.character)
        {
            return;
        }
        if (!source.IsEnemy(baseobj))
        {
            return;
        }

        InGameBaseCharacter character = (InGameBaseCharacter)baseobj;


        character.Hurt(source,
                    source.GetAtkForce(),
                    source.propertys.GetProperty(enCharacterProperty.comborate),
                    source.propertys.GetProperty(enCharacterProperty.comboval),
                    false);
        Debug.Log("set die");
        SetDie(true);
    }
}
