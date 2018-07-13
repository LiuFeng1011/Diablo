using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModelEndless : BaseGameModelManager {

    InGameBaseCharacter enemy = null;

    public override void Init()
    {

    }

    public override void Update()
    {
        if(enemy == null){

            List<MapObjConf> roleList = ConfigManager.mapObjectManager.dicByType[(int)MapObjConf.enMapObjType.enemy];

            enemy = InGameManager.GetInstance().inGameObjManager.AddObj(roleList[Random.Range(0,roleList.Count)].id,enMSCamp.en_camp_enemy) as InGameBaseCharacter;
            enemy.AddAI();

            Vector3 pos = InGameManager.GetInstance().inGameLevelManager.gameMap.GetRandomWay();
            enemy.transform.position = GameCommon.GetWorldPos(pos);
        }
    }

    public override void Destory()
    {

    }
}
