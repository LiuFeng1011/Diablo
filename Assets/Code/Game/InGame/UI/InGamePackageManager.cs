using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGamePackageManager : BaseWindowManager {
    public UIScrollView scrollview;
    public UIGrid grid;

    public List<EquipListItem> itemList = new List<EquipListItem>();
    public List<EquipListItem> itemEquipedList = new List<EquipListItem>();
    public GameObject equipListItem;

    public GameObject equipedList;

    PackageEquipInfo packageEquipInfo;


	// Use this for initialization
	void Start () {

        EventManager.Register(this,
                              EventID.EVENT_UI_PACKAGE_SELECT_ITEM,
                              EventID.EVENT_UI_REFRESH_PACKAGEUI);

        packageEquipInfo = transform.Find("EquipInfo").GetComponent<PackageEquipInfo>();

        for (int i = 0; i < UserDataManager.instance.GetPackageCount();  i++){
            GameObject obj = NGUITools.AddChild(grid.gameObject, equipListItem);
            EquipListItem item = obj.GetComponent<EquipListItem>();
            itemList.Add(item);
        }

        grid.repositionNow = true;
        grid.Reposition();
        scrollview.ResetPosition();

        RoleData rd = UserDataManager.instance.GetChooseRoleData();
        for (int i = 0; i < 9; i ++){
            GameObject obj = NGUITools.AddChild(equipedList, equipListItem);
            EquipListItem item = obj.GetComponent<EquipListItem>();
            itemEquipedList.Add(item);
            item.Init(null, null);

            //if(i >= rd.equipCount){
            //    item.iconSprite.color = new Color(100, 100, 100);
            //}

            obj.transform.localPosition = new Vector3(
                i % 3 * 160 - 160, 
                -i / 3 * 160, 
                0);
        }

        ReSetData();
	}

    public void ReSetData(){
        List<GoodsData> equipList = UserDataManager.instance.GetEquipList();


        int count = 0;
        for (int i = 0; i < itemList.Count; i ++){

            itemList[i].Init(null, null);
            if (i >= equipList.Count)
            {
                continue;
            }
            if (equipList[i].type == GoodsData.GoodsType.equip)
            {
                EquipData equip = (EquipData)equipList[i];
                EquipConf econf = ConfigManager.equipConfManager.dic[equip.equipid];

                if (equip.roleid > 0){ 
                    continue;
                }

                itemList[count].Init(equip, econf);
                count++;
            }
        }

        RoleData rd = UserDataManager.instance.GetChooseRoleData();

        for (int i = 0; i < itemEquipedList.Count; i ++){
            itemEquipedList[i].Init(null, null);

            if (i >= rd.choseEquipList.Count) continue;

            if (rd.choseEquipList[i] == -1) continue;
            EquipData edata = UserDataManager.instance.GetEquip(rd.choseEquipList[i]);
            EquipConf econf = ConfigManager.equipConfManager.dic[edata.equipid];

            itemEquipedList[i].Init(edata, econf);

        }

        packageEquipInfo.ResetData();
    }

	// Update is called once per frame
	void Update () {
		
	}
    public override void HandleEvent(EventData resp)
    {
        switch(resp.eid){
            case EventID.EVENT_UI_PACKAGE_SELECT_ITEM:
                packageEquipInfo.SetData((EquipData)resp.sUserData[0]);
                break;
            case EventID.EVENT_UI_REFRESH_PACKAGEUI:
                ReSetData();
                break;

        }
    }

    private void OnDestroy()
    {
        EventManager.Remove(this);
    }
}
