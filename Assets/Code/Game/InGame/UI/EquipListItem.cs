using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipListItem : MonoBehaviour {

    public UISprite iconSprite;
    public EquipConf econf;
    public EquipData edata;
	// Use this for initialization
	void Start () {
        GameUIEventListener.Get(gameObject).onClick = Click;
	}
    public void Init(EquipData edata, EquipConf econf){
        this.edata = edata;
        this.econf = econf;

        if (edata == null)
        {
            iconSprite.gameObject.SetActive(false);
            return;
        }
        iconSprite.gameObject.SetActive(true);
        iconSprite.spriteName = econf.prefabname;

    }

    void Click(GameObject go){
        if (edata == null) return;
        //InGameManager.GetInstance().inGamePlayerManager.ChangerWeapon(econf);
        //EquipInfoLayer.Create(edata);
        EventData.CreateEvent(EventID.EVENT_UI_PACKAGE_SELECT_ITEM).AddData(edata).Send();
    }
	// Update is called once per frame
	void Update () {
		
	}
}
