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

    public InGameBaseEquip OutEquip(GameObject go,int level,float fm){

        List<EquipConf> datas = ConfigManager.equipConfManager.datas;

        EquipConf equipConf = datas [Random.Range(0, datas.Count)];

        GameObject equipobj = (GameObject)Resources.Load(equipConf.prefabpath + equipConf.prefabname);
        equipobj = MonoBehaviour.Instantiate(equipobj);
        equipobj.transform.position = GameCommon.GetWorldPos(GameCommon.GetMapPos(go.transform.position));
        GameCommon.SetObjZIndex(equipobj,3);

        InGameBaseEquip equip = equipobj.GetComponent<InGameBaseEquip>();
        equip.Init(-1,equipConf.id,enMSCamp.en_camp_item);
        equip.level = level;
        equip.quality = ConfigManager.equipQualityManager.GetEquipQuality(fm);

        return equip;
    }

    public EquipData RandEquipProperty(InGameBaseEquip equip){
        
        EquipConf equipConf = ConfigManager.equipConfManager.dic[equip.confid];

        List<PropertyConf> propertys = ConfigManager.propertyConfManager.datas;
        List<EquipProperty> propertyList = new List<EquipProperty>();

        EquipQuality quality = equip.quality;

        for (int i = 0; i < quality.propertycount; i++)
        {
            PropertyConf propertyConf = propertys[Random.Range(0, propertys.Count)];
            EquipProperty e = new EquipProperty();
            e.id = propertyConf.id;
            float val = propertyConf.baseval + propertyConf.levelval * equip.level;
            e.val = Mathf.Ceil(val + Random.Range(-val * propertyConf.randomrange, val * propertyConf.randomrange));
            propertyList.Add(e);
        }

        EquipData equipData = new EquipData(
            UserDataManager.instance.GetInstanceID(),
            quality.id,
            equipConf.id,
            -1,
            propertyList);
        
        return equipData;
        //UserDataManager.instance.AddEquip(equipData);
    }
}
