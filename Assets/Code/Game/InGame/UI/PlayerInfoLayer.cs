using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoLayer : BasePopLayer {
    
    public UILabel nameLabel;
    public UILabel propertyLabel;

    public UIScrollView scrollview;
    public UIGrid grid;

    public GameObject propertyItem;
    public GameObject mainPropertyItem;

    List<PropertyListItem> itemList = new List<PropertyListItem>();

    public static PlayerInfoLayer Create()
    {
        GameObject playerInfoLayer = NGUITools.AddChild(
            GameObject.Find("UI Root"), Resources.Load("Prefabs/UI/PlayerInfoLayer") as GameObject);

        PlayerInfoLayer layer = playerInfoLayer.GetComponent<PlayerInfoLayer>();


        layer.Init();
        layer.Show();
        return layer;

    }

    public void Init()
    {
        InGameBaseCharacter role = InGameManager.GetInstance().inGamePlayerManager.GetRole();

        nameLabel.text = role.GetData().name;

        Dictionary<int,PropertyConf> propertyMap = ConfigManager.propertyConfManager.dataMap;
        Dictionary<int,List<PropertyConf>> mainPropertyMap = ConfigManager.propertyConfManager.mainDataMap;

        foreach(KeyValuePair<int,List<PropertyConf>> kv in mainPropertyMap){

            CreateProperty(mainPropertyItem,propertyMap[kv.Key],role);

            List<PropertyConf> list = kv.Value;
            for (int i = 0; i < list.Count;  i++){
                CreateProperty(propertyItem,list[i], role);
            }
        }

    }

    public void CreateProperty(GameObject prefab , PropertyConf conf, InGameBaseCharacter role){

        GameObject obj = NGUITools.AddChild(grid.gameObject, prefab);
        PropertyListItem item = obj.GetComponent<PropertyListItem>();
        item.Init(conf, role.propertys.propertyValues[conf.id]);
        itemList.Add(item);
    }

}
