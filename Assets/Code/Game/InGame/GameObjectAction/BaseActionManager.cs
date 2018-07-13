using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseActionManager : BaseGameObject {

    enum ActionState
    {
        non,
        move,
        atkbegin,
        atkend,
    }

    ActionState state;

    public InGameBaseCharacter parent;
    public InGameBaseObj target;

    public Vector3 targetPos = Vector3.zero;

    AStar astar = new AStar();
    List<Vector2> path = new List<Vector2>();


    public bool isaction = false;

    float atkTime = 0;

    public virtual void Init(InGameBaseCharacter parent, Vector3 targetPos)
    { 
        this.parent = parent;

        this.targetPos = targetPos;

    }

    public virtual void Update()
    { 
        switch (state)
        {
            case ActionState.non:
                break;
            case ActionState.move:
                Move();
                break;
            case ActionState.atkbegin:

                if (target == null || target.IsDie())
                {
                    parent.StopAction();
                    return;
                }
                atkTime += Time.deltaTime;
                if (atkTime > (1 / parent.GetAtkSpeed()) * 0.5f)
                {
                    Atk();
                }
                break;
            case ActionState.atkend:

                if (target == null || target.IsDie())
                {
                    parent.StopAction();
                    return;
                }

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

    public virtual bool IsAction(){
        return state != ActionState.non;
    }


    public virtual void StartAction(InGameBaseObj target,Vector3 targetPos){

        path.Clear();
        MazeMapManager gameMap = InGameManager.GetInstance().inGameLevelManager.gameMap;
        path = astar.StratAStar(
            gameMap.map,
            GameCommon.GetMapPos(parent.transform.position),
            GameCommon.GetMapPos(targetPos));
        if (path.Count <= 0)
        {
            this.targetPos = parent.transform.position;
            return;
        }


        isaction = true;
        if(target != null){
            this.targetPos = target.transform.position;
        }else{

            this.targetPos = targetPos;
        }
        this.target = target;

        atkTime = 0;
        state = ActionState.move;

        SetPathPos();

    }

    public virtual void StopAction()
    {
        isaction = false;

        parent.SetAnimatorState(InGameBaseCharacter.AnimatorState.Idle, 1);
        atkTime = 0;
        target = null;
        state = ActionState.non;
    }

    void Move()
    {
        if(target != null){

            targetPos = target.transform.position + (target.transform.position - parent.transform.position).normalized * parent.GetAtkDis(target) * 1.2f;

            if (Vector3.Distance(parent.transform.position, target.transform.position) > parent.GetAtkDis(target))
            {
                parent.Move(targetPos);
            }
            else
            {
                parent.Move(parent.transform.position);
                StartAtk();
            }
        }else{
            float dis = Vector3.Distance(parent.transform.position, targetPos);
            if (dis <= parent.GetMoveSpeed() * Time.deltaTime * 2)
            {
                parent.StopAction();
                return;
            }
            else
            {
                parent.Move(targetPos);
                parent.SetAnimatorState(InGameBaseCharacter.AnimatorState.Run, parent.GetMoveSpeed() * 0.3f);
            }
        }

    }

    void StartAtk()
    {
        state = ActionState.atkbegin;

        if(target == null || target.IsDie()){
            state = ActionState.non;
            return;
        }
        if (Vector3.Distance(parent.transform.position, target.transform.position) > parent.GetAtkDis(target))
        {
            state = ActionState.move;
            return;
        }

        if (GameConst.CAMP_ATK[(int)target.camp, (int)parent.camp] == 0)
        {
            if (target.GetObjType() == InGameBaseObj.enObjType.equip)
            {
                InGameBaseEquip equip = (InGameBaseEquip)target;

                parent.StopAction();

                EquipData e = EquipSystem.GetInstance().RandEquipProperty(equip.confid, equip.level);
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

    void Atk()
    {
        state = ActionState.atkend;
        parent.SetAnimatorState(InGameBaseCharacter.AnimatorState.Idle, 1);
        if(target != null){
            BaseSkill.CreateSkill(parent.GetBaseSkillID(), parent, (InGameBaseCharacter)target);
        }

        //((InGameBaseCharacter)target).ChangeLife(-parent.GetAtkForce());
    }
    void SetPathPos()
    {
        this.targetPos = GameCommon.GetWorldPos(path[0]);
        path.RemoveAt(0);
    }

    public void Destory(){
        
    }
}
