using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrlManager : InGameBaseManager {

	// Use this for initialization
	public override void Init () {

        EventManager.Register(this,
                       EventID.EVENT_TOUCH_DOWN);

	}

    public override void Destory(){

        EventManager.Remove(this);
    }

    public override void HandleEvent(EventData resp)
    {

        switch (resp.eid)
        {
            case EventID.EVENT_TOUCH_DOWN:
                EventTouch eve = (EventTouch)resp;
                TouchToPlane(eve.pos);
                //Fire(GameCommon.ScreenPositionToWorld(eve.pos));
                break;
        }

    }

    public void TouchToPlane(Vector3 pos)
    {
        Ray ray = InGameManager.GetInstance().gamecamera.ScreenPointToRay(pos);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 200, LayerMask.GetMask("GameObj")))
        {
            GameObject gameObj = hitInfo.collider.gameObject;
            InGameBaseObj obj = gameObj.GetComponent<InGameBaseObj>();
            if(obj != null ){
                (new EventData(EventID.EVENT_GAME_TOUCH_OBJ)).AddData(obj).Send();
                return;
            }
        }

        if (Physics.Raycast(ray, out hitInfo, 200, LayerMask.GetMask("TouchPlane")))
        {
            GameObject gameObj = hitInfo.collider.gameObject;
            Vector3 hitPoint = hitInfo.point;

            (new EventGameTouchGround(hitPoint)).Send();
        }
    }


    private void OnDestroy()
    {
        EventManager.Remove(this);
    }
}
