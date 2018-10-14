using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBaseMapObj : InGameBaseObj {

    [HideInInspector]public bool isAction = false;
    float actionTime = 0,maxActionTime = 0.5f;

    Vector3 targetPosition, targetScale ,startPosition,startScale;

    public override enObjType GetObjType()
    {
        return enObjType.map;

    }

    //是否结束逻辑【是否需要攻击】
    public virtual bool HandleFuntion(InGameBaseObj obj){
        return false;
    }

    //
    public virtual bool Hurt(InGameBaseObj obj)
    {
        return false;
    }


    public void Show(){
        isAction = true;
        targetPosition = transform.position;
        targetScale = transform.localScale;
        actionTime = 0f;
        startPosition = transform.position = targetPosition - new Vector3(0, 1, 0);
        startScale = transform.localScale = Vector3.zero;
    }

    public void Hide(){
        isAction = true;
        actionTime = 0f;
        targetPosition = targetPosition - new Vector3(0, 1, 0);
        targetScale = Vector3.zero;

        startPosition = transform.position;
        startScale = transform.localScale;
    }

    public void ActionUpdate(){
        if (!isAction) return;
        actionTime += Time.deltaTime;
        float rate = actionTime / maxActionTime;

        if(rate > 1){
            isAction = false;
            transform.position = targetPosition;
            transform.localScale = targetScale;
            return;
        }

        transform.position = startPosition + (targetPosition - startPosition) * rate;
        transform.localScale = startScale + (targetScale - startScale) * rate;

    }
}
