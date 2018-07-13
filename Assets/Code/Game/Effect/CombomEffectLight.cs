using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombomEffectLight : MonoBehaviour {

    Light light;

    private void Awake()
    {
        light = transform.GetComponent<Light>();
    }
	
	// Update is called once per frame
	void Update () {
        light.range -= Time.deltaTime * 20;
	}
}
