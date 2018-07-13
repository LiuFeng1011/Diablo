using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsertMapData{
    public Vector2 pos;
    public string mappath;
}

public class MazeMapManager : BaseGameMapManager {

    const float mapscale = 1;
    const int row = 50, col = 50;
    MazeCreate mazeCreate;
    int Accumulation = 95;//障碍堆积系数
    int Erosion = 30;//障碍侵蚀系数

    public int[,] map;

    GameObject mapObj;

    public override void Init()
    {
        mapObj = new GameObject("map");

        //mazeCreate = MazeCreate.GetMaze(row, col);
        List<List<int>> mapList = new List<List<int>>();

        for (int i = 0; i < row; i++)
        {
            mapList.Add(new List<int>());
            for (int j = 0; j < col; j++)
            {
                if ((i >= 10 && i < 20) &&
                    (j >= 10 && j < 20))
                {
                    mapList[i].Add((int)MazeCreate.PointType.nullpoint);
                }
                else
                {
                    mapList[i].Add((int)MazeCreate.PointType.wall);
                }
            }
        }

        mazeCreate = MazeCreate.GetMaze(mapList);


        CreateMap(mazeCreate.tree);

        map = new int[row, col];


        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if(mazeCreate.mapList[i][j] != (int)MazeCreate.PointType.wall && 
                   mazeCreate.mapList[i][j] != (int)MazeCreate.PointType.nullpoint){
                    map[i,j] = 1;
                }
                if (mazeCreate.mapList[i][j] == (int)MazeCreate.PointType.startpoint)
                {
                    startPoint = new Vector3(i, j, 0) ;
                    Debug.Log("startPosition1 : " + startPoint);
                }

                if (mazeCreate.mapList[i][j] == (int)MazeCreate.PointType.startpoint ||
                    mazeCreate.mapList[i][j] == (int)MazeCreate.PointType.way)
                {
                    int scale = GetMaxFullSpace(i, j);

                    GameObject ground = CreateGround(new Vector3(i + (float)(scale - 1f) / 2f, j + (float)(scale - 1f) / 2f, 0), scale);

                    for (int x = 0; x < scale; x++)
                    {
                        for (int y = 0; y < scale; y++)
                        {
                            mazeCreate.mapList[i + x][j + y] = (int)MazeCreate.PointType.fullway;
                        }
                    }
                }
                else
                {
                    
                }

            }
        }

        StaticBatchingUtility.Combine(mapObj);
    }

    void InsertMap(){
        
    }
    int GetMaxFullSpace(int i, int j)
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
                if (val == (int)MazeCreate.PointType.way)
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
                if (val == (int)MazeCreate.PointType.way)
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

    void CreateMap(MapTree tree)
    {
        for (int i = tree.children.Count - 1; i >= 0; i--)
        {
            CreateMap(tree.children[i]);
        }

        if (tree.children.Count < 1)
        {
            int rate = Random.Range(0, 100);
            if (rate < Accumulation)
            {

                mazeCreate.mapList[(int)tree.position.x][(int)tree.position.y] = (int)MazeCreate.PointType.nullpoint;

                tree.parent.children.Remove(tree);
                return;
            }
        }
        //CreateGround(tree.position + new Vector3(0,33));

        int x = (int)tree.position.x;
        int y = (int)tree.position.y;
        if (x > 0 && mazeCreate.mapList[x - 1][y] == (int)MazeCreate.PointType.wall)
        {
            SetMapWay(x - 1, y);
        }
        if (x < mazeCreate.mapList.Count - 1 && mazeCreate.mapList[x + 1][y] == (int)MazeCreate.PointType.wall)
        {
            SetMapWay(x + 1, y);
        }
        if (y > 0 && mazeCreate.mapList[x][y - 1] == (int)MazeCreate.PointType.wall)
        {
            SetMapWay(x, y - 1);
        }
        if (y < mazeCreate.mapList[x].Count - 1 && mazeCreate.mapList[x][y + 1] == (int)MazeCreate.PointType.wall)
        {
            SetMapWay(x, y + 1);
        }
    }

    void SetMapWay(int x, int y)
    {
        if (Random.Range(0, 100) < Erosion)
        {
            mazeCreate.mapList[x][y] = (int)MazeCreate.PointType.way;
            //CreateGround(new Vector3(x, y + 33));
        }
    }

    GameObject CreateGround(Vector3 v, int scale)
    {
        GameObject column = (GameObject)Resources.Load("Prefabs/MapObj/MapGround_1_" + scale + "_" + 0);
        column = MonoBehaviour.Instantiate(column);

        column.transform.position = GameCommon.GetWorldPos(v) ;
        column.transform.position += new Vector3(0, 0, v.y - v.x + 100);
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
