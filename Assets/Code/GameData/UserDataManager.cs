using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using LitJson;

public class UserDataManager : BaseSingleton<UserDataManager> {

    public enum UserDataType{
        UserData,
        UserDataPackage,
        UserDataStroage,
        UserDataRole,
        UserDataItemList,
    }

    UserData userData = new UserData();
    UserDataPackage userDataPackage = new UserDataPackage();
    UserDataStroage userDataStroage = new UserDataStroage();
    UserDataRole userDataRole = new UserDataRole();
    UserDataItemList userDataItemList = new UserDataItemList();

	public void Start(){
        Debuger.Log("===========初始化用户数据系统=============");

        LoadData(userData,UserDataType.UserData);
        LoadData(userDataPackage, UserDataType.UserDataPackage);
        LoadData(userDataStroage, UserDataType.UserDataStroage);
        LoadData(userDataRole, UserDataType.UserDataRole);
        LoadData(userDataItemList, UserDataType.UserDataItemList);

        Debuger.Log("----------用户数据系统初始化完成-------------");
	}

    public int GetInstanceID(){
        int ret = userData.instanceID;
        userData.instanceID += 1;
        SaveData(userData);
        return ret;
    }

    public void ClearData()
    {
        userData = new UserData();
        userDataPackage = new UserDataPackage();
        userDataStroage = new UserDataStroage();
        userDataRole = new UserDataRole();
        userDataItemList = new UserDataItemList();

        SaveData(userData);
        SaveData(userDataPackage);
        SaveData(userDataStroage);
        SaveData(userDataRole);
        SaveData(userDataItemList);
    }


    //获取选择的英雄id
    public int GetChooseRoleID()
    {
        return userData.chooseRoleId;
    }
    //选择英雄
    public void ChooseRole(int id)
    {
        userData.chooseRoleId = id;
        SaveData(userData);
    }
    //获取选中的英雄id
    public RoleData GetChooseRoleData()
    {
        return GetRoleData(userData.chooseRoleId);
    }

    public int GetPackageCount(){
        return userData.packageCount;
    }
    //=========================ROLE=============================

    //获取英雄列表
    public List<RoleData> GetRoleDataList(){
        return userDataRole.roleList;
    }
    //获取英雄数据
    public RoleData GetRoleData(int id){
        for (int i = 0; i < userDataRole.roleList.Count; i++)
        {
            if (userDataRole.roleList[i].instanceid == id)
            {
                return userDataRole.roleList[i];
            }
        }
        RoleData ret = new RoleData();
        ret.instanceid = -1;
        return ret;
    }
    //删除英雄
    public void DelRole(int instanceid){
        for (int i = 0; i < userDataRole.roleList.Count; i++)
        {
            if (userDataRole.roleList[i].instanceid == instanceid)
            {
                userDataRole.roleList.RemoveAt(i);
            }
        }

        if(userData.chooseRoleId == instanceid){
            if(userDataRole.roleList.Count > 0){
                userData.chooseRoleId = userDataRole.roleList[0].instanceid;
            }else{
                userData.chooseRoleId = -1;
            }
        }

        SaveData(userData,userDataRole);
    }

    //创建英雄
    public void AddRole(RoleData data){
        userData.chooseRoleId = data.instanceid;
        userDataRole.roleList.Add(data);
        SaveData(userData,userDataRole);
    }
    //穿装备
    public bool AddEquip(int roleid,int equipid){
        RoleData data = GetRoleData(roleid);
        EquipData edata = GetEquip(equipid);

        Debug.Log("AddEquip : " + edata.roleid);
        if (edata.roleid > 0) return false;

        Debug.Log("====== ");
        bool isadd = data.AddEquip(equipid);

        if(!isadd){
            Debug.Log("no add ");
            return false;
        }

        edata.roleid = roleid;

        SaveData(userDataRole,userDataItemList);
        return true;
    }
    //脱下装备
    public bool CancelEquip(int equipid)
    {
        EquipData edata = GetEquip(equipid);
        if (edata.roleid <= 0) return false;

        RoleData data = GetRoleData(edata.roleid);
        data.CancelEquip(equipid);

        edata.roleid = -edata.roleid;

        SaveData(userDataRole, userDataItemList);
        
        return true;
    }
    public void SaveRoleData(){
        SaveData(userDataRole);
    }

