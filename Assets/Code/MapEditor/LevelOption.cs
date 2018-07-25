using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelOption : MonoBehaviour {

	public string levelName = "";

	public float version = 0.0f;

	public int mapHeight = 20;
	public int mapWidth = 20;

	//public float autoDPos = 0.1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void serialize(DataStream stream){
        stream.WriteString16(levelName);
        stream.WriteSInt32((int)(version * 1000));
        stream.WriteSInt32(mapHeight);
        stream.WriteSInt32(mapWidth);
    }

    public void deserialize(DataStream stream)
    {
        levelName = stream.ReadString16();
        version = (float)stream.ReadSInt32() / 1000f;
        mapHeight = stream.ReadSInt32();
        mapWidth = stream.ReadSInt32();
    }
}
