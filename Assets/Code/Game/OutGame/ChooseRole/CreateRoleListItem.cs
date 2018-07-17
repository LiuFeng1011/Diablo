using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRoleListItem : MonoBehaviour {

    public UILabel nameLabel;
    CharacterConf conf;
	// Use this for initialization
    public void Init(CharacterConf conf){
        this.conf = conf;

        nameLabel.text = conf.name;

        GameUIEventListener.Get(gameObject).onClick = ChooseSelf;
    }
	
    void ChooseSelf(GameObject go){
        //Debug.Log(conf.name);
        ChooseRoleSceneManager.GetInstance().createRoleLayerManager.SelRole(conf.id);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
