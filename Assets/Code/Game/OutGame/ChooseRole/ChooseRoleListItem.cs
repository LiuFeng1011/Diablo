using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseRoleListItem : ScrollViewItem {

    public UILabel nameLabel;
    int instanceid;

    void Start(){
        GameUIEventListener.Get(gameObject).onClick = ChooseSelf;
    }

    void ChooseSelf(GameObject go)
    {
        //Debug.Log(conf.name);
        ChooseRoleSceneManager.GetInstance().chooseRoleLayerManager.ChooseRole(instanceid);
    }

    public override void SetData(ScrollViewItemData data){  
        this.data = data;
        RoleData d = (RoleData)data.data;
        instanceid = d.instanceid;

        nameLabel.text = d.name;
    }  
  
	
	// Update is called once per frame
	void Update () {
		
	}
}
