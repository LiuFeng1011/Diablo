using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject map = new GameObject("map");
        for (int i = 0; i < 50; i ++){
            GameObject column = (GameObject)Resources.Load("Prefabs/Map/map1_item_" + Random.Range(1,4));

            column = MonoBehaviour.Instantiate(column);
            column.transform.parent = map.transform;
            column.transform.position = new Vector3(Random.Range(0,5),Random.Range(0, 5),Random.Range(0, 5));

        }
        StaticBatchingUtility.Combine(map);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
