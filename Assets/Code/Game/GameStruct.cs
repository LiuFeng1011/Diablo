using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct EquipProperty
{
    public AHInt id;
    public AHFloat val;

    public void Deserialize(DataStream stream)
    {
        short type = stream.ReadSInt16();
        while(type != -1){
            switch(type){
                case 1:
                    id = stream.ReadSInt32();
                    break;
                case 2:
                    val = (float)stream.ReadSInt32() / 1000f;
                    break;
            }
            type = stream.ReadSInt16();
        }

        //Debug.Log("EquipProperty : " + id + " val1,val2,val3 : " + val1 + "/" + val2 + "/" + val3);
    }
    public void Serialize(DataStream stream)
    { 
        stream.WriteSInt16(1);
        stream.WriteSInt32(id);
        stream.WriteSInt16(2);
        stream.WriteSInt32((int)(val * 1000f));
        stream.WriteSInt16(-1);
    }
}
[Serializable]
public class GoodsData{
    public enum GoodsType{
        item,
        equip
    }
    public GoodsType type;

    public virtual void Deserialize(DataStream stream) { }
    public virtual void Serialize(DataStream stream) { }
}
[Serializable]
public class ItemData : GoodsData
{
    int confid;
    int count;
    public ItemData(){
        type = GoodsType.item;
    }

    public override void Deserialize(DataStream stream) {

        short _type = stream.ReadSInt16();
        while (_type != -1)
        {
            switch (_type)
            {
                case 1:
                    confid = stream.ReadSInt32(); break;
                case 2:
                    count = stream.ReadSInt32(); break;
            }
            _type = stream.ReadSInt16();
        }
    }
    public override void Serialize(DataStream stream) { 
        stream.WriteSInt16(0);
        stream.WriteSInt32(confid);
        stream.WriteSInt16(1);
        stream.WriteSInt32(count);
        stream.WriteSInt16(-1);
    }
}
[Serializable]
public class  EquipData : GoodsData
{
    public AHInt instanceid;
    public AHInt equipid;
    public AHInt qualityID = 1;
    public AHInt roleid;//<0 仓库 >0角色id
    public List<EquipProperty> propertyList ;

    public EquipData()
    {
        type = GoodsType.equip;
    }

    public EquipData(int instanceid,int qualityid,int equipid,int roleid,List<EquipProperty> propertyList){
        this.instanceid = instanceid;
        this.qualityID = qualityid;
        this.equipid = equipid;
        this.roleid = roleid;
        this.propertyList = propertyList;

        type = GoodsType.equip;
    }

    public override void Deserialize(DataStream stream)
    {
        //Debug.Log("================EquipData=================");
        short _type = stream.ReadSInt16();
        while (_type != -1)
        {
            switch (_type)
            {
                case 1:
                    instanceid = stream.ReadSInt32();break;
                case 2:
                    equipid = stream.ReadSInt32();break;
                case 3:
                    roleid = stream.ReadSInt32();break;
                case 4:
                    int listcount = stream.ReadSInt32();
                    propertyList = new List<EquipProperty>();
                    for (int i = 0; i < listcount; i++)
                    {
                        EquipProperty p = (new EquipProperty());
                        p.Deserialize(stream);
                        propertyList.Add(p);
                    }
                    break;
                case 5:
                    qualityID = stream.ReadSInt32(); break;
            }
            _type = stream.ReadSInt16();
        }
        //Debug.Log("instanceid : " + instanceid + 
                  //" equipid : " + equipid +
                  //" roleid : " + roleid +
                  //" baseval : " + baseval);
    }
    public override void Serialize(DataStream stream){
        stream.WriteSInt16(1);
        stream.WriteSInt32(instanceid);
        stream.WriteSInt16(2);
        stream.WriteSInt32(equipid);
        stream.WriteSInt16(3);
        stream.WriteSInt32(roleid);
        stream.WriteSInt16(4);
        stream.WriteSInt32(propertyList.Count);
        for (int i = 0; i < propertyList.Count;  i++ ){
            propertyList[i].Serialize(stream);
        }
        stream.WriteSInt16(5);
        stream.WriteSInt32(qualityID);
        stream.WriteSInt16(-1);
    }
}

