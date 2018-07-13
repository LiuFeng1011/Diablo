using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MapObjConf
{
    public enum enMapObjType
    {
        character = 1,
        enemy = 2,
    }

    public int id;    /*  道具id    */
    public string name; /*  道具名 */
    public string prefabName;   /*  文件名 */
    public int type;/*   "种类
    1角色" 
    */
    public int level;
    public int unlocktype;//解锁方式

    public int price;//价格

    public float life;    /*  生命  */
    public float liferevive;  /*  生命回复    */
    public float atkForce;    /*  攻击力 */
    public float moveSpeed;   /*  移动速度加成  */
    public float atkSpeed;    /*  攻击速度加成  */
    public float comborate;   /*  暴击几率    */
    public float comboval;    /*  暴击伤害    */
    public float armor;   /*  护甲  */
    public float avoid;   /*  闪避  */
    public float returnhurt;  /*  反弹伤害    */
    public float steallife;   /*  偷取生命    */
    public float golddrop;    /*  金币掉率加成  */
    public float equipdrop;   /*  魔法物品掉率  */


    public float[] propertyValues ;

    public void SetPropertyvaluse(){
        propertyValues = new float[(int)enCharacterProperty.maxval];

        propertyValues[(int)enCharacterProperty.life] = life;
        propertyValues[(int)enCharacterProperty.liferevive] = liferevive;
        propertyValues[(int)enCharacterProperty.atkForce] = atkForce;
        propertyValues[(int)enCharacterProperty.moveSpeed] = moveSpeed;
        propertyValues[(int)enCharacterProperty.atkSpeed] = atkSpeed;
        propertyValues[(int)enCharacterProperty.comborate] = comborate;
        propertyValues[(int)enCharacterProperty.comboval] = comboval;
        propertyValues[(int)enCharacterProperty.armor] = armor;
        propertyValues[(int)enCharacterProperty.avoid] = avoid;
        propertyValues[(int)enCharacterProperty.returnhurt] = returnhurt;
    }
}


public class ConfMapObjectManager{
    public List<MapObjConf> datas {get;private set;}
    public Dictionary<int, MapObjConf> dic = new Dictionary<int, MapObjConf>();

    //key : obj type 
    //val : obj
    public Dictionary<int, List<MapObjConf>> dicByType = new Dictionary<int, List<MapObjConf>>();

	public void Load(){

		if(datas != null) datas.Clear();

        datas = ConfigManager.Load<MapObjConf>();
        dic.Clear();

        for (int i = 0; i < datas.Count; i++)
        {
            MapObjConf obj = datas[i];
            dic.Add(obj.id, obj);
            if(!dicByType.ContainsKey(obj.type)){
                List<MapObjConf> typelist = new List<MapObjConf>();
                typelist.Add(obj);
                dicByType.Add(obj.type,typelist);
            }else{
                dicByType[obj.type].Add(obj);
            }

            obj.SetPropertyvaluse();
        }
	}

    public MapObjConf GetRandomObjByType(int type){
        List<MapObjConf> list ;
        if(!dicByType.TryGetValue(type,out list)){
            return null;
        } 
        return list[(int)UnityEngine.Random.Range(0, list.Count)];
    }

}
