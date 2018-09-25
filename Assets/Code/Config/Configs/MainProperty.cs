using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MainProperty {

    public int propertyid;
    public int additionid;
}
public class MainPropertyManager
{

    public List<MainProperty> datas = new List<MainProperty>();

    //mainid->list propertyid
    public Dictionary<int, List<int>> dataMap = new Dictionary<int, List<int>>();

    public void Load()
    {
        datas.Clear();
        dataMap.Clear();

        datas = ConfigManager.Load<MainProperty>();
        for (int i = 0; i < datas.Count; i++)
        {
            MainProperty conf = datas[i];
            if(!dataMap.ContainsKey(conf.propertyid)){
                dataMap.Add(conf.propertyid,new List<int>());
            }
            dataMap[conf.propertyid].Add(conf.additionid);
        }
    }
}