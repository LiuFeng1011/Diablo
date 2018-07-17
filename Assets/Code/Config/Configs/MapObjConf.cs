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
}
public class MapObjConfManager {

    public List<MapObjConf> datas { get; private set; }

    public Dictionary<int, MapObjConf> groupMap = new Dictionary<int, MapObjConf>();

    public void Load()
    {
        if (datas != null) datas.Clear();

        datas = ConfigManager.Load<MapObjConf>();
        groupMap.Clear();

        for (int i = 0; i < datas.Count; i++)
        {
            MapObjConf obj = datas[i];
            groupMap.Add(obj.group, obj);
        }
    }


}
