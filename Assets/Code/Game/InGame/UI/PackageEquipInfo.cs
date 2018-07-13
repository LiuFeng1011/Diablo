using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageEquipInfo : BaseUnityObject {
    public GameObject content;
    public UILabel equipNameLabel, equipPropertyLabel;
    public GameObject delBtn,unequipBtn,equipBtn;
    public UISprite iconSprite;

    EquipData data;

    public void Start(){
        GameUIEventListener.Get(delBtn).onClick = DelCB;
        GameUIEventListener.Get(equipBtn).onClick = EquipCB;
        GameUIEventListener.Get(unequipBtn).onClick = UnEquipCB;
        SetData(null);
    }
    public void ResetData(){
        SetData(data);
    }
    public void SetData(EquipData data){
        this.data = data;
        if(data == null){
            content.SetActive(false);
            return;
        }
        content.SetActive(true);

        EquipConf conf = ConfigManager.equipConfManager.dic[data.equipid];

        iconSprite.spriteName = conf.prefabname;
        equipNameLabel.text = conf.name;

        string propertyText = "";
        for (int i = 0; i < data.propertyList.Count; i++)
        {
            EquipProperty p = data.propertyList[i];
            PropertyConf pconf = ConfigManager.propertyConfManager.dataMap[p.id];

            propertyText += string.Format(pconf.des, p.val) + "\n";
        }

        equipPropertyLabel.text = propertyText;


        if (data.roleid > 0)
        {
            delBtn.SetActive(false);
            equipBtn.SetActive(false);
            unequipBtn.SetActive(true);
        }
        else
        {
            delBtn.SetActive(true);
            equipBtn.SetActive(true);
            unequipBtn.SetActive(false);
        }
    }

    void DelCB(GameObject go)
    {
        (new EventData(EventID.EVENT_UI_DELEQUIP)).AddData(data.instanceid).Send();

        SetData(null);
    }

    void EquipCB(GameObject go)
    {
        (new EventData(EventID.EVENT_UI_EQUIP)).AddData(data.instanceid).Send();
    }

    void UnEquipCB(GameObject go)
    {
        (new EventData(EventID.EVENT_UI_UNEQUIP)).AddData(data.instanceid).Send();
    }
}
