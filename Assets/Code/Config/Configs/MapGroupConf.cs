using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MapGroupConf  {
    public int id;
    public int group;
    public int size;
    public string path;
	
}
public class MapGroupConfManager
{
    public List<MapGroupConf> datas = new List<MapGroupConf>();
    public Dictionary<int, MapGroupConf> dataMap = new Dictionary<int, MapGroupConf>();
    public Dictionary<int, List<MapGroupConf>> dataGroupMap = new Dictionary<int, List<MapGroupConf>>();

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
        }
    }

}