[Serializable]
public class RoleData
{
    public AHInt instanceid;
    public AHInt roldid;

    public string name;

    public AHInt equipCount = 9;

    public List<AHInt> choseEquipList = new List<AHInt>();

    //public AHInt strength = 1;//力量 1攻击力，1生命，3护甲 ，0.1生命回复
    //public AHInt agility = 1;//敏捷 1攻击速度，0.3闪避 0.01移动速度
    //public AHInt brains = 1;//智力 0.01释放技能概率 

    public Dictionary<int, float> additionPropertyList = new Dictionary<int, float>();

    public AHInt level = 1;
    public AHInt exp = 0;

    public AHInt levelupPoint = 0;//升级点数
    private int strength;

    public static RoleData Create(int roleid,string name){
        RoleData data = new RoleData();
        data.instanceid = UserDataManager.instance.GetInstanceID();
        data.roldid = roleid;
        data.name = name;
        data.equipCount = 9;
        return data;
    }

    public bool AddEquip(int id){
        if (choseEquipList.Count >= equipCount) return false;
        choseEquipList.Add(id);
        return true;
    }
    public bool CancelEquip(int id)
    {
        for (int i = 0; i < choseEquipList.Count; i ++ ){
            if(choseEquipList[i] == id){
                choseEquipList.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
    //TYPE 位置
    public int GetChooseEquip(int type){

        if (choseEquipList.Count <= type) return -1;
        return choseEquipList[type];
    } 

    public void Deserialize(DataStream stream)
    {
        short type = stream.ReadSInt16();
        while (type != -1)
        {
            switch (type)
            {
                case 1:
                    instanceid = stream.ReadSInt32();
                    break;
                case 2:
                    roldid = stream.ReadSInt32();
                    break;
                case 3:
                    name = stream.ReadString16();
                    break;
                case 4:
                    int count = stream.ReadSInt32();
                    for (int i = 0; i < count; i ++){
                        int id = stream.ReadSInt32();
                        choseEquipList.Add(id);
                    }
                    break;
                case 5: equipCount = stream.ReadSInt32(); break;
                case 6: 
                    int pcount = stream.ReadSInt32();
                    for (int i = 0; i < pcount; i++)
                    {
                        int id = stream.ReadSInt32();
                        additionPropertyList.Add(id, (float)stream.ReadSInt32() / 1000);
                    }
                    break;
                case 7: level = stream.ReadSInt32(); break;
                case 8: exp = stream.ReadSInt32(); break;
                case 9: levelupPoint = stream.ReadSInt32(); break;

            }
            type = stream.ReadSInt16();
        }

    }
    public void Serialize(DataStream stream)
    {
        stream.WriteSInt16(1);
        stream.WriteSInt32(instanceid);
        stream.WriteSInt16(2);
        stream.WriteSInt32(roldid);
        stream.WriteSInt16(3);
        stream.WriteString16(name);

        stream.WriteSInt16(4);
        stream.WriteSInt32(choseEquipList.Count);
        for (int i = 0; i < choseEquipList.Count; i++)
        {
            stream.WriteSInt32(choseEquipList[i]);
        }
        stream.WriteSInt16(5);
        stream.WriteSInt32(equipCount);

        stream.WriteSInt16(6);
        stream.WriteSInt32(additionPropertyList.Count);

        foreach (KeyValuePair<int, float> kv in additionPropertyList)
        {
            stream.WriteSInt32(kv.Key);
            stream.WriteSInt32((int)(kv.Value * 1000));
        }

        stream.WriteSInt16(7);
        stream.WriteSInt32(level);
        stream.WriteSInt16(8);
        stream.WriteSInt32(exp);

        stream.WriteSInt16(9);
        stream.WriteSInt32(levelupPoint);

        stream.WriteSInt16(-1);
    }
}