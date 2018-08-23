using UnityEngine;
using System.Collections;

using UnityEditor;
using System.IO;
using System.Collections.Generic;

using LitJson;

public class MELoadLevelData : EditorWindow {

	public static List<string> fileDataList = new List<string>();
	
	public Vector2 scrollPosition = Vector2.zero;

	void Awake()
	{
        title = "载入关卡";
	}

	[MenuItem("关卡编辑器/载入关卡")]
	public static void OpenLoadLevel(){
		fileDataList.Clear();

        string[] files = Directory.GetFiles(GameConst.GetLevelDataFilePath());
		for(int i = 0 ; i < files.Length ; i++){
			string[] fileFolders = files[i].Split('/');

			string[] filenames = fileFolders[fileFolders.Length - 1].Split('.');

			if(filenames.Length == 2){
				if(filenames[1] == "bytes"){
					fileDataList.Add(filenames[0]);
				}
			}

		}
		Open();
	}

	[MenuItem("关卡编辑器/新关卡")]
	public static void NewLevel(){


		List<GameObject> delarr = new List<GameObject>();
		bool isfind = false;
		foreach (GameObject sceneObject in Object.FindObjectsOfType(typeof(GameObject))){
            if( sceneObject.name == "MapEditor"){
                LevelOption lo = sceneObject.GetComponent<LevelOption>();
                lo.levelName = "newlevel";
                lo.version = GameCommon.GAME_DATA_VERSION;
                lo.mapWidth = 100;
                lo.mapHeight = 100;

				isfind = true;
			}else if(sceneObject.name != "Main Camera"){
				delarr.Add(sceneObject);

			}
		}
		foreach(GameObject obj in delarr){
			DestroyImmediate(obj);
		}
		if(!isfind){
            GameObject tGO = new GameObject("MapEditor");  
            LevelOption lo = tGO.AddComponent<LevelOption>();
            lo.levelName = "newlevel";
            lo.version = GameCommon.GAME_DATA_VERSION;
            lo.mapWidth = 100;
            lo.mapHeight = 100;
            MapEditor me = tGO.AddComponent<MapEditor>();
            me.lo = lo;
		}
	}

	public static EditorWindow Open()
	{
		MELoadLevelData window = (MELoadLevelData)GetWindow<MELoadLevelData>();
		window.Show();
		return window;
	}

	public static EditorWindow Closeed(){
		MELoadLevelData window = (MELoadLevelData)GetWindow<MELoadLevelData>();
		window.Close();
		return window;
	}

	void OnGUI()
	{
		scrollPosition = GUILayout.BeginScrollView(
			scrollPosition,
			GUILayout.Width(position.width),
			GUILayout.Height(position.height)
			);
		
		
		for(int i = 0 ;i < fileDataList.Count ; i ++){
			if (GUILayout.Button(fileDataList[i], GUILayout.Width(220), GUILayout.Height(32)))
			{
				SelectLevel(i);
			}
		}
		GUILayout.EndScrollView();
		
	}

	void SelectLevel(int index){
		Debug.Log("sel index : " + index);
		LoadLevel(fileDataList[index]);
	}

	public void ReloadScene(DataStream datastream){

		Debug.Log("ReloadScene");

        LevelOption me = null;

		List<GameObject> delarr = new List<GameObject>();

		foreach (GameObject sceneObject in Object.FindObjectsOfType(typeof(GameObject))){
            if( sceneObject.name == "MapEditor"){
                me = sceneObject.GetComponent<LevelOption>();
                me.deserialize(datastream);
                GameCommon.GAME_DATA_VERSION = me.version;
                continue;
            }else if(sceneObject.name != "Main Camera" && sceneObject.name != "Directional Light"){
				delarr.Add(sceneObject);
			}
		}

		//创建关卡配置物体
		if(me == null){
            GameObject tGO = new GameObject("MapEditor");
            me = tGO.AddComponent<LevelOption>();
            me.deserialize(datastream);

            MapEditor me1 = tGO.AddComponent<MapEditor>();
            me1.lo = me;
		}

        Debug.Log(me.levelName + " / " + me.version);

		foreach(GameObject obj in delarr){
			DestroyImmediate(obj);
		}
		int objcount = datastream.ReadSInt32();

        Debug.Log("objcount : " + objcount);
        Dictionary<int, InGameBaseObj> dic = new Dictionary<int, InGameBaseObj>();
        MapObjConfManager mapObjConfManager = ConfigManager.GetEditorMapObjConfManager();

		for(int i = 0 ; i < objcount ;i ++){
            //MSBaseObject.CreateObj(datastream);
            //从字节流中获取id

            int confid = 0;
            float x = 0, y = 0;

            MapObjConf conf = null;
            GameObject go = null,tempObj;
            InGameBaseObj baseObj = null;
            int dataid = datastream.ReadByte();
            while(dataid != 0){
                switch(dataid){
                    case 1: 
                        confid = datastream.ReadSInt32();
                        conf = mapObjConfManager.map[confid];
                        tempObj = Resources.Load(conf.path) as GameObject;
                        if (tempObj == null)
                        {
                            Debug.Log(confid + " is null!");
                        }
                        go = (GameObject)Instantiate(tempObj);
                        baseObj = go.GetComponent<InGameBaseObj>();
                        break;
                    case 2: x = datastream.ReadSInt32() / 1000f;break;
                    case 3: y = datastream.ReadSInt32() / 1000f;break;
                    case 4: 
                        int parentid = datastream.ReadSInt32();
                        if (dic.ContainsKey(parentid)){
                            go.transform.parent = dic[parentid].transform;
                        }else{
                            go.transform.parent = me.transform;
                        }
                        break;
                    case 5:
                        int instanceid = datastream.ReadSInt32();
                        dic.Add(instanceid, baseObj);
                        break;
                    case 7: go.name = datastream.ReadString16(); break;
                    case 6: baseObj.Deserialize(datastream); break;
                }
                dataid = datastream.ReadByte();
            }

            go.transform.position = new Vector3(x,y);
            GameCommon.SetObjZIndex(go, conf.depth);
		}
	}

	public void LoadLevel(string fileName){
        byte[] levelGzipData = GameCommon.ReadByteToFile(GameConst.GetLevelDataFilePath(fileName+".bytes"));
		byte[] levelData = GameCommon.UnGZip(levelGzipData);
		DataStream datastream = new DataStream(levelData,true);

		ReloadScene(datastream);
		Closeed();
	}



}
