using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGameModelManager : BaseGameObject {

    public enum GameLevelModel
    {
        nothing,
        endless,
    }

    public static BaseGameModelManager GetGameMode(GameLevelModel model){
        BaseGameModelManager ret = null;
        switch(model){
            case GameLevelModel.endless:
                ret = new GameModelEndless();
                break;
            default:
                ret = new BaseGameModelManager();
                break;
        }

        ret.Init();
        return ret;
    }

    public virtual void Init(){
        
    }

    public virtual void Update()
    {

    }

    public virtual void Destory()
    {

    }


}
