using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class MapTypeEditorMap : MazeMapManager {

    int[,] groundArr ;


    public override void InitMap(int group, int row, int col)
    {
        base.InitMap(group, row, col);

        map = new InGameMapPointData[row, col];
        astarArray = new int[row, col];

        groundArr = new int[row, col];


        List<MapGroupConf> conflist = ConfigManager.mapGroupConfManager.dataGroupMap[group];

        if (conflist == null && conflist.Count <= 0) {
            Debug.LogError("no map conf!! group : " + group);
            return;
        }

        //放置group
        int count = row * col / 100;
        for (int i = 0; i < count ; i++)
        {
            MapGroupConf conf = conflist[Random.Range(0, conflist.Count)];
            SetObjPos(conf);
        }

        List<Vector2> poslist;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if(groundArr[i,j] > 0){
                    InsertGroup(new Vector2(i, j),groundArr[i, j]);
                }
            }
        }


        //for (int x = 0; x < row; x++)
        //{
        //    for (int y = 0; y < col; y++)
        //    {
        //        int sx = x;
        //        int sy = y;
        //        InGameMapPointData data = map[sx, sy];
        //        if (data == null)
        //        {
        //            continue;
        //        }
        //        for (int i = 0; i < data.objList.Count; i++)
        //        {
        //            if (data.objList[i].obj == null)
        //            {
        //                GameObject obj = GetPoolObj(data.objList[i].conf);
        //                obj.transform.position = GameCommon.GetWorldPos(data.objList[i].pos);
        //                GameCommon.SetObjZIndex(obj, data.objList[i].conf.depth);
        //                data.objList[i].obj = obj;
        //            }
        //        }
        //        lastScreenObj.Add(data);
        //    }
        //}
        //string s = "";
        //for (int x = 0; x < row; x++)
        //{
        //    for (int y = 0; y < col; y++)
        //    {
        //        s += " " + astarArray[x, y];
        //    }
        //    s += "\n";
        //}
        //Debug.Log(s);

        startPoint = GetRandomWay();
    }

    public override void Update(){
        base.Update();
    }


    void SetObjPos(MapGroupConf conf)
    {
        int size = conf.size;

        int count = 0;

        while(count < row || count < col){
            int x = count , y = count;
            if (count < row) x = row ;
            if (count >= col) y = col ;

            bool isfind = false;
            for (int i = 0; i < x; i ++){
                Vector2 v = new Vector2(i, y);
                if (haveSpace(v, size))
                {
                    SetSpace(v, size, conf.id);
                    isfind = true;
                    break;
                }
            }
            if (isfind) break;
            for (int i = 0; i < y; i++)
            {
                Vector2 v = new Vector2(x, i);
                if (haveSpace(v, size))
                {
                    SetSpace(v, size, conf.id);
                    isfind = true;
                    break;
                }
            }
            if (isfind) break;
            count++;
        }

        //for (int i = 0; i < row; i++)
        //{
        //    for (int j = 0; j < col; j++)
        //    {
        //        Vector2 v = new Vector2(i, j);
        //        if (haveSpace(v, size))
        //        {
        //            poslist.Add(v);
        //        }
        //    }
        //}
    }

    bool haveSpace(Vector2 pos, int size)
    {
        int x = (int)pos.x, y = (int)pos.y;

        for (int i = x; i < x + size; i++)
        {
            if (i >= row) return false;
            for (int j = y; j < y + size; j++)
            {
                if (j >= col) return false;
                if (groundArr[i, j] != 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    void SetSpace(Vector2 pos, int size,int confid)
    {
        int x = (int)pos.x, y = (int)pos.y;
        for (int i = x; i < x + size; i++)
        {
            if (i >= row) return;
            for (int j = y; j < y + size; j++)
            {
                if (j >= col) return;
                groundArr[i, j] = -1;
            }
        }
        groundArr[x, y] = confid;
    }
}
