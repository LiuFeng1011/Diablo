using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGamePlayerManager : InGameBaseManager {

    InGameBaseCharacter role;


    public override void Init()
    {
        base.Init();


        EventManager.Register(this,
                              EventID.EVENT_GAME_TOUCH_GROUND,
                              EventID.EVENT_GAME_TOUCH_OBJ,
                              EventID.EVENT_UI_EQUIP,
                              EventID.EVENT_UI_DELEQUIP,
                              EventID.EVENT_UI_UNEQUIP);


        RoleData rd = UserDataManager.instance.GetChooseRoleData();

        role = InGameManager.GetInstance().inGameObjManager.AddObj(rd.roldid,enMSCamp.en_camp_player) as InGameBaseCharacter;
        role.SetData(rd);

        InGameManager.GetInstance().inGameCameraManager.SetTarget(role.gameObject);
        //InGameManager.GetInstance().GetManager<InGameObjManager>(InGameManager.enGameManager.InGameObjManager).AddObj(0);

        role.transform.position = GameCommon.GetWorldPos(InGameManager.GetInstance().inGameLevelManager.gameMap.GetStartPosition());

        role.ResetAllProperty(true);
        EventData.CreateEvent(EventID.EVENT_GAME_INITROLE).AddData(role).Send();

    }

    public InGameBaseCharacter GetRole(){
        return role;
    }

    // 穿装备
    public void AddEquip(int instanceid){
        
        if (instanceid == -1) return;
        UserDataManager.instance.AddEquip(role.GetData().instanceid, instanceid);
        //role.GetData().weaponid = instanceid;

        EquipData edata = UserDataManager.instance.GetEquip(instanceid);
        role.ResetAllProperty();
    }
    //捡装备
    public void AddEquip(EquipData ed){
        ed.roleid = -role.instanceId;
        UserDataManager.instance.AddEquip(ed);

        EventData.CreateEvent(EventID.EVENT_UI_REFRESH_PACKAGEUI).Send();
    }

    public void CancelEquip(int unequipinstanceid){

        UserDataManager.instance.CancelEquip(unequipinstanceid);
        role.ResetAllProperty();

        EventData.CreateEvent(EventID.EVENT_UI_REFRESH_PACKAGEUI).Send();
    }


    public override void HandleEvent(EventData resp)
    {
        if (role.IsDie()) return;
        switch (resp.eid)
        {
            case EventID.EVENT_GAME_TOUCH_GROUND:
                EventGameTouchGround eve = (EventGameTouchGround)resp;

                role.StartRun(eve.pos);
                break;
            case EventID.EVENT_GAME_TOUCH_OBJ:
                EventData touchobje = (EventData)resp;
                InGameBaseObj obj = (InGameBaseObj) touchobje.sUserData[0];

                if (obj.instanceId != role.instanceId && !obj.IsDie())
                {
                    role.StartAtk(obj);
                }

                break;
            case EventID.EVENT_UI_EQUIP:
                int instanceid = int.Parse(resp.sUserData[0].ToString());
                AddEquip(instanceid);
                EventData.CreateEvent(EventID.EVENT_UI_REFRESH_PACKAGEUI).Send();
                break;
            case EventID.EVENT_UI_DELEQUIP:
                int delinstanceid = int.Parse(resp.sUserData[0].ToString());
                Debug.Log("delinstanceid : " + delinstanceid);
                UserDataManager.instance.DelEquip(delinstanceid);
                EventData.CreateEvent(EventID.EVENT_UI_REFRESH_PACKAGEUI).Send();
                break;

            case EventID.EVENT_UI_UNEQUIP:
                int unequipinstanceid = int.Parse(resp.sUserData[0].ToString());
                CancelEquip(unequipinstanceid);
                break;
        }

    }

    private void OnDestroy()
    {
        EventManager.Remove(this);
    }

}
