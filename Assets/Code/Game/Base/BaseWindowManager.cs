using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWindowManager : BaseUnityObject {

    public GameObject closeBtn;

    public virtual void Init(){
        if(closeBtn != null) GameUIEventListener.Get(closeBtn).onClick = CloseBtn;
    }
	
	// Update is called once per frame
	public virtual void MUpdate () {
		
	}

    void CloseBtn(GameObject go){
        Hide();
    }

    public virtual void Show(){
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
