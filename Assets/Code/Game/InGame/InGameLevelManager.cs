using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameLevelManager : InGameBaseManager {

    public BaseGameModelManager gameModel;
    public MazeMapManager gameMap;

    public override void Init()
    {
        gameModel = BaseGameModelManager.GetGameMode(BaseGameModelManager.GameLevelModel.endless);

        gameMap = new MazeMapManager();
        gameMap.Init();
    }

    public override void Update()
    {
        gameModel.Update();
    }

    public override void Destory()
    {
        gameModel.Destory();
    }
}
