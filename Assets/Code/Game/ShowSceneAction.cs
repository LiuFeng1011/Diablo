using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSceneAction : MonoBehaviour {
    float actionTime = 0f;
    public float maxActionTime = 0.4f;

    UISprite sprite;
	// Use this for initialization
	void Start () {
        sprite = gameObject.GetComponent<UISprite>();
	}
	
	// Update is called once per frame
	void Update () {
        actionTime += Time.deltaTime;

        float rate = actionTime / maxActionTime;

        if(rate >= maxActionTime){
            Destroy(gameObject);
            return;
        }

        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1-rate);
	}
}
