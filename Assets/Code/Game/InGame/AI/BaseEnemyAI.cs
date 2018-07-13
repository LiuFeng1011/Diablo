using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyAI : BaseGameObject {

    enum AIState{
        run,
        atk
    }

    AIState state = AIState.run;
    InGameBaseCharacter character;

    public void Init(InGameBaseCharacter character){
        this.character = character;
    }

    public void Update()
    {
        if (state == AIState.run)
        {
            //寻找敌人
            List<InGameBaseObj> objlist = InGameManager.GetInstance().inGameObjManager.objList;

            for (int i = 0; i < objlist.Count; i ++){
                if(objlist[i].GetObjType() == InGameBaseObj.enObjType.role){
                    if (Vector3.Distance(objlist[i].transform.position, this.character.transform.position) < 3f){
                        character.StartAtk((InGameBaseCharacter)objlist[i]);
                        state = AIState.atk;
                        break;
                    }
                }
            }

        }
        else{
            if(character == null || character.actionManager.target == null){
                state = AIState.run;
                character.StopAction();
                return;
            }
            if (Vector3.Distance(character.actionManager.target.transform.position,
                                 this.character.transform.position) > 3f)
            {  
                state = AIState.run;
                //character.StopRun();
                character.StopAction();
            }

        }
    }

    public void Destory()
    {

    }

}
