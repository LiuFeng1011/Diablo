﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill300002 : BaseSkill {

    Vector3 moveVector;
    Vector3 startPos;

    Vector3 additionPos;
    public override void SkillInit(InGameBaseCharacter source, InGameBaseCharacter target)
    {
        base.SkillInit(source,target);

        additionPos = new Vector3(0, source.boxSize.y / 2, 0);
        Vector3 sourcePos = source.transform.position + additionPos;
        Vector3 targetPos = target.transform.position + additionPos;

        moveVector = (targetPos - sourcePos).normalized;
        moveVector.z = 0;
        startPos = sourcePos;
        transform.position = startPos;

        float angle = GameCommon.GetVecAngle(moveVector);
        transform.eulerAngles = new Vector3(0, 0,angle);
    }

    public override bool ObjUpdate()
    {
        if(source == null) {
            SetDie(true);
            return false;
        }
        //Debug.Log("move vec : " + moveVector);
        transform.position = transform.position + moveVector * Time.deltaTime * 10;

        GameCommon.SetObjZIndex(gameObject,3);

        if(Vector2.Distance(startPos,transform.position) > 20){
            SetDie(true);
            return false;
        }
        Vector3 collisionpos = transform.position - additionPos;
        //游戏物体碰撞
        List<InGameBaseObj> list = InGameManager.GetInstance().inGameObjManager.GetObjListByDistance(collisionpos, 0.5f);

        for (int i = 0; i < list.Count; i ++){
            if(HitGameObj(list[i])){
                return false;
            }
        }

        //地图碰撞
        Vector2 mapos = GameCommon.GetMapPos(collisionpos);
        if (InGameManager.GetInstance().inGameLevelManager.gameMap.GetPointType((int)mapos.x, (int)mapos.y) == MazeCreate.PointType.wall){
            SetDie(true);
            return false;
        }

        return base.ObjUpdate();

    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    HitObj(collision.gameObject);
    //}

    //private void OnTriggerEnter(Collider collision)
    //{
    //    HitObj(collision.gameObject);
    //}

    void HitObj(GameObject obj){

        InGameBaseObj baseobj  = obj.GetComponent<InGameBaseObj>();
        if (baseobj == null)
        {
            return;
        }

        HitGameObj(baseobj);
    }

    bool HitGameObj(InGameBaseObj baseobj)
    {
        if(baseobj.instanceId == this.instanceId){
            return false;
        }
        if (baseobj.GetObjType() != enObjType.character)
        {
            return false;
        }
        if (!source.IsEnemy(baseobj))
        {
            return false;
        }

        InGameBaseCharacter character = (InGameBaseCharacter)baseobj;


        character.Hurt(source,
                    source.GetAtkForce(),
                    source.propertys.GetProperty(enCharacterProperty.comborate),
                    source.propertys.GetProperty(enCharacterProperty.comboval),
                    false);
        Debug.Log(baseobj.gameObject.name);
        SetDie(true);

        return true;
    }

}
