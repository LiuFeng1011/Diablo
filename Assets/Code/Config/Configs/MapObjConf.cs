using System.Collections;
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
    public int size;
    public int depth;
    public int isstatic;
}
public class MapObjConfManager {

    public List<MapObjConf> datas { get; private set; }

    public Dictionary<int, Dictionary<int,List<MapObjConf>>> groupMap = 
        new Dictionary<int, Dictionary<int, List<MapObjConf>>>();

    public Dictionary<int, MapObjConf> map = new Dictionary<int, MapObjConf>();

    public void Load()
    {
        if (datas != null) datas.Clear();

        datas = ConfigManager.Load<MapObjConf>();

        groupMap.Clear();
        map.Clear();

        for (int i = 0; i < datas.Count; i++)
        {
            MapObjConf obj = datas[i];

            map.Add(obj.id,obj);

            if(!groupMap.ContainsKey(obj.group)){
                groupMap.Add(obj.group,new Dictionary<int, List<MapObjConf>>());
            }

            if(!groupMap[obj.group].ContainsKey(obj.size)){
                groupMap[obj.group].Add(obj.size, new List<MapObjConf>());
            }
            groupMap[obj.group][obj.size].Add(obj);
        }
    }

    public MapObjConf GetConfByPrefabName(string prefabName){
        for (int i = 0; i < datas.Count; i ++){
            string[] names = datas[i].path.Split('/');
            if (prefabName == names[names.Length-1]) {
                return datas[i];
            }
        }
        return null;
    }


}
