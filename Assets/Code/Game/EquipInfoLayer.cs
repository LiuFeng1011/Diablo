using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipInfoLayer : BasePopLayer {
    public GameObject delBtn,equipBtn;
    public UISprite iconSprite;
    public UILabel titleLabel, typeLabel, basePropertyLabel,propertyLabel;
    EquipData data;

    public static EquipInfoLayer Create(EquipData data) {
        GameObject equipInfoLayer = NGUITools.AddChild(
            GameObject.Find("UI Root"), Resources.Load("Prefabs/UI/EquipInfoLayer") as GameObject);

        EquipInfoLayer layer = equipInfoLayer.GetComponent<EquipInfoLayer>();
        layer.Init(data);
        layer.Show();
        return layer;
        
    }

    public override void Show(){
        base.Show();
    }

    public void Init(EquipData data){
        this.data = data;
        GameUIEventListener.Get(delBtn).onClick = DelCB;
        GameUIEventListener.Get(equipBtn).onClick = EquipCB;

        EquipConf conf = ConfigManager.equipConfManager.dic[data.equipid];
        if (conf == null) return;

        iconSprite.spriteName = conf.prefabname;
        titleLabel.text = conf.name;

        basePropertyLabel.text = "";

        EquipProperty mainp = data.propertyList[0];
        PropertyConf mainpconf = ConfigManager.propertyConfManager.dataMap[mainp.id];

        typeLabel.text = "";
        //lvLabel.text = 1+"";

        string propertyText = "";
        for (int i = 1; i < data.propertyList.Count; i ++){
            EquipProperty p = data.propertyList[i];
            PropertyConf pconf = ConfigManager.propertyConfManager.dataMap[p.id];

            propertyText += string.Format(pconf.des, p.val) + "\n";
        }

        propertyLabel.text = propertyText;

        Show();
    }

    void DelCB(GameObject go){
        Hide(null);

        UserDataManager.instance.DelEquip(data.instanceid);
        (new EventData(EventID.EVENT_UI_DELEQUIP)).AddData(data.instanceid).Send();
    }

    void EquipCB(GameObject go){
        Hide(null);

        (new EventData(EventID.EVENT_UI_EQUIP)).AddData(data.instanceid).Send();

        //EquipConf econf = ConfigManager.equipConfManager.dic[data.equipid];
        //InGameManager.GetInstance().inGamePlayerManager.ChangerWeapon(econf);
    }

	// Use this for initialization
	void Start () {
        //Init();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
