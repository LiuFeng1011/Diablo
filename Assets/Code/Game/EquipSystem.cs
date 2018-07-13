using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSystem : BaseGameObject {

    private static EquipSystem instance;

    public static EquipSystem GetInstance(){
        if(instance == null){
            instance = new EquipSystem();
        }
        return instance;
    }

    public InGameBaseEquip OutEquip(GameObject go,int level){

        List<EquipConf> datas = ConfigManager.equipConfManager.datas;

        EquipConf equipConf = datas [Random.Range(0, datas.Count)];

        GameObject equipobj = (GameObject)Resources.Load(equipConf.prefabpath + equipConf.prefabname);
        equipobj = MonoBehaviour.Instantiate(equipobj);
        equipobj.transform.position = go.transform.position;

        InGameBaseEquip equip = equipobj.GetComponent<InGameBaseEquip>();
        equip.Init(-1,equipConf.id,enMSCamp.en_camp_item);
        equip.level = level;

        return equip;
    }

    public EquipData RandEquipProperty(int id,int level){
        
        EquipConf equipConf = ConfigManager.equipConfManager.dic[id];

        List<PropertyConf> propertys = ConfigManager.propertyConfManager.datas;
        List<EquipProperty> propertyList = new List<EquipProperty>();

        int propertyCount = Random.Range(1, 5);
        for (int i = 0; i < propertyCount; i++)
        {
            PropertyConf propertyConf = propertys[Random.Range(0, propertys.Count)];
            EquipProperty e = new EquipProperty();
            e.id = propertyConf.id;
            float val = propertyConf.baseval + propertyConf.levelval * level;
            e.val = Mathf.Ceil(val + Random.Range(-val * propertyConf.randomrange, val * propertyConf.randomrange));
            propertyList.Add(e);
        }

        EquipData equipData = new EquipData(
            UserDataManager.instance.GetInstanceID(),
            equipConf.id,
            -1,
            propertyList);
        
        return equipData;
        //UserDataManager.instance.AddEquip(equipData);
    }
}
