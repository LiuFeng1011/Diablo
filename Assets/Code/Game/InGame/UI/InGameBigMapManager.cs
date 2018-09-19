using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBigMapManager : BaseWindowManager {

    const int mapScale = 6;

    public UITexture uitexture;
    public override void Init()
    {
        base.Init();
        EventManager.Register(this,
                              EventID.EVENT_UI_INITMAPTEXTURE);
    }
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void HandleEvent(EventData resp)
    {
        switch (resp.eid)
        {
            case EventID.EVENT_UI_INITMAPTEXTURE:
                Texture2D texture = resp.sUserData[0] as Texture2D;
                this.uitexture.mainTexture = texture;


                uitexture.width = texture.width * mapScale;
                uitexture.height = texture.height * mapScale;

                break;
        }
    }
}
