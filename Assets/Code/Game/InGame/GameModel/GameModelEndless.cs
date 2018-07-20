using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModelEndless : BaseGameModelManager {

    List<InGameBaseCharacter> enemyList = new List<InGameBaseCharacter>();
    public override void Init()
    {

    }

    public override void Update()
    {

        for (int i = enemyList.Count - 1; i >= 0; i--){
            if(enemyList[i] == null){
                enemyList.RemoveAt(i);
            }
        }
        List<CharacterConf> roleList = ConfigManager.characterConfManager.dicByType[(int)CharacterConf.enCharacterType.enemy];

        while(enemyList.Count < 2){
            InGameBaseCharacter enemy = InGameManager.GetInstance().inGameObjManager.AddObj(roleList[Random.Range(0, roleList.Count)].id, enMSCamp.en_camp_enemy) as InGameBaseCharacter;
            enemy.AddAI();

            Vector3 pos = InGameManager.GetInstance().inGameLevelManager.gameMap.GetRandomWay();
            enemy.transform.position = GameCommon.GetWorldPos(pos);
            enemy.SetZPos();

            enemyList.Add(enemy);
        }
    }

    public override void Destory()
    {

    }
}
