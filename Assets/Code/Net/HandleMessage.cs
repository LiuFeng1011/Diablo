using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMessage : BaseUnityObject {

    static HandleMessage instance;

    public void Awake(){
        instance = this;
    }

    public void Init(){
        EventManager.Register(this,
                              EventID.EVENT_DATA_USELEVELUPPOINT);
    }

    public override void HandleEvent(EventData resp)
    {
        base.HandleEvent(resp);
        switch(resp.eid){
            case EventID.EVENT_DATA_USELEVELUPPOINT:
                int roleid = (int)resp.sUserData[0];
                int type = (int)resp.sUserData[1];
                UserDataManager.instance.AddLevelupProperty(roleid,type);
                break;
        }
    }
}
