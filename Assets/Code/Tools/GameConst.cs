using UnityEngine;
using System.IO;

public enum enMSObjID{
	en_obj_id_null		=	0,
	en_obj_id_obj_1		= 	1001,
	en_obj_id_obj_2,
}

//阵营
public enum enMSCamp{
	en_camp_player,//玩家
	en_camp_enemy,//敌人
	en_camp_neutral,//中立
	en_camp_item,//其他
}

public enum enDataType{
	en_datatype_int,
	en_datatype_float,
	en_datatype_string,
	en_datatype_end = 1000,
}

//角色动作
public enum enObjectAnimationType{
	en_animationtype_wait,//待机
	en_animationtype_run,//跑
	en_animationtype_jump,//跳
	en_animationtype_atk1,//攻击
	en_animationtype_atk2,
	en_animationtype_atk3,
	en_animationtype_atk4,
	en_animationtype_atk5,
}

//刷怪点类型
public enum enEnemyPointType{
	en_enemypoint_type_null,
	en_enemypoint_type_dailytiem,//时间间隔
	en_enemypoint_type_event,//

}

//AI Type
public enum enAIType{
	en_aitype_wait,//木桩
	en_aitype_counterattack,//反击
	en_aitype_active,//主动
	en_aitype_patrol,//巡逻
}

public enum enGameState{
    ready,
    playing,
    pause,
    over
}

public struct GameModel{
    public int modelid;
    public string name;
    public string lbname;

    public GameModel(int modelid,string name, string lbname){
        this.modelid = modelid;
        this.name = name;
        this.lbname = lbname;
    }
}

public static class GameConst  {
	
	public const string userDataFileName = "userdata";
    public const string CONF_FILE_NAME = ".bytes";

    public const string USERDATANAME_MODEL = "model";
    public const string USERDATANAME_MODEL_MAXSCORES = "model_maxscores_";
    public const string USERDATANAME_MODEL_LASTSCORES = "model_lastscores_";
    public const string USERDATANAME_SEL_ROLE = "selrole";
    public const string USERDATANAME_GOLD = "gold";
    public const string USERDATANAME_UNLOCK_BULLET = "bullet_";


    public static GameModel[] gameModels = {
        new GameModel(0,"Normal","Normal"),
        new GameModel(1,"OneLife","OneLife"),
        new GameModel(2,"NoFault","NoFault"),
        new GameModel(3,"Time","wipecube_time"),
    };

    public const int MAP_HEIGHT = 16;
    public const int MAP_WIDTH = 6;
    public const float MAP_OBJ_MAX_POSX = 3;

    public const float comboDis = 0.3f;
    public const int timeModelTime = 60;

    public const float STEP_MIN_SIZE = 0.5f;
    public const float STEP_MAX_SIZE = 1.5f;

    //观看视频奖励
    public const int WATCH_AD_GOLD = 50;

    public static int[,] CAMP_ATK = {
         //玩家, //敌人, //中立, //其他
        {   0,     1,     1,      0   },//玩家
        {   1,     0,     1,      0   },//敌人
        {   1,     1,     0,      0   },//中立
        {   0,     0,     0,      0   },//其他
    };

	
	public static string GetLevelDataFilePath(string filename){
        return "Assets/Resources/LevelData/" + filename;
	}

    public static string GetLevelDataFilePath()
    {
        return Application.dataPath + "/Resources/LevelData/";
    }
	public static string GetExcelFilePath(string filename){
		return Application.dataPath+"/ExcelTools/xlsx/"+filename;
	}

	public static string GetConfigFilePath(string tablename){
		string src = "";

		if (Application.platform == RuntimePlatform.Android)
		{
			src = "jar:file://" + Application.dataPath + "!/assets/Config/" + tablename;
		}else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			src = "file://" + Application.dataPath + "/Raw/Config/" + tablename;
		}else{
			src = "file://" + Application.streamingAssetsPath + "/Config/" + tablename;
		}
		return src;
	}
	public static string GetConfigPath(){
        return "Config/";
	}

	public static string SaveConfigFilePath(string tablename)
    {
        return "Assets/Resources/Config/" + tablename;
	}

	public static string GetPersistentDataPath(string filename){
		return Application.persistentDataPath + "/" + filename;  
	}

	public static string[] objectAnimationName = {"wait","run","jump","atk1","atk2","atk3","atk4","atk5"};
}
