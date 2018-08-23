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

        MazeMapManager gameMap = InGameManager.GetInstance().inGameLevelManager.gameMap;
        List<Vector2> _path = astar.StratAStar(
            gameMap.astarArray,
            GameCommon.GetMapPos(parent.transform.position) ,
            GameCommon.GetMapPos(targetPos));
        
        if (_path.Count <= 0)
        {
            return;
        }
        path = _path;

        isaction = true;
        this.target = target;
        this.targetPos = targetPos;

        atkTime = 0;
        state = ActionState.move;

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

            if (Vector2.Distance(parent.transform.position, target.transform.position) > parent.GetAtkDis(target))
            {
                StartAction( target,  target.transform.position);
                MoveAction();
            }
            else
            {
                StartAtk();
            }
        }else{
            MoveAction();
        }
    }

    void MoveAction(){
        Vector3 tp = GameCommon.GetWorldPos(path[0]);

        float dis = Vector2.Distance(parent.transform.position, tp);
        if (dis <= parent.GetMoveSpeed() * Time.deltaTime)
        {

            SetPathPos();
            if (path.Count <= 0)
            {
                parent.StopAction();
            }
            else
            {
                parent.Move(path[0]);
            }
            return;
        }
        else
        {
            parent.Move(path[0]);
            parent.SetAnimatorState(InGameBaseCharacter.AnimatorState.Run, parent.GetMoveSpeed()*0.6f);
        }
    }
    void StartAtk()
    {
        state = ActionState.atkbegin;

        if(target == null || target.IsDie()){
            state = ActionState.non;
            return;
        }
        if (Vector2.Distance(parent.transform.position, target.transform.position) > parent.GetAtkDis(target))
        {
            state = ActionState.move;
            return;
        }

        if (GameConst.CAMP_ATK[(int)target.camp, (int)parent.camp] == 0)
        {
            if (target.GetObjType() == InGameBaseObj.enObjType.equip)
            {
                InGameBaseEquip equip = (InGameBaseEquip)target;
                EquipData e = EquipSystem.GetInstance().RandEquipProperty(equip);
                InGameManager.GetInstance().inGamePlayerManager.AddEquip(e);
                MonoBehaviour.Destroy(equip.gameObject);
            }else if(target.GetObjType() == InGameBaseObj.enObjType.map){
                InGameBaseMapObj mapobj = (InGameBaseMapObj)target;
                mapobj.HandleFuntion(parent);
            }
            parent.StopAction();

            return;
        }

        parent.SetAnimatorState(parent.GetAtkAnimator(), parent.GetAtkSpeed());
        atkTime = 0;

        parent.SetDir(target.transform.position.x - parent.transform.position.x);
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
