using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PropertyConf  {
    public int id;  /*  属性id    */
    public string name; /*  属性名 */
    public string des;  /*  属性描述    */
    public string boardDes;
    public float baseval;  /* 基础值 */
    public float randomrange;  /*  随机范围  */
    public int mainProerty;
    public int isLevelup;
    public float levelval;

    public int formula;

    public float GetLevelupVal(int level){
        if(isLevelup == 0){
            return levelval * level;
        }

        return level * level * levelval / isLevelup + levelval;
    }
}

public class PropertyConfManager
{
    public List<PropertyConf> datas { get; private set; }
    //id -> data
    public Dictionary<int,PropertyConf> dataMap { get; private set; }
    //mainproperty -> data
    public Dictionary<int, List<PropertyConf>> mainDataMap { get; private set; }

    public void Load()
    {
        if (datas != null) datas.Clear();

        if (dataMap != null) dataMap.Clear();
        else dataMap = new Dictionary<int, PropertyConf>();

        if (mainDataMap != null) mainDataMap.Clear();
        else mainDataMap = new Dictionary<int, List<PropertyConf>>();

        datas = ConfigManager.Load<PropertyConf>();
        for (int i = 0; i < datas.Count; i ++){
            PropertyConf data = datas[i];
            dataMap.Add(data.id,data);

            if(!mainDataMap.ContainsKey(data.mainProerty)){
                mainDataMap.Add(data.mainProerty,new List<PropertyConf>());
            }
            mainDataMap[data.mainProerty].Add(data);
        }
    }

}