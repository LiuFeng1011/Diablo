using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseRoleLayerManager : MonoBehaviour {
    public ScrollViewManager scrollViewManager;
    public GameObject createRoleBtn, startBtn, delBtn;
    public int selinstanceid = -1;
    private void Start()
    {

        GameUIEventListener.Get(createRoleBtn).onClick = CreateRoleCB;

        GameUIEventListener.Get(startBtn).onClick = StartCB;

        GameUIEventListener.Get(delBtn).onClick = DelCB;

    }
    // Use this for initialization
    public void Init () {
        List<RoleData> roleList = UserDataManager.instance.GetRoleDataList();
        List<ScrollViewItemData> dataList = new List<ScrollViewItemData>();
        for (int i = 0; i < roleList.Count; i ++){
            //if (selinstanceid == -1) selinstanceid = roleList[i].instanceid;
            //GameObject obj = NGUITools.AddChild(chooseRoleGrid, chooseRoleListItem);
            //ChooseRoleListItem item = obj.GetComponent<ChooseRoleListItem>();
            //item.Init(roleList[i]);
            ScrollViewItemData data = new ScrollViewItemData(i,roleList[i]);
            dataList.Add(data);
        }
        scrollViewManager.Init(dataList);

        if(UserDataManager.instance.GetChooseRoleID() != -1){
            selinstanceid = UserDataManager.instance.GetChooseRoleID();
        }

        ChooseRole(selinstanceid);
	}

    void CreateRoleCB(GameObject go){
        ChooseRoleSceneManager.GetInstance().CreateRole();
    }

    void StartCB(GameObject go){
        UserDataManager.instance.ChooseRole(selinstanceid);
        (new EventChangeScene(GameSceneManager.SceneTag.Game)).Send();
    }

    void DelCB(GameObject go)
    {
        UserDataManager.instance.DelRole(selinstanceid);

        Init();
    }
    public void ChooseRole(int instanceid){
        selinstanceid = instanceid;
        ChooseRoleSceneManager.GetInstance().ShowPlayerRole(instanceid);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
