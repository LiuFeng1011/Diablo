using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  
public class InGamePadManager : BaseWindowManager {
    public GameObject openPackageBtn;
    public GameObject playerInfoBtn;

    private void Start()
    {
        GameUIEventListener.Get(openPackageBtn).onClick = OpenPackageBtnCB;
        GameUIEventListener.Get(playerInfoBtn).onClick = PlayerInfoBtnCB;

    }

    public void OpenPackageBtnCB(GameObject obj)  
    {
        (new EventData(EventID.EVENT_UI_OPENPACKAGE)).Send();
    }

    public void PlayerInfoBtnCB(GameObject obj)
    {
        PlayerInfoLayer.Create();
    }



}
