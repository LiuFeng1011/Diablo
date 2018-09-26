using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIPlayerInfoManager : BaseGameObject {
	Transform playerInfo;
    GameObject playerInfoUnit;
	Dictionary<int,InGameUIPlayerInfoUnit> infoList = new Dictionary<int,InGameUIPlayerInfoUnit>();

	public void Init(Transform playerInfo){
        playerInfoUnit = Resources.Load("Prefabs/UI/CharactorInfoUI") as GameObject;

		this.playerInfo = playerInfo;

        EventManager.Register(this,
                              EventID.EVENT_GAME_CHARACTER_BORN,
                              EventID.EVENT_GAME_CHARACTER_DIE);
	}

	//Update
	public void InGameUIPlayerInfoManagerUpdate(){
        //		Dictionary<int,InGameRole> playerList = InGameManager.instance.playerManager.GetPlayerList();

		//寻找玩家
		foreach (KeyValuePair<int,InGameUIPlayerInfoUnit> kv in infoList)
		{
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

    public override void HandleEvent(EventData resp)
    {
        switch (resp.eid)
        {
            case EventID.EVENT_GAME_CHARACTER_BORN:

                InGameBaseCharacter bornobj = (InGameBaseCharacter)resp.sUserData[0];
                this.AddRole(bornobj);
                break;
            case EventID.EVENT_GAME_CHARACTER_DIE:
                InGameBaseCharacter dieobj = (InGameBaseCharacter)resp.sUserData[0];
                this.DelRole(dieobj.instanceId);
                break;
        }
    }
}
