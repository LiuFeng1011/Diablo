using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class LevelExp
{
    public int id;
    public int level;
    public int exp;
}
public class LevelExpManager {

    public List<LevelExp> datas = new List<LevelExp>();

    //level->data
    public Dictionary<int, LevelExp> dataMap = new Dictionary<int, LevelExp>();

    public void Load()
    {
        datas.Clear();
        dataMap.Clear();

        datas = ConfigManager.Load<LevelExp>();
        for (int i = 0; i < datas.Count; i++)
        {
            LevelExp conf = datas[i];
            dataMap.Add(conf.level, datas[i]);
        }
    }
}
