﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MapObjConf
{
    public int id;
    public string name;
    public int group;
    public string path;
    public int sizeX;
    public int sizeY;
    public int depth;
    public int isstatic;
}
public class MapObjConfManager {

    public List<MapObjConf> datas { get; private set; }

    //group>size>obj
    public Dictionary<int, Dictionary<int,List<MapObjConf>>> groupMap = 
        new Dictionary<int, Dictionary<int, List<MapObjConf>>>();

    public Dictionary<int, MapObjConf> map = new Dictionary<int, MapObjConf>();

    public Dictionary<string, MapObjConf> prefabNameMap = new Dictionary<string, MapObjConf>();

    public void Load()
    {
        if (datas != null) datas.Clear();

        datas = ConfigManager.Load<MapObjConf>();

        groupMap.Clear();
        map.Clear();
        prefabNameMap.Clear();

        for (int i = 0; i < datas.Count; i++)
        {
            MapObjConf obj = datas[i];

            map.Add(obj.id,obj);

            if(!groupMap.ContainsKey(obj.group)){
                groupMap.Add(obj.group,new Dictionary<int, List<MapObjConf>>());
            }

            if(!groupMap[obj.group].ContainsKey(obj.sizeX)){
                groupMap[obj.group].Add(obj.sizeX, new List<MapObjConf>());
            }
            groupMap[obj.group][obj.sizeX].Add(obj);

            string[] names = obj.path.Split('/');
            string _n = names[names.Length - 1];
            if(!prefabNameMap.ContainsKey(_n)){
                prefabNameMap.Add(_n, obj);
            }else{
                Debug.Log(_n);
            }


        }
    }

    public MapObjConf GetConfByPrefabName(string prefabName){
        return prefabNameMap[prefabName];
    }


}
