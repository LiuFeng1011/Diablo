using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SmallMapConf {

    public int id;
    public string name;
    public string des;
    public string filepath;
    public int sizex;
    public int sizey;
}

public class SmallMapConfManager
{
    public List<SmallMapConf> datas { get; private set; }
    public Dictionary<int, SmallMapConf> dataMap { get; private set; }
    public void Load()
    {
        if (datas != null) datas.Clear();
        if (dataMap != null) dataMap.Clear();
        else dataMap = new Dictionary<int, SmallMapConf>();

        datas = ConfigManager.Load<SmallMapConf>();
        for (int i = 0; i < datas.Count; i++)
        {
            dataMap.Add(datas[i].id, datas[i]);
        }
    }

}