using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGameModelManager : BaseGameObject {

    public enum GameLevelModel
    {
        nothing,
        endless,
        level,
    }

    protected List<MapEnemyPoint> enemyPoints = new List<MapEnemyPoint>();

    public static BaseGameModelManager GetGameMode(GameLevelModel model){
        BaseGameModelManager ret = null;
        switch(model){
            case GameLevelModel.endless:
                ret = new GameModelEndless();
                break;
            case GameLevelModel.level:
                ret = new GameModelLevel();
                break;
            default:
                ret = new BaseGameModelManager();
                break;
        }

        ret.Init();
        return ret;
    }

    public virtual void Init(){
        EventManager.Register(this,EventID.EVENT_ADD_ENEMYPOINT);
    }

    public virtual void Update()
    {

    }

    public virtual void Destory()
    {
        EventManager.Remove(this);
    }

    public override void HandleEvent(EventData resp)
    {
        switch (resp.eid)
        {
            case EventID.EVENT_ADD_ENEMYPOINT:
                MapEnemyPoint enemyPoint = (MapEnemyPoint)resp.sUserData[0];
                enemyPoints.Add(enemyPoint);
                break;
        }

    }

}
