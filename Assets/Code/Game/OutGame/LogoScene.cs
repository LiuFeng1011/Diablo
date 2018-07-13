using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Invoke("ChangeScene",0.1f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeScene(){
        (new EventChangeScene(GameSceneManager.SceneTag.Menu)).Send();
    }
}
