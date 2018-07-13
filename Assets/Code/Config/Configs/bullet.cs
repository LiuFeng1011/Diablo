using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class bullet
{
    public int objid;   
    public string name;    
    public string prefabName; 
    public int type;  
    public int speed;   
    public int life;    
    public string dieeffect;   
    public string tail;
    public int unlocktype;
    public int price;
}

public class BulletManager
{
    public List<bullet> datas { get; private set; }
    //key id
    public Dictionary<int, bullet> dic = new Dictionary<int, bullet>();

    public void Load()
    {

        if (datas != null) datas.Clear();

        datas = ConfigManager.Load<bullet>();
        dic.Clear();

        for (int i = 0; i < datas.Count; i++)
        {
            bullet obj = datas[i];
            dic.Add(obj.objid, obj);
        }
    }

}