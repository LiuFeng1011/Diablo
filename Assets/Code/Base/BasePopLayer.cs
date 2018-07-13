using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePopLayer : BaseUnityObject {

    public GameObject closeBtn;

    public virtual void Show(){
        GameObject maskLayer = NGUITools.AddChild(gameObject,Resources.Load("Prefabs/UI/maskbg") as GameObject);
        if(closeBtn){
            GameUIEventListener.Get(closeBtn).onClick = Hide;
        }
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Hide(GameObject go){
        Destroy(gameObject);
    }
}
