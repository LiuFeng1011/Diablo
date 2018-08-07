using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixEditorMap : MazeMapManager {

    public override void InitMap(int group, int row, int col)
    {
        base.InitMap(group, row, col);

        InsertGroup(new Vector2(0, 0), group);

        if (startPointList.Count > 0)
        {
            startPoint = startPointList[Random.Range(0, startPointList.Count)];
        }
        else
        {
            startPoint = GetRandomWay();
        }
    }
}
