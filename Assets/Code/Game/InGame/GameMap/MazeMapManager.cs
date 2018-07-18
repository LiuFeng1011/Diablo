using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsertMapData{
    public Vector2 pos;
    public SmallMapConf conf;
}

public class MazeMapManager : BaseGameMapManager {

    const float mapscale = 1;
    int row = 30, col = 30;
    MazeCreate mazeCreate;
    int Accumulation = 95;//障碍堆积系数
    int Erosion = 50;//障碍侵蚀系数

    int mapGroup = 1;

    public int[,] map;

    GameObject mapObj;

    public override void Init()
    {
        mapObj = new GameObject("map");

        List<InsertMapData> insertMapList = new List<InsertMapData>();
        InsertMapData insertMapData = new InsertMapData();
        //insertMapData.pos = 
        //mazeCreate = MazeCreate.GetMaze(row, col);
        List<List<int>> mapList = new List<List<int>>();

        for (int i = 0; i < row; i++)
        {
            mapList.Add(new List<int>());
            for (int j = 0; j < col; j++)
            {
                //if ((i >= 10 && i < 20) &&
                //    (j >= 10 && j < 20))
                //{
                //    mapList[i].Add((int)MazeCreate.PointType.nullpoint);
                //}
                //else
                {
                    mapList[i].Add((int)MazeCreate.PointType.wall);
                }
            }
        }

        mazeCreate = MazeCreate.GetMaze(mapList);


        AccumulationMap(mazeCreate.tree,Accumulation);
        ErosionMap(mazeCreate.tree);

        map = new int[row, col];

        startPoint = mazeCreate.tree.position;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (IsPointType(i, j, MazeCreate.PointType.way) )
                {
                    map[i, j] = 1;
                    if (mazeCreate.mapList[i][j] == (int)MazeCreate.PointType.fullway) continue;
                    int scale = GetMaxFullSpace(i, j,MazeCreate.PointType.way);

                    GameObject ground = CreateGround(new Vector3(i + (float)(scale - 1f) / 2f, j + (float)(scale - 1f) / 2f, 0),mapGroup, scale);

                    while(ground == null){
                        scale--;
                        ground = CreateGround(new Vector3(i + (float)(scale - 1f) / 2f, j + (float)(scale - 1f) / 2f, 0),mapGroup, scale);
                    }

                    for (int x = 0; x < scale; x++)
                    {
                        for (int y = 0; y < scale; y++)
                        {
                            mazeCreate.mapList[i + x][j + y] = (int)MazeCreate.PointType.fullway;
                        }
                    }
                }
                else if(IsNearFullGround(i,j))
                {
                    GameObject ground   = CreateGround(new Vector3(i, j), mapGroup , 1);
                    GameObject obs      = CreateGround(new Vector3(i, j), mapGroup + 100000, 1);
                    if (Random.Range(0, 100) < 50) obs.transform.localScale = new Vector3(
                        obs.transform.localScale.x * -1,
                        obs.transform.localScale.y,
                        obs.transform.localScale.z
                    );
                    mazeCreate.mapList[i][j] = (int)MazeCreate.PointType.wallfull;

                }else{
                    if (mazeCreate.mapList[i][j] == (int)MazeCreate.PointType.wallfull) continue;
                    int scale = GetMaxFullSpace(i, j, MazeCreate.PointType.wall);
                    if (scale < 2 /*&& Random.Range(0,100) < 50*/) {
                        continue;
                    }
                    GameObject ground = CreateGround(new Vector3(i + (float)(scale - 1f) / 2f, j + (float)(scale - 1f) / 2f, 0), mapGroup + 100000, scale);

                    while (ground == null)
                    {
                        scale--;
                        ground = CreateGround(new Vector3(i + (float)(scale - 1f) / 2f, j + (float)(scale - 1f) / 2f, 0), mapGroup + 100000, scale);
                    }
                    for (int x = 0; x < scale; x++)
                    {
                        for (int y = 0; y < scale; y++)
                        {
                            ground = CreateGround(new Vector3(i+x, j+y), mapGroup, 1);
                            mazeCreate.mapList[i + x][j + y] = (int)MazeCreate.PointType.wallfull;
                        }
                    }
                }

            }
        }

        StaticBatchingUtility.Combine(mapObj);
    }

    void InsertMap(){
        
    }
    int GetMaxFullSpace(int i, int j,MazeCreate.PointType type)
    {
        int z = 0;

        while (true)
        {
            z++;
            if (i + z >= row || j + z >= col) break;
            bool isfull = true;
            for (int x = 0; x < z; x++)
            {
                int val = mazeCreate.mapList[i + x][j + z];
                if (val == (int)type)
                {

                }
                else
                {
                    isfull = false;
                    break;
                }
            }
            if (!isfull) break;

            for (int y = 0; y < z; y++)
            {
                int val = mazeCreate.mapList[i + z][j + y];
                if (val == (int)type)
                {

                }
                else
                {
                    isfull = false;
                    break;
                }
            }
            if (!isfull) break;
        }
        return z;
    }

    bool IsNearFullGround(int i,int j){
        if(IsPointType(i,j+1,MazeCreate.PointType.way) &&
           IsPointType(i,j-1,MazeCreate.PointType.way) &&
           IsPointType(i+1,j,MazeCreate.PointType.way) &&
           IsPointType(i-1,j,MazeCreate.PointType.way) 
           ){
            return true;
        }
        return false;
    }

    bool IsPointType(int i ,int j,MazeCreate.PointType type){

        if (i < 0 || i >= mazeCreate.mapList.Count) return false;
        if (j < 0 || j >= mazeCreate.mapList[i].Count) return false;

        return (mazeCreate.mapList[i][j] & (int)type) == (int)type;
    }

    void AccumulationMap(MapTree tree,float accumulation)
    {
        for (int i = tree.children.Count - 1; i >= 0; i--)
        {
            AccumulationMap(tree.children[i], accumulation * 0.9f);
        }

        if (tree.children.Count < 1)
        {
            int rate = Random.Range(0, 100);
            if (rate < Accumulation/*-accumulation*/)
            {

                mazeCreate.mapList[(int)tree.position.x][(int)tree.position.y] = (int)MazeCreate.PointType.wall;

                tree.parent.children.Remove(tree);
                return;
            }
        }
        //CreateGround(tree.position + new Vector3(0,33));

    }

    void ErosionMap(MapTree tree){
        for (int i = 0; i < tree.children.Count; i++)
        {
            ErosionMap(tree.children[i]);
        }

        int x = (int)tree.position.x;
        int y = (int)tree.position.y;

        if (x > 0 && mazeCreate.mapList[x - 1][y] == (int)MazeCreate.PointType.wall)
        {
            SetErosionMapWay(tree,x - 1, y);
        }
        if (x < mazeCreate.mapList.Count - 1 && mazeCreate.mapList[x + 1][y] == (int)MazeCreate.PointType.wall)
        {
            SetErosionMapWay(tree,x + 1, y);
        }
        if (y > 0 && mazeCreate.mapList[x][y - 1] == (int)MazeCreate.PointType.wall)
        {
            SetErosionMapWay(tree,x, y - 1);
        }
        if (y < mazeCreate.mapList[x].Count - 1 && mazeCreate.mapList[x][y + 1] == (int)MazeCreate.PointType.wall)
        {
            SetErosionMapWay(tree,x, y + 1);
        }

    }

    void SetErosionMapWay(MapTree tree,int x, int y)
    {
        int rate = Random.Range(0, 100);
        if (rate < Erosion)
        {
            mazeCreate.mapList[x][y] = (int)MazeCreate.PointType.way;
            tree.AddChile(new MapTree(new Vector3(x, y)));
            //CreateGround(new Vector3(x, y + 33));
        }
    }

    GameObject CreateGround(Vector3 v,int group, int scale)
    {
        if(!ConfigManager.mapObjConfManager.groupMap[group].ContainsKey(scale)){
            return null;
        }
        List<MapObjConf> list = ConfigManager.mapObjConfManager.groupMap[group][scale];
        MapObjConf conf = list[Random.Range(0, list.Count)];
        GameObject column = (GameObject)Resources.Load(conf.path);
        column = MonoBehaviour.Instantiate(column);

        Vector3 worldpos = GameCommon.GetWorldPos(v);
        column.transform.position = worldpos ;

        GameCommon.SetObjZIndex(column, 0);
        //column.transform.localScale = new Vector3(1, 1, 1);

        column.transform.parent = mapObj.transform;

        return column;
    }

    public void LogMap()
    {
        string str = "";
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                //if(map[i,j] == 1) CreatePath(new Vector3(i * unitPathSize  - offset,0, j* unitPathSize - offset));
                str += map[i, j];
            }
            str += "\n";
        }
        Debug.Log(str);
    }

    public Vector3 GetRandomWay(){
        int x = Random.Range(0, map.GetLength(0));
        int y = Random.Range(0, map.GetLength(1));

        if(map[x, y] != 1){
            int count = 0;

            bool isfind = false;

            while (!isfind){
                count++;

                for (int i = -count; i <= count; i ++){
                    if(IsPointWay(x + i,y - count)){
                        isfind = true;
                        x = x + i;
                        y = y - count;
                        break;
                    }
                    if (IsPointWay(x + i, y + count))
                    {
                        isfind = true;
                        x = x + i;
                        y = y + count;
                        break;
                    }

                    if (IsPointWay(x - count, y + i))
                    {
                        isfind = true;
                        x = x - count;
                        y = y + i;
                        break;
                    }
                    if (IsPointWay(x + count, y + i))
                    {
                        isfind = true;
                        x = x + count;
                        y = y + i;
                        break;
                    }
                }
                if(count >= map.GetLength(0)){
                    break;
                }
            }

        }

        return new Vector3(x, y, 0);
    }

    bool IsPointWay(int x,int y){
        if (x < 0 || x >= map.GetLength(0)) return false;
        if (y < 0 || y >= map.GetLength(1)) return false;
        return map[x, y] == 1;
    }
}
