using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjMoveManager : BaseActionManager {

    AStar astar = new AStar();
    List<Vector3> path = new List<Vector3>();

    public override void Update()
    {
        
        float dis = Vector3.Distance(parent.transform.position, targetPos);
        //Debug.Log("dis : " +dis + " speed : " + parent.GetMoveSpeed());
        if(dis <= parent.GetMoveSpeed() * Time.deltaTime * 2){
            //parent.transform.position = targetPos;
            //parent.SetAnimatorState(InGameBaseCharacter.AnimatorState.idle, 1);
            parent.StopAction();
            return;
        }else{
            parent.Move(targetPos);
            parent.SetAnimatorState(InGameBaseCharacter.AnimatorState.Run, parent.GetMoveSpeed()*10f);
        }
    }

    public override void StartAction(InGameBaseObj target, Vector3 targetPos)
    { 
        base.StartAction(target,targetPos);
    }


    public override void StopAction()
    {
        parent.SetAnimatorState(InGameBaseCharacter.AnimatorState.Idle, 1);
        isaction = false;
    }
    public void Destory()
    {

    }

}
