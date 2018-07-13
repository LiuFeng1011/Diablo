using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyListItem : BaseUnityObject {
    public PropertyConf conf;

    public UILabel desLabel;


    public void Init(PropertyConf conf,float val){
        this.conf = conf;
        desLabel.text = string.Format(conf.boardDes, val.ToString("f2"));


    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
