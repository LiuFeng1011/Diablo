﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;


public class MESaveLevelData : Editor
{
    public static List<InGameBaseObj> msobjlist = new List<InGameBaseObj>();

	[MenuItem("关卡编辑器/保存关卡")]
	public static void SaveLevel(){
		Debug.Log("SaveLevel");
		msobjlist.Clear();
		// 场景路径
		/*
		string scenePath = AssetDatabase.GetAssetPath(selectObject);
		
		Debug.Log("=====================================");
		Debug.Log(sceneName + "   path : " + scenePath );
		// 打开这个关卡
		EditorApplication.OpenScene(scenePath);
		*/
		
		DataStream stream = new DataStream(true);

		//获取场景中全部道具
		Object[] objects = Object.FindObjectsOfType(typeof(GameObject));
        LevelOption lo = null;
		foreach (GameObject sceneObject in objects){
            if(sceneObject.name == "MapEditor"){
				lo = sceneObject.GetComponent<LevelOption>();
                if(lo != null){
                    lo.serialize(stream);
                    GameCommon.GAME_DATA_VERSION = lo.version;
				}

				ForeachObjAndSave(sceneObject);
			}
		}
        if(lo == null){
            Debug.LogError("Can't find LevelOption!!");
            return;
        }
		Debug.Log("save list count:"+msobjlist.Count);
		stream.WriteSInt32(msobjlist.Count);

        foreach(InGameBaseObj msobj in msobjlist){

            stream.WriteByte(1);
            stream.WriteSInt32(msobj.confid);
            int x = (int)(msobj.transform.position.x * 1000f);
            int y = (int)(msobj.transform.position.y * 1000f);
            stream.WriteByte(2);
            stream.WriteSInt32(x);
            stream.WriteByte(3);
            stream.WriteSInt32(y);
            stream.WriteByte(4);
            stream.WriteSInt32(msobj.parentid);
            stream.WriteByte(5);
            stream.WriteSInt32(msobj.instanceId);

            stream.WriteByte(7);
            stream.WriteString16(msobj.gameObject.name);

            int sx = (int)(msobj.transform.localScale.x * 1000f);
            int sy = (int)(msobj.transform.localScale.y * 1000f);

            stream.WriteByte(8);
            stream.WriteSInt32(sx);
            stream.WriteByte(9);
            stream.WriteSInt32(sy);

            stream.WriteByte(6);
            msobj.Serialize(stream);
            stream.WriteByte(0);
		}

        WriteDataToFile(lo,stream);
	}

	static void ForeachObjAndSave(GameObject obj){
		foreach (Transform child in obj.transform)
		{
            InGameBaseObj gameobj = child.GetComponent<InGameBaseObj>();

            if(gameobj == null){
                continue;
			}
            gameobj.parentid = child.parent.GetInstanceID();
            gameobj.instanceId = child.GetInstanceID();

            msobjlist.Add(gameobj);

            ForeachObjAndSave(child.gameObject);
		}
	}

    static void WriteDataToFile(LevelOption od ,DataStream data){
        string filePath = GameConst.GetLevelDataFilePath(od.levelName + ".bytes");

		byte[] objData = data.ToByteArray();
		byte[] objGzipData = GameCommon.CompressGZip(objData);
		GameCommon.WriteByteToFile(objGzipData,filePath );

		Debug.Log("save path : " + filePath);
		EditorUtility.DisplayDialog("导出关卡" + od.levelName + "成功！", filePath,  "确定");
	}

}
