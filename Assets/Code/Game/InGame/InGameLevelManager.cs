﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameLevelManager : InGameBaseManager {

    public BaseGameModelManager gameModel;
    public MazeMapManager gameMap;

    public override void Init()
    {
        gameModel = BaseGameModelManager.GetGameMode(BaseGameModelManager.GameLevelModel.level);
        gameModel.Init();

        if(GameGlobal.selLevelConfId == -1){
            gameMap = MazeMapManager.CreateMapManager(ConfigManager.mapConfManager.dataMap[1]);
        }else{
            gameMap = MazeMapManager.CreateMapManager(ConfigManager.mapConfManager.dataMap[GameGlobal.selLevelConfId]);
        }
        //gameMap = MazeMapManager.CreateMapManager(MazeMapManager.MapType.en_type_randomgroup,4,2,2);
        //gameMap = MazeMapManager.CreateMapManager(MazeMapManager.MapType.en_type_fixeditormap, 5004010, 100, 100);

    }

    public override void Update()
    {
        gameModel.Update();
        gameMap.Update();
    }

    public override void Destory()
    {
        gameModel.Destory();
        gameMap.Destory();
    }
}
