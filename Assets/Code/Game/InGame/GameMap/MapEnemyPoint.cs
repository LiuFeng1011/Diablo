using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapEnemyPointEnemy{
    public int rangeMinCount = 0;
    public int rangeMaxCount = 0;
    public List<int> rangeObjIdList = new List<int>();
    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(1);
        writer.WriteSInt32(rangeMinCount);
        writer.WriteSInt32(2);
        writer.WriteSInt32(rangeMaxCount);

        writer.WriteSInt32(3);
        int count = rangeObjIdList.Count;
        Debug.Log(count);
        writer.WriteSInt32(count);

        for (int i = 0; i < count; i++)
        {
            writer.WriteSInt32(rangeObjIdList[i]);
        }

        writer.WriteSInt32(-1);
    }

    public void Deserialize(DataStream reader)
    {
        rangeObjIdList.Clear();
        int dataid = reader.ReadSInt32();

        while (dataid != -1)
        {
            switch (dataid)
            {
                case 1: rangeMinCount = reader.ReadSInt32(); break;
                case 2: rangeMaxCount = reader.ReadSInt32(); break;
                case 3:
                    int idcount = reader.ReadSInt32();

                    for (int i = 0; i < idcount; i++)
                    {
                        rangeObjIdList.Add(reader.ReadSInt32());
                    }
                    break;
            }
            dataid = reader.ReadSInt32();
        }
    }
}

public class MapEnemyPoint : InGameBaseMapObj {
    
    float intervalTime = 0f;
    public float flushTime = 10f;
    public bool isElite = false;

    public List<MapEnemyPointEnemy> enemyPointList = new List<MapEnemyPointEnemy>();

    List<InGameBaseCharacter> enemyList = new List<InGameBaseCharacter>();

	// Use this for initialization
	void Start () {
        intervalTime = flushTime;

        EventData.CreateEvent(EventID.EVENT_ADD_ENEMYPOINT).AddData(this).Send();

        this.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
    public override bool ObjUpdate () {
        if (InGameManager.GetInstance().GetGameState() != enGameState.playing) return false;
        int length = enemyList.Count-1;

        for (int i = length; i >= 0; i --){
            if (enemyList[i] == null) enemyList.RemoveAt(i);
        }

        if (enemyList.Count > 0) return false;

        intervalTime += Time.deltaTime;

        if (intervalTime < flushTime) return false;

        intervalTime = 0f;

        AddRangeEnemy();
        return true;
	}

    void AddRangeEnemy(){
        if (enemyPointList.Count <= 0) return;

        List<int> objidList = new List<int>();
        //将所有敌人随机插入到队列中
        for (int i = 0; i < enemyPointList.Count; i++){
            MapEnemyPointEnemy point = enemyPointList[i];
            int enemycount = Random.Range(point.rangeMinCount, point.rangeMaxCount);
            for (int j = 0; j < enemycount; j ++){
                if(objidList.Count <= 0){
                    objidList.Add(point.rangeObjIdList[Random.Range(0, point.rangeObjIdList.Count)]);
                }else{
                    objidList.Insert(Random.Range(0, objidList.Count),
                                 point.rangeObjIdList[Random.Range(0, point.rangeObjIdList.Count)]);
                }

            }
        }

        //向周围扩散加入敌人
        int count = objidList.Count;
        int size = 1;
        Vector2 basepos = GameCommon.GetMapPos(transform.position);
        while(count >= 0){
            for (int i = 0; i < size; i++){
                count--;
                if (count < 0) break;
                Vector2 _pos = basepos + new Vector2(i, size - 1);
                AddObj(objidList[count],GameCommon.GetWorldPos(_pos));

                count--;
                if (count < 0) break;
                _pos = basepos + new Vector2(size - 1, i);
                AddObj(objidList[count], GameCommon.GetWorldPos(_pos));
            }
            size++;
        }

    }


    InGameBaseCharacter AddObj(int id,Vector2 pos){
        InGameBaseCharacter enemy = InGameManager.GetInstance().inGameObjManager.AddObj(id,enMSCamp.en_camp_enemy) as InGameBaseCharacter;
        enemyList.Add(enemy);
        enemy.SetIsElite(isElite) ;
        enemy.AddAI();

        enemy.transform.position = pos;
        enemy.SetZPos();
        return enemy;
    }

    public override void Serialize(DataStream writer)
    {
        writer.WriteSInt32(1);
        writer.WriteSInt32((int)(flushTime * 1000f));

        writer.WriteSInt32(2);
        int count = enemyPointList.Count;
        writer.WriteSInt32(enemyPointList.Count);

        for (int i = 0; i < count; i ++){
            MapEnemyPointEnemy point = enemyPointList[i];
            point.Serialize(writer);
        }

        writer.WriteSInt32(3);
        writer.WriteBoolean(isElite);

        writer.WriteSInt32(-1);
    }

    public override void Deserialize(DataStream reader)
    {
        enemyPointList.Clear();
        int dataid = reader.ReadSInt32();

        while(dataid != -1){
            switch(dataid){
                case 1: flushTime = (float)reader.ReadSInt32() / 1000f; break;
                case 2: 
                    int count = reader.ReadSInt32();
                    for (int i = 0; i < count; i++)
                    {
                        MapEnemyPointEnemy point = new MapEnemyPointEnemy();
                        point.Deserialize(reader);
                        enemyPointList.Add(point);
                    }
                    break;
                case 3: isElite = reader.ReadBoolean(); break;
                    
            }
            dataid = reader.ReadSInt32();
        }
    }
}
