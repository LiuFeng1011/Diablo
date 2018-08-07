using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEnemyPoint : InGameBaseMapObj {
    
    float intervalTime = 0f;
    public float flushTime = 10f;

    public int rangeMinCount = 0;
    public int rangeMaxCount = 0;
    public List<int> rangeObjIdList = new List<int>();

    List<InGameBaseCharacter> enemyList = new List<InGameBaseCharacter>();

	// Use this for initialization
	void Start () {
        intervalTime = flushTime;

        EventData.CreateEvent(EventID.EVENT_ADD_ENEMYPOINT).AddData(this).Send();
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
        if (rangeObjIdList.Count <= 0) return;
        int count = Random.Range(rangeMinCount, rangeMaxCount);
        int size = 1;
        Vector2 basepos = GameCommon.GetMapPos(transform.position);
        while(count > 0){
            for (int i = 0; i < size; i++){
                count--;
                Vector2 _pos = basepos + new Vector2(i, size - 1);
                AddObj(rangeObjIdList[Random.Range(0, rangeObjIdList.Count)],GameCommon.GetWorldPos(_pos));
                if (count <= 0) break;

                count--;
                _pos = basepos + new Vector2(size - 1, i);
                AddObj(rangeObjIdList[Random.Range(0, rangeObjIdList.Count)], GameCommon.GetWorldPos(_pos));
                if (count <= 0) break;
            }
            size++;
        }

    }


    InGameBaseCharacter AddObj(int id,Vector2 pos){
        InGameBaseCharacter enemy = InGameManager.GetInstance().inGameObjManager.AddObj(id,enMSCamp.en_camp_enemy) as InGameBaseCharacter;
        enemyList.Add(enemy);
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
        writer.WriteSInt32(rangeMinCount);
        writer.WriteSInt32(3);
        writer.WriteSInt32(rangeMaxCount);

        writer.WriteSInt32(4);
        writer.WriteSInt32(rangeObjIdList.Count);
        for (int i = 0; i < rangeObjIdList.Count; i++){
            writer.WriteSInt32(rangeObjIdList[i]);
        }
        writer.WriteSInt32(-1);
    }

    public override void Deserialize(DataStream reader)
    {

        int dataid = reader.ReadSInt32();

        while(dataid != -1){
            switch(dataid){
                case 1: flushTime = (float)reader.ReadSInt32() / 1000f; break;
                case 2: rangeMinCount = reader.ReadSInt32(); break;
                case 3: rangeMaxCount = reader.ReadSInt32();break;
                case 4:
                    int idcount = reader.ReadSInt32();
                    for (int i = 0; i < idcount; i ++){
                        rangeObjIdList.Add(reader.ReadSInt32());
                    }
                    break;
            }
            dataid = reader.ReadSInt32();
        }
    }
}
