using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SkillConf {

    public int id;
    public string name;
    public string des;
    public string filepath;
}

public class SkillConfManager
{
    public List<SkillConf> datas { get; private set; }
    public Dictionary<int, SkillConf> dataMap { get; private set; }
    public void Load()
    {
        if (datas != null) datas.Clear();
        if (dataMap != null) dataMap.Clear();
        else dataMap = new Dictionary<int, SkillConf>();

        datas = ConfigManager.Load<SkillConf>();
        for (int i = 0; i < datas.Count; i++)
        {
            dataMap.Add(datas[i].id, datas[i]);
        }
    }

}