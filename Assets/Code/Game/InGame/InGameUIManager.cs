using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : InGameBaseManager {
    InGamePadManager inGamePadManager;
    InGamePackageManager inGamePackageManager;
    InGameBigMapManager inGameBigMapManager;

    InGameUIPlayerInfoManager playerInfo;
    public GameObject gameUICanvas;

    public override void Init()
    {
        if (!GameCommon.GAME_INIT) return;
        base.Init();

        EventManager.Register(this,
                              EventID.EVENT_UI_OPENPACKAGE,
                              EventID.EVENT_UI_OPENBIGMAP);

        GameObject canvas = GameObject.Find("UI Root");
        gameUICanvas = canvas;

        inGamePadManager = canvas.transform.Find("GamePad").GetComponent<InGamePadManager>();
        inGamePadManager.gameObject.SetActive(true);
        inGamePadManager.Init();

        inGamePackageManager = canvas.transform.Find("Package").GetComponent<InGamePackageManager>();
        inGamePackageManager.gameObject.SetActive(false);
        inGamePackageManager.Init();

        inGameBigMapManager = canvas.transform.Find("BigMap").GetComponent<InGameBigMapManager>();
        inGameBigMapManager.gameObject.SetActive(false);
        inGameBigMapManager.Init();

        GameObject playerinfoObj = new GameObject("PlayerInfo");
        Transform PlayerInfo = playerinfoObj.transform;
        PlayerInfo.parent = canvas.transform;

        PlayerInfo.transform.position = Vector3.zero;
        PlayerInfo.transform.localScale = new Vector3(1,1,1);

        playerInfo = new InGameUIPlayerInfoManager();
        playerInfo.Init(PlayerInfo);

    }

    public override void HandleEvent(EventData resp)
    {
        switch(resp.eid){
            case EventID.EVENT_UI_OPENPACKAGE:
                inGamePackageManager.Show();
                break;
            case EventID.EVENT_UI_OPENBIGMAP:
                inGameBigMapManager.Show();
                break;
        }
    }

    public override void Update(){

        //更新玩家信息
        playerInfo.InGameUIPlayerInfoManagerUpdate();

        inGamePadManager.MUpdate();
        inGamePackageManager.MUpdate();
        inGameBigMapManager.MUpdate();
    }

    //增加了一个玩家
    //public void AddRole(InGameBaseCharacter role)
    //{
    //    playerInfo.AddRole(role);
    //}
    //public void DelRole(int instanceid)
    //{
    //    playerInfo.DelRole(instanceid);
    //}

    public override void Destory()
    {
        inGamePadManager.Destory();
        inGameBigMapManager.Destory();
        playerInfo.Destory();
    }

}
