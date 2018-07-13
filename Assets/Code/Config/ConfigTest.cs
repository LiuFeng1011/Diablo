using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigTest : MonoBehaviour {


	// Use this for initialization
	void Start () {
		ConfigManager.LoadData();

        //Debug.Log(ConfigManager.confMapObjectManager.datas[1].ToString());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
