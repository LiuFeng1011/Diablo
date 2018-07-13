using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownEffect : MonoBehaviour {
    public AnimationCurve ac;
    float actionTime = 0;
    public float speed = 0.3f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (actionTime >= 1.0f) return;
        actionTime += (Time.deltaTime * (1 / speed));

        float rate = ac.Evaluate(actionTime);
        transform.localScale = new Vector3(rate, rate, 1);
	}
}
