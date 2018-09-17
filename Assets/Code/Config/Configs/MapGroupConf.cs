using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MapGroupConf  {
    public int id;
    public int group;
    public int size;
    public string path;
    public string way;
	
}
public class MapGroupConfManager
{
    public List<MapGroupConf> datas = new List<MapGroupConf>();

    //id->data
    public Dictionary<int, MapGroupConf> dataMap = new Dictionary<int, MapGroupConf>();

    //group->data list
    public Dictionary<int, List<MapGroupConf>> dataGroupMap = new Dictionary<int, List<MapGroupConf>>();

    //group->way->data list
    public Dictionary<int, Dictionary<string,List<MapGroupConf>>> dataWayGroupMap = new Dictionary<int, Dictionary<string, List<MapGroupConf>>>();


    public void Load()
    {
        datas.Clear();
        dataMap.Clear();
        dataGroupMap.Clear();

        datas = ConfigManager.Load<MapGroupConf>();
        for (int i = 0; i < datas.Count; i++)
        {
            MapGroupConf conf = datas[i];
            dataMap.Add(conf.id, datas[i]);

            if(!dataGroupMap.ContainsKey(conf.group)){
                dataGroupMap.Add(conf.group,new List<MapGroupConf>());
            }
            dataGroupMap[conf.group].Add(conf);

            if(!dataWayGroupMap.ContainsKey(conf.group)){
                dataWayGroupMap.Add(conf.group, new Dictionary<string, List<MapGroupConf>>());
            }
            if(!dataWayGroupMap[conf.group].ContainsKey(conf.way)){
                dataWayGroupMap[conf.group].Add(conf.way, new List<MapGroupConf>());
            }
            dataWayGroupMap[conf.group][conf.way].Add(conf);
        }
    }

}