using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTypeRandomGroup : MazeMapManager {
    struct MapVector2{
        public int x, y;
        public MapVector2(int x,int y){
            this.x = x;
            this.y = y;
        }
    }
    class MapTree
    {
        public enum MapTreeType{
            start,
            end,
            normal
        }
        public MapTree parent = null;
        public List<MapTree> children = new List<MapTree>();
        public MapVector2 pos ;

        public MapTreeType treetype = MapTreeType.normal;

        public MapTree(MapVector2 pos){
            this.pos = pos;
        }

        public void AddChild(MapTree child) {
            this.children.Add(child);
            child.parent = this;
        }
    }

    int[,] maparr;

    MapTree mapTree;

    int i_mapBreathCount=0,i_mapmaindepth = 3;
    bool b_ismainbreath = true;
    /// <summary>
    /// Inits the map.
    /// </summary>
    /// <param name="group">Group type id.</param>
    /// <param name="row">Row 每行的group数量.</param>
    /// <param name="col">Col 每列的group数量.</param>
    public override void InitMap(int group, int row, int col)
    {
        base.InitMap(group, row*GameConst.RANDOM_GROUP_SIZE, col*GameConst.RANDOM_GROUP_SIZE);

        i_mapmaindepth = row * col / 2;
        maparr = new int[row, col];

        Dictionary<string,List<MapGroupConf>> conflist = ConfigManager.mapGroupConfManager.dataWayGroupMap[group];
        if (conflist == null && conflist.Count <= 0)
        {
            Debug.LogError("no map conf!! group : " + group);
            return;
        }
        //随机地图
        MapVector2 startPos = new MapVector2(Random.Range(0,row),Random.Range(0, col));

        mapTree = new MapTree(startPos);
        mapTree.treetype = MapTree.MapTreeType.start;
        maparr[startPos.x,startPos.y] = 1;
        this.i_mapBreathCount++;
        RandomMap( this.mapTree);

        Debug.Log(LitJson.JsonMapper.ToJson(maparr));

        //for (int i = 0; i < maparr.GetLength(0); i++)
        //{
        //    for (int j = 0; j < maparr.GetLength(1); j++)
        //    {
        //        if (maparr[i, j] > 0)
        //        {
        //            string waystring = GetWayString(i,j);

                    //if(!conflist.ContainsKey(waystring)){

                    //    Debug.Log(" no group : " + waystring);
                    //    continue;
                    //}
                    //List<MapGroupConf> mapList = conflist[waystring];
                    //if (mapList.Count <= 0) {
                    //    Debug.Log(" no group : " + waystring);
                    //    continue;
                    //}
        //InsertGroup(new Vector2(i*GameConst.RANDOM_GROUP_SIZE, j*GameConst.RANDOM_GROUP_SIZE), mapList[Random.Range(0,mapList.Count)].id);
        //        }
        //    }
        //}

        GenerateGroup(mapTree);

        if (startPointList.Count > 0)
        {
            startPoint = startPointList[Random.Range(0, startPointList.Count)];
        }
        else
        {
            startPoint = GetRandomWay();
        }
    }

    void GenerateGroup(MapTree tree){
        string waystring = "";

        if(tree.treetype == MapTree.MapTreeType.start){
            waystring = "-1";
        }else {
            waystring = GetTreeWayString(tree);
        }

        Dictionary<string, List<MapGroupConf>> conflist = ConfigManager.mapGroupConfManager.dataWayGroupMap[this.mapGroup];

        if (!conflist.ContainsKey(waystring))
        {

            Debug.Log(" no group : " + waystring);
            return;
        }
        List<MapGroupConf> mapList = conflist[waystring];
        if (mapList.Count <= 0)
        {
            Debug.Log(" no group : " + waystring);
            return;
        }

        MapGroupConf conf = mapList[Random.Range(0, mapList.Count)];
        Debug.Log(conf.id + " : " + tree.pos.x + " / " + tree.pos.y);
        InsertGroup(new Vector2(tree.pos.x * GameConst.RANDOM_GROUP_SIZE, tree.pos.y * GameConst.RANDOM_GROUP_SIZE),
                    conf.id);

        for (int i = 0; i < tree.children.Count; i++)
        {
            this.GenerateGroup(tree.children[i]);
        }
    }

    void RandomMap(MapTree tree){
        List<MapVector2> nearlist = new List<MapVector2>();

        if (tree.pos.x - 1 >= 0 && maparr[tree.pos.x - 1,tree.pos.y] == 0)
        {
            nearlist.Add(new MapVector2(tree.pos.x - 1, tree.pos.y));
        }
        if (tree.pos.x + 1 < maparr.GetLength(0) && maparr[tree.pos.x + 1,tree.pos.y] == 0)
        {
            nearlist.Add(new MapVector2(tree.pos.x + 1, tree.pos.y));
        }
        if (tree.pos.y - 1 >= 0 && maparr[tree.pos.x,tree.pos.y - 1] == 0)
        {
            nearlist.Add(new MapVector2(tree.pos.x, tree.pos.y - 1));
        }
        if (tree.pos.y + 1 < maparr.GetLength(1) && maparr[tree.pos.x,tree.pos.y + 1] == 0)
        {
            nearlist.Add(new MapVector2(tree.pos.x, tree.pos.y + 1));
        }


        while (nearlist.Count > 0)
        {
            if (this.b_ismainbreath && this.i_mapBreathCount > this.i_mapmaindepth)
            {
                this.b_ismainbreath = false;
                return;
            }

            var random = Random.Range(0,nearlist.Count);
            if (!this.b_ismainbreath && Random.Range(0f,1f) < 0.8f)
            {
                nearlist.RemoveAt(random);
                continue;
            }
            this.i_mapBreathCount++;

            var pos = nearlist[random];
            var branch = new MapTree(pos);
            tree.AddChild(branch);

            maparr[pos.x,pos.y] = this.i_mapBreathCount;
            nearlist.RemoveAt(random);

            if (this.b_ismainbreath && this.i_mapBreathCount > this.i_mapmaindepth)
            {
                branch.treetype = MapTree.MapTreeType.end;
            }

            this.RandomMap( branch);
        }
    }

    string GetWayString(int x,int y){
        string ret = "";
        //东
        if (x + 1 < maparr.GetLength(0) && maparr[x + 1, y] != 0)
        {
            ret += "1";
        }else{
            ret += "0";
        }
        //南
        if (y - 1 >= 0 && maparr[x, y - 1] != 0)
        {
            ret += "1";
        }
        else
        {
            ret += "0";
        }
        //西
        if (x - 1 >= 0 && maparr[x - 1, y] != 0)
        {
            ret += "1";
        }
        else
        {
            ret += "0";
        }
        //北
        if (y + 1 < maparr.GetLength(1) && maparr[x, y + 1] != 0)
        {
            ret += "1";
        }
        else
        {
            ret += "0";
        }
        return ret;
    }

    string GetTreeWayString(MapTree tree){
        string ret = "";
        int[] wayarr = new int[4];
        if(tree.parent != null){
            wayarr[GetWayIndex(tree.parent.pos, tree.pos)] = 1;
        }

        for (int i = 0; i < tree.children.Count; i ++){
            wayarr[GetWayIndex(tree.children[i].pos,tree.pos)] = 1;
        }
        for (int i = 0; i < wayarr.Length; i ++){
            ret += wayarr[i];
        }
        return ret;
    }

    int GetWayIndex(MapVector2 start,MapVector2 end){
        if (start.x != end.x)
        {
            if (start.x > end.x)
            {
                return 0;
            }
            else
            {
                return 2;
            }
        }
        else
        {
            if (start.y < end.y)
            {
                return 1;
            }
            else
            {
                return 3;
            }
        }
    }
}
