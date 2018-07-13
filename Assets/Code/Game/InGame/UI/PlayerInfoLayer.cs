using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoLayer : BasePopLayer {
    
    public UILabel nameLabel;
    public UILabel propertyLabel;

    public UIScrollView scrollview;
    public UIGrid grid;

    public GameObject propertyItem;

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

        //string propertyString = "";

        List<PropertyConf> propertyList = ConfigManager.propertyConfManager.datas;

        for (int i = 0; i < propertyList.Count; i ++){
            PropertyConf conf = propertyList[i];

            //propertyString += string.Format(conf.des, role.propertys.propertyValues[conf.id]);
            //propertyString += "\n";

            GameObject obj = NGUITools.AddChild(grid.gameObject, propertyItem);
            PropertyListItem item = obj.GetComponent<PropertyListItem>();
            item.Init(conf, role.propertys.propertyValues[conf.id]);

            itemList.Add(item);
        }
        //propertyLabel.text = propertyString;
    }



}
