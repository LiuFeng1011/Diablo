using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MapConf
{
    public int id;
    public int type;
    public int group;
    public int row;
    public int col;

}
public class MapConfManager  {

    public List<MapConf> datas = new List<MapConf>();

    //id->data
    public Dictionary<int, MapConf> dataMap = new Dictionary<int, MapConf>();

    public void Load()
    {
        datas.Clear();
        dataMap.Clear();

        datas = ConfigManager.Load<MapConf>();
        for (int i = 0; i < datas.Count; i++)
        {
            MapConf conf = datas[i];
            dataMap.Add(conf.id, datas[i]);
        }
    }
	
}
