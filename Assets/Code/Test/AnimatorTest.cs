using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTest : BaseUnityObject {
    InGameBaseCharacter girl1;
    GameTouchController gameTouchController;
	// Use this for initialization
	void Start () {

        //初始化事件管理器
        EventManager em = gameObject.AddComponent<EventManager>();

        EventManager.Register(this,
                              EventID.EVENT_TOUCH_SWEEP,
                              EventID.EVENT_TOUCH_UP);



        GameObject obj = GameObject.Find("Girl1");
        girl1 = obj.transform.GetComponent<InGameBaseCharacter>();
        girl1.Init(1,1,enMSCamp.en_camp_player);
        gameTouchController = new GameTouchController();

	}
	
	// Update is called once per frame
	void Update () {

        if (gameTouchController != null) gameTouchController.Update();
	}

    public override void HandleEvent(EventData resp)
    {
        switch (resp.eid)
        {
            case EventID.EVENT_TOUCH_SWEEP:
                
                EventTouch e = (EventTouch)resp;
                girl1.StartRun(Vector3.zero);
                break;
            case EventID.EVENT_TOUCH_UP:
                EventTouch eup = (EventTouch)resp;
                girl1.StopAction();
                break;
        }
    }
}
