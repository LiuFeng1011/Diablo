using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  
public class InGamePadManager : BaseWindowManager {

    const int mapScale = 12;

    public GameObject openPackageBtn;
    public GameObject playerInfoBtn;
    public GameObject smallMapBtn;

    public UISprite lifeLine,manaLine,expLine;
    public UILabel lifeLabel,manaLabel,expLabel;

    float lifeTargetVal, manaTargetVal, expTargetVal;

    public UITexture uitexture;

    public override void Init()
    {
        base.Init();

        GameUIEventListener.Get(openPackageBtn).onClick = OpenPackageBtnCB;
        GameUIEventListener.Get(playerInfoBtn).onClick = PlayerInfoBtnCB;
        GameUIEventListener.Get(smallMapBtn).onClick = SmallMapBtnCB;

        EventManager.Register(this,
                              EventID.EVENT_UI_INITMAPTEXTURE,
                              EventID.EVENT_GAME_INITROLE,
                              EventID.EVENT_DATA_CHANGELIFE,
                              EventID.EVENT_DATA_CHANGEEXP);
    }

    public override void MUpdate(){
        base.MUpdate();
            //更新小地图位置
        Vector3 rolePos = InGameManager.GetInstance().inGamePlayerManager.GetRolePos();
        Vector2 arrPos = GameCommon.GetMapPos(rolePos);

        float x = (float)arrPos.x / (float)uitexture.mainTexture.width;
        float y = (float)arrPos.y / (float)uitexture.mainTexture.height;

        uitexture.transform.localPosition = -1 * new Vector2( x * uitexture.width, y *uitexture.height );

        //update life

        if(Mathf.Abs(lifeTargetVal - lifeLine.fillAmount) > 0.01f){
            lifeLine.fillAmount = lifeLine.fillAmount + (lifeTargetVal - lifeLine.fillAmount) * 0.1f;
            if(Mathf.Abs(lifeTargetVal - lifeLine.fillAmount) <= 0.01f){
                lifeLine.fillAmount = lifeTargetVal;
            }
        }

        //update mana

        //update exp
        if (Mathf.Abs(expTargetVal - expLine.fillAmount) > 0.01f)
        {
            expLine.fillAmount = expLine.fillAmount + (expTargetVal - expLine.fillAmount) * 0.1f;
            if (Mathf.Abs(expTargetVal - expLine.fillAmount) <= 0.01f)
            {
                expLine.fillAmount = expTargetVal;
            }
        }
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

    void SetLife(InGameBaseCharacter role){
        float irmaxval = role.propertys.propertyValues[(int)enCharacterProperty.life];
        this.lifeTargetVal = role.life / irmaxval;
        lifeLabel.text = role.life + "/" + irmaxval;
    }

    void SetExp(RoleData role)
    {
        this.expTargetVal = role.exp / (float)ConfigManager.levelExpManager.dataMap[role.level].exp;
        expLabel.text = "Lv."+role.level;
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
            case EventID.EVENT_DATA_CHANGELIFE:
                InGameBaseCharacter obj = resp.sUserData[0] as InGameBaseCharacter;
                if(InGameManager.GetInstance().inGamePlayerManager.GetRole().instanceId == obj.instanceId){
                    SetLife(obj);
                }
                break;
            case EventID.EVENT_GAME_INITROLE:
                InGameBaseCharacter role = resp.sUserData[0] as InGameBaseCharacter;
                SetLife(role);
                SetExp(role.GetData());
                break;
            case EventID.EVENT_DATA_CHANGEEXP:
                RoleData roledata = resp.sUserData[0] as RoleData;
                SetExp(roledata);
                break;

        }
    }

    public void Destory()
    {
        EventManager.Remove(this);
    }
}
