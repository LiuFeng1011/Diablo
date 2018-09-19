using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  
public class InGamePadManager : BaseWindowManager {

    const int mapScale = 6;

    public GameObject openPackageBtn;
    public GameObject playerInfoBtn;
    public GameObject smallMapBtn;

    public UITexture uitexture;

    public override void Init()
    {
        base.Init();

        GameUIEventListener.Get(openPackageBtn).onClick = OpenPackageBtnCB;
        GameUIEventListener.Get(playerInfoBtn).onClick = PlayerInfoBtnCB;
        GameUIEventListener.Get(smallMapBtn).onClick = SmallMapBtnCB;

        EventManager.Register(this,
                              EventID.EVENT_UI_INITMAPTEXTURE);
    }

    public override void MUpdate(){
        base.MUpdate();
            //更新小地图位置
        Vector3 rolePos = InGameManager.GetInstance().inGamePlayerManager.GetRolePos();
        Vector2 arrPos = GameCommon.GetMapPos(rolePos);

        float x = (float)arrPos.x / (float)uitexture.mainTexture.width;
        float y = (float)arrPos.y / (float)uitexture.mainTexture.height;

        uitexture.transform.localPosition = -1 * new Vector2( x * uitexture.width, y *uitexture.height );
    }

    public void OpenPackageBtnCB(GameObject obj)  
    {
        (new EventData(EventID.EVENT_UI_OPENPACKAGE)).Send();
    }

    public void PlayerInfoBtnCB(GameObject obj)
    {
        PlayerInfoLayer.Create();
    }
    public void SmallMapBtnCB(GameObject obj)
    {
        (new EventData(EventID.EVENT_UI_OPENBIGMAP)).Send();
        
    }

    public override void HandleEvent(EventData resp)
    {
        switch (resp.eid)
        {
            case EventID.EVENT_UI_INITMAPTEXTURE:
                Texture2D texture = resp.sUserData[0] as Texture2D;

                uitexture.mainTexture = texture;

                uitexture.width = texture.width * mapScale;
                uitexture.height = texture.height * mapScale;

                break;
        }
    }

}
