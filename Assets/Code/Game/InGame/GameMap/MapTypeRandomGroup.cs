using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTypeRandomGroup : MazeMapManager {

    int[,] groundArr;

    /// <summary>
    /// Inits the map.
    /// </summary>
    /// <param name="group">Group type id.</param>
    /// <param name="row">Row 每行的group数量.</param>
    /// <param name="col">Col 每列的group数量.</param>
    public override void InitMap(int group, int row, int col)
    {
        base.InitMap(group, row, col);

        groundArr = new int[row, col];


        Dictionary<string,List<MapGroupConf>> conflist = ConfigManager.mapGroupConfManager.dataWayGroupMap[group];
        if (conflist == null && conflist.Count <= 0)
        {
            Debug.LogError("no map conf!! group : " + group);
            return;
        }



    }
}
