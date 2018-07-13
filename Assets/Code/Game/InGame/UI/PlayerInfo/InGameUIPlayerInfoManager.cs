using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIPlayerInfoManager {
	Transform playerInfo;
    GameObject playerInfoUnit;
	Dictionary<int,InGameUIPlayerInfoUnit> infoList = new Dictionary<int,InGameUIPlayerInfoUnit>();

	public void Init(Transform playerInfo){
        playerInfoUnit = Resources.Load("Prefabs/UI/CharactorInfoUI") as GameObject;

		this.playerInfo = playerInfo;

	}

	//Update
	public void InGameUIPlayerInfoManagerUpdate(){
        //		Dictionary<int,InGameRole> playerList = InGameManager.instance.playerManager.GetPlayerList();

		//寻找玩家
		foreach (KeyValuePair<int,InGameUIPlayerInfoUnit> kv in infoList)
		{
			if(!GameCommon.IsPositionInScreen(kv.Value.role.transform.position)){
				kv.Value.gameObject.SetActive(false);
				continue;
			}
			kv.Value.gameObject.SetActive(true);
			kv.Value.InGameUIPlayerInfoUnitUpdate();
		}
	}

    public void AddRole(InGameBaseCharacter role){
        if(infoList.ContainsKey(role.instanceId)){
			return;
		}
		CreateInfo(role);
	}

    public void DelRole(int instanceid){
        if (!infoList.ContainsKey(instanceid))
        {
            return;
        }
        MonoBehaviour.Destroy(infoList[instanceid].gameObject);
        infoList.Remove(instanceid);
    }

    void CreateInfo(InGameBaseCharacter role){

        GameObject roleGo = NGUITools.AddChild(playerInfo.gameObject,playerInfoUnit);

		InGameUIPlayerInfoUnit unit = roleGo.GetComponent<InGameUIPlayerInfoUnit>();
		unit.Init(role);
        infoList.Add(role.instanceId,unit);
	}
}
