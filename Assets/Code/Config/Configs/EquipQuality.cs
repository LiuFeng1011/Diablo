using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EquipQuality {

    public int id;
    public float odds;
    public String color;
    public String name;
    public int propertycount;
}

public class EquipQualityManager
{
    public List<EquipQuality> datas { get; private set; }
    public Dictionary<int, EquipQuality> dic = new Dictionary<int, EquipQuality>();
    public Dictionary<int, Color> colordic = new Dictionary<int, Color>();

    public float allOdds = 0;

    public void Load()
    {

        if (datas != null) datas.Clear();

        datas = ConfigManager.Load<EquipQuality>();
        dic.Clear();

        for (int i = 0; i < datas.Count; i++)
        {
            EquipQuality obj = datas[i];

            colordic.Add(obj.id,GameCommon.HexToColor(obj.color));
            dic.Add(obj.id, obj);

            allOdds += obj.odds;
        }
    }

    public EquipQuality GetEquipQuality(float fm){
        float _allodds = allOdds + fm * datas.Count;
        float randodds = UnityEngine.Random.Range(0, _allodds);

        float nowodds = 0;
        for (int i = 0; i < datas.Count;  i++){
            nowodds += datas[i].odds + fm;
            if(randodds < nowodds){
                return datas[i];
            }
        }
        return datas[0];
    }

}