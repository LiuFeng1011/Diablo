using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyListItem : BaseUnityObject {
    public PropertyConf conf;

    public UILabel desLabel;

    public GameObject addBtn;

    public void Init(PropertyConf conf){
        this.conf = conf;
    }

    public void Refresh(){
        InGameBaseCharacter role = InGameManager.GetInstance().inGamePlayerManager.GetRole();

        desLabel.text = string.Format(conf.boardDes, role.propertys.propertyValues[conf.id].ToString("f2"));

        if(addBtn != null && role.GetData().levelupPoint > 0){
            GameUIEventListener.Get(addBtn).onClick = AddBtnCB;
        }
    }

    public void AddBtnCB(GameObject go){
        InGameBaseCharacter role = InGameManager.GetInstance().inGamePlayerManager.GetRole();

        EventData.CreateEvent(EventID.EVENT_DATA_USE_LEVELUP_POINT).AddData(role.GetData().instanceid,conf.id).Send();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
