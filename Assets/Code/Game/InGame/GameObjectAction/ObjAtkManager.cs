using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjAtkManager : BaseActionManager {

    float atkTime = 0;

    enum AtkState{
        non,
        move,
        atkbegin,
        atkend,
    }

    AtkState state;

    public override void Init(InGameBaseCharacter parent, Vector3 targetPos)
    {
        base.Init(parent,targetPos);
        state = AtkState.non;
    }

    public override void Update()
    {
        if (target == null || target.IsDie()) {
            parent.StopAction();
            return;
        }


        switch(state){
            case AtkState.non:
                break;
            case AtkState.move:
                Move();
                break;
            case AtkState.atkbegin:
                atkTime += Time.deltaTime;
                if (atkTime > (1 / parent.GetAtkSpeed()) * 0.5f)
                {
                    Atk();
                }
                break;
            case AtkState.atkend:
                atkTime += Time.deltaTime;
                if (atkTime >= (1 / parent.GetAtkSpeed()))
                {
                    parent.SetAnimatorState(InGameBaseCharacter.AnimatorState.Idle, 1);
                    if (atkTime >= (1 / parent.GetAtkSpeed()) * 1.1)
                    {
                        StartAtk();
                    }
                }
                break;
        }
    }

    public override void StartAction(InGameBaseObj target, Vector3 targetPos)
    {
        if (state == AtkState.atkbegin || state == AtkState.atkend)
        {
            return;
        }

        base.StartAction(target, targetPos);

        atkTime = 0;

        //Vector3 v = (target.transform.position - parent.transform.position).normalized;
        //parent.transform.forward = v;

        state = AtkState.move;
    }

    public override void StopAction()
    {
        parent.SetAnimatorState(InGameBaseCharacter.AnimatorState.Idle, 1);
        isaction = false;
        atkTime = 0;
        state = AtkState.move;
    }

    void Move(){
        if (Vector3.Distance(parent.transform.position, target.transform.position) > parent.GetAtkDis(target))
        {
            parent.Move(target.transform.position);
            return;
        }
        else
        {
            //parent.agent.SetDestination(parent.transform.position);

        }
        StartAtk();
    }

    void StartAtk(){
        state = AtkState.atkbegin;


        if (Vector3.Distance(parent.transform.position, target.transform.position) > parent.GetAtkDis(target))
        {
            state = AtkState.non;
            return;
        }

        if(GameConst.CAMP_ATK[(int)target.camp,(int)parent.camp] == 0){
            if (target.GetObjType() == InGameBaseObj.enObjType.equip)
            {
                InGameBaseEquip equip = (InGameBaseEquip)target;

                parent.StopAction();

                EquipData e = EquipSystem.GetInstance().RandEquipProperty(equip);
                InGameManager.GetInstance().inGamePlayerManager.AddEquip(e);

                MonoBehaviour.Destroy(equip.gameObject);
            }
            return;
        }

        parent.SetAnimatorState(parent.GetAtkAnimator(), parent.GetAtkSpeed());
        atkTime = 0;

        Vector3 v = (target.transform.position - parent.transform.position).normalized;
        parent.transform.forward = v;
    }


    void Atk(){

        state = AtkState.atkend;
        parent.SetAnimatorState( InGameBaseCharacter.AnimatorState.Idle, 1);
        BaseSkill.CreateSkill(parent.GetBaseSkillID(), parent, (InGameBaseCharacter)target);
        //((InGameBaseCharacter)target).ChangeLife(-parent.GetAtkForce());
    }

    public bool IsAtk(){
        return target != null;
    }

    public void Destory(){
        
    }

}