    //----level exp----
    public void AddExp(int roleid,int count){
        RoleData data = GetRoleData(roleid);
        int exp = data.exp + count;
        data.exp = exp;
        if(!ConfigManager.levelExpManager.dataMap.ContainsKey(data.level)){
            return;
        }
        LevelExp leveldata = ConfigManager.levelExpManager.dataMap[data.level];
        while(leveldata.exp <= exp){
            exp -= leveldata.exp;
            data.exp = exp;
            data.level++;
            data.levelupPoint++;
            if (!ConfigManager.levelExpManager.dataMap.ContainsKey(data.level))
            {
                break;
            }
            leveldata = ConfigManager.levelExpManager.dataMap[data.level];

            EventData.CreateEvent(EventID.EVENT_GAME_ROLR_LEVELUP).AddData(data).Send();
        }

        SaveData(userDataRole);

        EventData.CreateEvent(EventID.EVENT_DATA_CHANGEEXP).AddData(data).Send();

    }

    //使用升级点
    public void UseLevelupProperty(int roleid,int type){
        RoleData data = GetRoleData(roleid);

        if(data.levelupPoint <= 0){
            return;
        }
        data.levelupPoint--;
        AddProperty(roleid,type,1);
    }

    public void AddProperty(int roleid, int type, int count)
    {
        RoleData data = GetRoleData(roleid);

        float val = count;
        if (data.additionPropertyList.ContainsKey(type))
        {
            data.additionPropertyList[type] += val;
        }
        else
        {
            data.additionPropertyList.Add(type, val);
        }

        SaveData(userDataRole);

        EventData.CreateEvent(EventID.EVENT_DATA_REFRESHPROPERTY).Send();
    }
    //=========================EQUIP=============================

    public void AddEquip(EquipData e){
        userDataItemList.itemList.Add(e);
        SaveData(userDataItemList);
    }

    public bool DelEquip(int instanceid){
        for (int i = 0; i < userDataItemList.itemList.Count; i++){
            if(userDataItemList.itemList[i].type == GoodsData.GoodsType.equip){
                EquipData equip = (EquipData)userDataItemList.itemList[i];

                if (equip.instanceid == instanceid)
                {
                    if (equip.roleid > 0) return false;
                    userDataItemList.itemList.RemoveAt(i);
                    SaveData(userDataItemList);
                    return true;
                }
            }
        }
        return false;
    }

    public EquipData GetEquip(int instanceid){
        for (int i = 0; i < userDataItemList.itemList.Count; i++)
        {
            if (userDataItemList.itemList[i].type != GoodsData.GoodsType.equip) continue;
            EquipData equip = (EquipData)userDataItemList.itemList[i];

            if (equip.instanceid == instanceid)
            {
                return equip;
            }
        }

        return null;
    }

    public List<GoodsData> GetEquipList(){
        return userDataItemList.itemList;
    }

	//======================================================

    public void LoadData(BaseUserData userdata, UserDataType type ){
        string dataname = type + "";
        string filepath = GameConst.GetPersistentDataPath(dataname);
		byte[] gzipdata = GameCommon.ReadByteToFile(filepath);

		if( gzipdata == null){
			
		}else{
            byte[] data = GameCommon.UnGZip(gzipdata);
            DataStream datastream = new DataStream(data, true);

            userdata.Deserialize(datastream);
		}

	}

    public void SaveData(params BaseUserData[] datas){
        for (int i = 0; i < datas.Length; i ++){
            DataStream stream = new DataStream(true);
            datas[i].Serialize(stream);

            byte[] gzipData = GameCommon.CompressGZip(stream.ToByteArray());
            GameCommon.WriteByteToFile(gzipData, GameConst.GetPersistentDataPath(datas[i].GetDataType() + ""));
        }

	}
}
