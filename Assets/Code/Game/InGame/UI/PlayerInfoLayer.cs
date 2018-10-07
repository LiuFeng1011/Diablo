using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoLayer : BasePopLayer {
    
    public UILabel nameLabel,levelLabel;
    public UILabel propertyLabel;

    public UILabel addPointLabel;
    public GameObject addPointInfoObj;


    public UIScrollView scrollview;
    public UIGrid grid;

    //PREFAB
    public GameObject propertyItem,mainPropertyItem,otherPropertyItem;

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
        levelLabel.text = role.level+"";

        Dictionary<int,PropertyConf> propertyMap = ConfigManager.propertyConfManager.dataMap;
        Dictionary<int,List<PropertyConf>> mainPropertyMap = ConfigManager.propertyConfManager.mainDataMap;

        foreach(KeyValuePair<int,List<PropertyConf>> kv in mainPropertyMap){

            if(kv.Key > 0){
                CreateProperty(mainPropertyItem, propertyMap[kv.Key], role);
            }else if(kv.Key == -2){
                CreateProperty(otherPropertyItem, null, role);
            }else{
                continue;
            }

            List<PropertyConf> list = kv.Value;
            for (int i = 0; i < list.Count;  i++){
                CreateProperty(propertyItem,list[i], role);
            }
        }

        RefreshUI();

        EventManager.Register(this,
                              EventID.EVENT_DATA_REFRESHPROPERTY);
    }

    public void CreateProperty(GameObject prefab , PropertyConf conf, InGameBaseCharacter role){

        GameObject obj = NGUITools.AddChild(grid.gameObject, prefab);
        PropertyListItem item = obj.GetComponent<PropertyListItem>();
        if(conf != null){
            item.Init(conf/*, role.propertys.propertyValues[conf.id]*/);
        }

        itemList.Add(item);
    }

    void RefreshUI(){
        Debug.Log("RefreshUI");
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].Refresh();
        }

        InGameBaseCharacter role = InGameManager.GetInstance().inGamePlayerManager.GetRole();

        //ADDPOING
        if(role.GetData().levelupPoint > 0){
            this.addPointInfoObj.SetActive(true);
            this.addPointLabel.text = "+" + role.GetData().levelupPoint;
        }else{
            this.addPointInfoObj.SetActive(false);
        }
    }

    public override void HandleEvent(EventData resp)
    {
        base.HandleEvent(resp);
        switch (resp.eid)
        {
            case EventID.EVENT_DATA_REFRESHPROPERTY:
                RefreshUI();
                break;
        }
    }
    private void OnDestroy()
    {
        EventManager.Remove(this);
    }

}
