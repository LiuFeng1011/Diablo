﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameLevelManager : InGameBaseManager {

    public BaseGameModelManager gameModel;
    public MazeMapManager gameMap;

    public override void Init()
    {
        gameModel = BaseGameModelManager.GetGameMode(BaseGameModelManager.GameLevelModel.endless);

        gameMap = MazeMapManager.CreateMapManager(MazeMapManager.MapType.en_type_editormap,1,50,50);
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
