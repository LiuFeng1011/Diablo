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

    public string atlasName;
    public string skill;
    public string property;
    public int minlv;
    public int rate;//爆率
}

public class EquipConfPropertyData{
    public int id;
    public int rate;//属性加成率 1+1*rate
}
public class EquipConfSkillData
{
    public int id;
    public int val;//技能值
}
public class EquipConfData{
    public List<EquipConfSkillData> skillList = new List<EquipConfSkillData>();
    public List<EquipConfPropertyData> propertyList = new List<EquipConfPropertyData>();

    public void Init(string skill,string property){
        string[] skills = skill.Split(';');

        for (int i = 0; i < skills.Length;  i++){
            if (skills[i].Length <= 0) continue;
            string[] skilldatas = skills[i].Split(',');
            if (skilldatas.Length < 2) continue;

            EquipConfSkillData data = new EquipConfSkillData();
            data.id = int.Parse(skilldatas[0]);
            data.val = int.Parse(skilldatas[1]);
            skillList.Add(data);
        }

        string[] propertys = property.Split(';');

        for (int i = 0; i < propertys.Length; i++)
        {
            if (propertys[i].Length <= 0) continue;
            string[] propertydatas = propertys[i].Split(',');
            if (propertydatas.Length < 2) continue;

            EquipConfPropertyData data = new EquipConfPropertyData();
            data.id = int.Parse(propertydatas[0]);
            data.rate = int.Parse(propertydatas[1]);
            propertyList.Add(data);
        }
    }
}

public class EquipConfManager
{
    public List<EquipConf> datas { get; private set; }
    public Dictionary<int, EquipConf> dic = new Dictionary<int, EquipConf>();
    public Dictionary<int, EquipConfData> datadic = new Dictionary<int, EquipConfData>();
    public int allOdds = 0;
    public void Load()
    {

        if (datas != null) datas.Clear();

        datas = ConfigManager.Load<EquipConf>();
        dic.Clear();

        for (int i = 0; i < datas.Count; i++)
        {
            EquipConf obj = datas[i];
            dic.Add(obj.id, obj);

            EquipConfData equipConfData = new EquipConfData();
            equipConfData.Init(obj.skill,obj.property);
            datadic.Add(obj.id,equipConfData);

            allOdds += obj.rate;
        }
    }

    public EquipConf GetRandomEquip(){
        int rand = UnityEngine.Random.Range(0, allOdds);
        int nowOdds = 0;
        for (int i = 0; i < datas.Count; i ++){
            nowOdds += datas[i].rate;
            if(nowOdds > rand){
                return datas[i];
            }
        }
        return datas[0];
    }

}