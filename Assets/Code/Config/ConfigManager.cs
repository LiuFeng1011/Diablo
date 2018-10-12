using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 游戏配置表管理类
 * 
 * 添加配置表流程
 * 1.新建配置表管理脚本 
 * 2.创建数据类 xxxData ，属性字段与配置表一一对应
 * 3.创建数据管理类 xxxDataManager，
 * 4.实现管理类的load方法
 * 5.在ConfigManager中定义静态变量，在LoadData方法中调用管理类的Load方法
 */
public class ConfigManager  {
//
//	public static ConfigManager GetInstance(){
//		if(instance == null){
//			instance = new ConfigManager();
//		}
//		return instance;
//	}

	public static bool loadDown = false;

    public static CharacterConfManager characterConfManager = new CharacterConfManager();
    public static EquipConfManager equipConfManager = new EquipConfManager();
    public static PropertyConfManager propertyConfManager = new PropertyConfManager();
    public static SkillConfManager skillConfManager = new SkillConfManager();
    public static MapObjConfManager mapObjConfManager = new MapObjConfManager();
    public static MapGroupConfManager mapGroupConfManager = new MapGroupConfManager();
    public static EquipQualityManager equipQualityManager = new EquipQualityManager();
    public static LevelExpManager levelExpManager = new LevelExpManager();
    public static MapConfManager mapConfManager = new MapConfManager();

    static MapObjConfManager editorMapObjConfManager = null ;

    public static void LoadData(){
        Debuger.Log("===========启动配置表管理器===========");
        //confMapObjectManager.Load();
        //confEffectManager.Load();
        characterConfManager.Load();
        equipConfManager.Load();
        propertyConfManager.Load();
        skillConfManager.Load();
        mapObjConfManager.Load();
        mapGroupConfManager.Load();
        equipQualityManager.Load();
        levelExpManager.Load();
        mapConfManager.Load();
        Debuger.Log("----------配置表管理器启动成功-----------");

    }

	public static List<T> Load<T>() where T : new(){
		
		string[] names = (typeof(T)).ToString().Split('.');

        string filename = names[names.Length - 1];

        TextAsset text = Resources.Load(GameConst.GetConfigPath()+filename) as TextAsset;

        byte[] data = GameCommon.UnGZip(text.bytes);

        List<object> datalist = (List<object>)GameCommon.DeserializeObject(data);

        List<T> list = new List<T>();

        for (int i = 0; i < datalist.Count; i ++){
            list.Add((T)datalist[i]);
        }

        return list;
	}

    public static MapObjConfManager GetEditorMapObjConfManager(){
        if(editorMapObjConfManager == null){
            editorMapObjConfManager = new MapObjConfManager();
            editorMapObjConfManager.Load();
        }
        return editorMapObjConfManager;
    }

    public static void ReloadEditorMapObjConf(){
        GetEditorMapObjConfManager().Load();
    }

}
