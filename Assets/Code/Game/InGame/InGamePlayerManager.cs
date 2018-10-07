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
                              EventID.EVENT_UI_UNEQUIP,
                              EventID.EVENT_DATA_REFRESHPROPERTY);


        RoleData rd = UserDataManager.instance.GetChooseRoleData();

        role = InGameManager.GetInstance().inGameObjManager.AddObj(rd.roldid,enMSCamp.en_camp_player) as InGameBaseCharacter;
        role.SetData(rd);

        //InGameManager.GetInstance().GetManager<InGameObjManager>(InGameManager.enGameManager.InGameObjManager).AddObj(0);
        Vector3 startpos = InGameManager.GetInstance().inGameLevelManager.gameMap.GetStartPosition();

        role.transform.position = GameCommon.GetWorldPos(startpos);
        role.SetZPos();

        InGameManager.GetInstance().inGameCameraManager.SetTarget(role.gameObject);

        for (int i = 0; i < rd.choseEquipList.Count; i++)
        {
            if (rd.choseEquipList[i] == -1) continue;
            EquipData edata = UserDataManager.instance.GetEquip(rd.choseEquipList[i]);
            role.AddEquip(edata);
        }

        //UserDataManager.instance.AddLevelupPoint(role.GetData().instanceid,5);
        role.ResetAllProperty(true);
        EventData.CreateEvent(EventID.EVENT_GAME_INITROLE).AddData(role).Send();

    }

    public InGameBaseCharacter GetRole(){
        return role;
    }

    // 穿装备
    public void AddEquip(int instanceid){
        
        if (instanceid == -1) return;
        bool isequip = UserDataManager.instance.AddEquip(role.GetData().instanceid, instanceid);
        //role.GetData().weaponid = instanceid;
        if (!isequip) return;
        EquipData edata = UserDataManager.instance.GetEquip(instanceid);
        role.AddEquip(edata);
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

        role.CancleEquip(unequipinstanceid);
        role.ResetAllProperty();

        EventData.CreateEvent(EventID.EVENT_UI_REFRESH_PACKAGEUI).Send();
    }

    public Vector3 GetRolePos(){

        if(role == null){
            return Vector3.zero;
        }
        return role.transform.position;
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
                UserDataManager.instance.DelEquip(delinstanceid);
                EventData.CreateEvent(EventID.EVENT_UI_REFRESH_PACKAGEUI).Send();
                break;

            case EventID.EVENT_UI_UNEQUIP:
                int unequipinstanceid = int.Parse(resp.sUserData[0].ToString());
                CancelEquip(unequipinstanceid);
                break;

            case EventID.EVENT_DATA_REFRESHPROPERTY:
                role.ResetAllProperty();
                break;
        }

    }

    private void OnDestroy()
    {
        EventManager.Remove(this);
    }

}
