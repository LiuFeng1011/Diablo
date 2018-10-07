using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMessage : BaseUnityObject {

    static HandleMessage instance;

    public void Awake(){
        instance = this;
        Init();
    }

    public void Init(){
        EventManager.Register(this,
                              EventID.EVENT_DATA_USE_LEVELUP_POINT,
                              EventID.EVENT_DATA_KILLENEMY);
    }

    public override void HandleEvent(EventData resp)
    {
        base.HandleEvent(resp);
        switch(resp.eid){
            case EventID.EVENT_DATA_USE_LEVELUP_POINT:
                int roleid = int.Parse(resp.sUserData[0].ToString());
                int type = int.Parse(resp.sUserData[1].ToString());
                UserDataManager.instance.UseLevelupProperty(roleid,type);
                break;
            case EventID.EVENT_DATA_KILLENEMY:
                InGameBaseCharacter source = (InGameBaseCharacter)resp.sUserData[0];
                InGameBaseCharacter target = (InGameBaseCharacter)resp.sUserData[1];

                UserDataManager.instance.AddExp(source.GetData().instanceid,target.level);
                break;
        }
    }
}
