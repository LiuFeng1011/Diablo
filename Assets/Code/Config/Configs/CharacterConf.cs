﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CharacterConf
{
    public enum enCharacterType
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
    public float atkdis;
    public int atkskill;

    public int unlocktype;//解锁方式

    public int price;//价格

    public int strength;//力量
    public int agility;//敏捷
    public int brains;//智利
    public float life;    /*  生命  */
    public float liferevive;  /*  生命回复    */
    public float mana;//魔法 
    public float manarevive;// 魔法回复
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
    public float outodds;   /*  爆率  */


    public float[] propertyValues ;

    public void SetPropertyvaluse(){
        propertyValues = new float[(int)enCharacterProperty.maxval];

        propertyValues[(int)enCharacterProperty.life] = life;
        propertyValues[(int)enCharacterProperty.liferevive] = liferevive;
        propertyValues[(int)enCharacterProperty.mana] = mana;
        propertyValues[(int)enCharacterProperty.manarevive] = manarevive;
        propertyValues[(int)enCharacterProperty.atkForce] = atkForce;
        propertyValues[(int)enCharacterProperty.moveSpeed] = moveSpeed;
        propertyValues[(int)enCharacterProperty.atkSpeed] = atkSpeed;
        propertyValues[(int)enCharacterProperty.comborate] = comborate;
        propertyValues[(int)enCharacterProperty.comboval] = comboval;
        propertyValues[(int)enCharacterProperty.armor] = armor;
        propertyValues[(int)enCharacterProperty.avoid] = avoid;
        propertyValues[(int)enCharacterProperty.returnhurt] = returnhurt;

        propertyValues[(int)enCharacterProperty.strength] = strength;
        propertyValues[(int)enCharacterProperty.agility] = agility;
        propertyValues[(int)enCharacterProperty.brains] = brains;
    }
}


public class CharacterConfManager{
    public List<CharacterConf> datas {get;private set;}
    public Dictionary<int, CharacterConf> dic = new Dictionary<int, CharacterConf>();

    //key : obj type 
    //val : obj
    public Dictionary<int, List<CharacterConf>> dicByType = new Dictionary<int, List<CharacterConf>>();

	public void Load(){

		if(datas != null) datas.Clear();

        datas = ConfigManager.Load<CharacterConf>();
        dic.Clear();

        for (int i = 0; i < datas.Count; i++)
        {
            CharacterConf obj = datas[i];
            dic.Add(obj.id, obj);
            if(!dicByType.ContainsKey(obj.type)){
                List<CharacterConf> typelist = new List<CharacterConf>();
                typelist.Add(obj);
                dicByType.Add(obj.type,typelist);
            }else{
                dicByType[obj.type].Add(obj);
            }

            obj.SetPropertyvaluse();
        }
	}

    public CharacterConf GetRandomObjByType(int type){
        List<CharacterConf> list ;
        if(!dicByType.TryGetValue(type,out list)){
            return null;
        } 
        return list[(int)UnityEngine.Random.Range(0, list.Count)];
    }

}
