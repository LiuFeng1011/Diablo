using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EquipConf  {
    public int id;  /*  道具id    */
    public string name; /*  道具名 */
    public string prefabpath;   /*  预制体 */
    public string prefabname;   /*  文件名 */
}

public class EquipConfManager
{
    public List<EquipConf> datas { get; private set; }
    public Dictionary<int, EquipConf> dic = new Dictionary<int, EquipConf>();

    public void Load()
    {

        if (datas != null) datas.Clear();

        datas = ConfigManager.Load<EquipConf>();
        dic.Clear();

        for (int i = 0; i < datas.Count; i++)
        {
            EquipConf obj = datas[i];
            dic.Add(obj.id, obj);
        }
    }

}