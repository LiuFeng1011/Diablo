using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjFloatAction : MonoBehaviour {
    public float scalespeedx = 0.05f;
    public float scalespeedy = 0.03f;

    public float speedmin = 1f;
    public float speedmax = 2f;

    public float speed,time;
    Vector3 baseScale;
	// Use this for initialization
	void Start () {
        
        this.speed = Random.Range(speedmin,speedmax) ;
        baseScale = transform.parent.localScale;
	}
	
	// Update is called once per frame
	void Update () {

        this.time += Time.deltaTime * this.speed;

        float scaleX = baseScale.x - Mathf.Sin(this.time) * this.scalespeedx;
        float scaleY = baseScale.y + Mathf.Sin(this.time) * this.scalespeedy;

        transform.parent.localScale = new Vector3(scaleX,scaleY,1);

	}
}
