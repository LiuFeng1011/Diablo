using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRoleLayerManager : MonoBehaviour {

    public UIInput inputLabel;

    public GameObject createRoleListItem;
    public GameObject createRoleGrid;

    int selid = -1;
	// Use this for initialization
	public void Init () {
        GameObject createBtn = transform.Find("Bottom").Find("CreateBtn").gameObject;
        GameUIEventListener.Get(createBtn).onClick = CreateCB;

        GameObject backBtn = transform.Find("Bottom").Find("BackBtn").gameObject;
        GameUIEventListener.Get(backBtn).onClick = BackCB;

        List<CharacterConf> roleList = ConfigManager.characterConfManager.dicByType[(int)CharacterConf.enCharacterType.character];

        for (int i = 0; i < roleList.Count; i++)
        {
            GameObject obj = NGUITools.AddChild(createRoleGrid, createRoleListItem);
            CreateRoleListItem item = obj.GetComponent<CreateRoleListItem>();
            item.Init(roleList[i]);
            if(selid == -1 ) selid = roleList[i].id;
        }
        createRoleGrid.GetComponent<UIGrid>().Reposition();
	}

    public void Show(){
        SelRole(selid);
    }

    void CreateCB(GameObject go){
        Debug.Log(selid + " / " + inputLabel.value);
        RoleData data = RoleData.Create(selid, inputLabel.value);

        UserDataManager.instance.AddRole(data);

        ChooseRoleSceneManager.GetInstance().ChooseRole();
    }

    void BackCB(GameObject go)
    {
        ChooseRoleSceneManager.GetInstance().ChooseRole();
    }

    public void SelRole(int id){
        selid = id;
        ChooseRoleSceneManager.GetInstance().ShowBaseRole(id);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
