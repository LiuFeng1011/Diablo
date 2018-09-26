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
    public int isLevelup;
    public float levelval;

    public int formula;
}

public class PropertyConfManager
{
    public List<PropertyConf> datas { get; private set; }
    public Dictionary<int,PropertyConf> dataMap { get; private set; }
    public void Load()
    {
        if (datas != null) datas.Clear();
        if (dataMap != null) dataMap.Clear();
        else dataMap = new Dictionary<int, PropertyConf>();

        datas = ConfigManager.Load<PropertyConf>();
        for (int i = 0; i < datas.Count; i ++){
            dataMap.Add(datas[i].id,datas[i]);
        }
    }

}