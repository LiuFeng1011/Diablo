using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public struct STMapEnemyPoint{
    public int count;
    public int rangeObjId;
    public float rangeSize;
    public GameObject positionObj;
}

public class MapEnemyPoint : BaseUnityObject {
    public int enemyLevel = 1;

    float intervalTime = 0f;
    public float flushTime = 1f;

    public int rangeMinCount = 0;
    public int rangeMaxCount = 0;
    public int rangeObjId = -1;
    public float rangeSize = 0;

    public List<STMapEnemyPoint> pointList = new List<STMapEnemyPoint>();
    List<InGameBaseEnemy> enemyList = new List<InGameBaseEnemy>();

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (InGameManager.GetInstance().GetGameState() != enGameState.playing) return;
        int length = enemyList.Count-1;

        for (int i = length; i >= 0; i --){
            if (enemyList[i] == null) enemyList.RemoveAt(i);
        }

        if (enemyList.Count > 0) return;

        intervalTime += Time.deltaTime;

        if (intervalTime < flushTime) return;

        intervalTime = 0f;

        AddRangeEnemy();
        AddPointEnemy();
	}

    void AddRangeEnemy(){
        if (rangeObjId == -1) return;

        int count = Random.Range(rangeMinCount, rangeMaxCount);

        for (int i = 0; i < count;  i++){
            InGameBaseEnemy enemy = AddObj(rangeObjId);

            float vecx = Random.Range(0f, rangeSize);
            float vecz = Random.Range(0f, rangeSize);

            enemy.transform.position = transform.position + new Vector3(vecx, 0, vecz);
        }


    }

    void AddPointEnemy(){
        for (int i = 0; i < pointList.Count; i ++){
            STMapEnemyPoint point = pointList[i];

            for (int j = 0; j < point.count; j ++){

                InGameBaseEnemy enemy = AddObj(point.rangeObjId);

                float vecx = Random.Range(0f, point.rangeSize);
                float vecz = Random.Range(0f, point.rangeSize);

                enemy.transform.position = point.positionObj.transform.position + 
                    new Vector3(vecx, 0, vecz);
                
            }
        }
    }

    InGameBaseEnemy AddObj(int id){
        
        InGameBaseEnemy enemy = InGameManager.GetInstance().inGameObjManager.AddObj(id,enMSCamp.en_camp_enemy) as InGameBaseEnemy;
        enemyList.Add(enemy);

        return enemy;
    }
}
