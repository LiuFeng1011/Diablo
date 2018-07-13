using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSceneAction : MonoBehaviour {
    float maxActionTime = 0.4f;
    float actionTime = 0f;

    UISprite sprite;
    public static HideSceneAction Create(){
        GameObject uiroot = GameObject.Find("UI Root");
        if(uiroot == null){
            return null;
        }
        GameObject labelObj = NGUITools.AddChild(uiroot, Resources.Load("Prefabs/UI/HideSceneAction")as GameObject);
        HideSceneAction action = labelObj.GetComponent<HideSceneAction>();
        return action;
    }

	// Use this for initialization
	void Start () {
        sprite = gameObject.GetComponent<UISprite>();
        sprite.color = new Color(sprite.color.r,sprite.color.g,sprite.color.b,0);
	}
	
	// Update is called once per frame
	void Update () {
        if (actionTime >= maxActionTime) {
            //Destroy(gameObject);
            GameSceneManager.ChangeActionFinished();
            return;
        }
        actionTime += Time.deltaTime;

        float rate = actionTime / maxActionTime;
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, rate);
	}
}
