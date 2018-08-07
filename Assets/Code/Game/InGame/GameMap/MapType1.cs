using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapType1 : MazeMapManager {


    public override void InitMap(int group, int row, int col)
    {
        base.InitMap(group, row, col);

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


        AccumulationMap(mazeCreate.tree, Accumulation);
        ErosionMap(mazeCreate.tree);

        startPoint = mazeCreate.tree.position;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (IsPointType(i, j, MazeCreate.PointType.way))
                {
                    astarArray[i, j] = 1;
                    //map[i, j] = new InGameMapPointData(MazeCreate.PointType.way,new Vector2(i,j));

                    if (mazeCreate.mapList[i][j] == (int)MazeCreate.PointType.fullway) continue;
                    int scale = GetMaxFullSpace(i, j, MazeCreate.PointType.way);

                    while (scale > 1 && !ConfigManager.mapObjConfManager.groupMap[mapGroup].ContainsKey(scale))
                    {
                        scale--;
                    }

                    for (int x = 0; x < scale; x++)
                    {
                        for (int y = 0; y < scale; y++)
                        {
                            mazeCreate.mapList[i + x][j + y] = (int)MazeCreate.PointType.fullway;
                            map[i + x, j + y] = new InGameMapPointData(MazeCreate.PointType.way, new Vector2(i + x, j + y));
                        }
                    }

                    //Vector3 objpos = new Vector3(i + (float)(scale - 1f) / 2f, j + (float)(scale - 1f) / 2f, 0);
                    Vector3 objpos = new Vector3(i, j, 0);
                    CreateGround(objpos, mapGroup, scale);

                }
                else if (IsNearFullGround(i, j))
                {
                    map[i, j] = new InGameMapPointData(MazeCreate.PointType.wall, new Vector2(i, j));

                    CreateGround(new Vector3(i, j), mapGroup, 1);
                    CreateGround(new Vector3(i, j), mapGroup + 100000, 1);

                    mazeCreate.mapList[i][j] = (int)MazeCreate.PointType.wallfull;
                }
                else
                {
                    if (mazeCreate.mapList[i][j] == (int)MazeCreate.PointType.wallfull) continue;
                    int scale = GetMaxFullSpace(i, j, MazeCreate.PointType.wall);

                    while (scale > 1 && !ConfigManager.mapObjConfManager.groupMap[mapGroup + 100000].ContainsKey(scale))
                    {
                        scale--;
                    }
                    if (scale < 2)
                    {
                        continue;
                    }

                    for (int x = 0; x < scale; x++)
                    {
                        for (int y = 0; y < scale; y++)
                        {
                            map[i + x, j + y] = new InGameMapPointData(MazeCreate.PointType.wall, new Vector2(i, j));
                            CreateGround(new Vector3(i + x, j + y), mapGroup, 1);
                            mazeCreate.mapList[i + x][j + y] = (int)MazeCreate.PointType.wallfull;
                        }
                    }

                    //CreateGround(new Vector3(i + (float)(scale - 1f) / 2f, j + (float)(scale - 1f) / 2f, 0), mapGroup + 100000, scale);
                    CreateGround(new Vector3(i, j, 0), mapGroup + 100000, scale);

                }
            }
        }

        StaticBatchingUtility.Combine(mapObj);

    }
}
