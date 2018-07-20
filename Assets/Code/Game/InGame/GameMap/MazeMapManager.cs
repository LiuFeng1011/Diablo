using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPointObj{
    public Vector2 pos;
    public MapObjConf conf;
    public GameObject obj;
    public MapPointObj(MapObjConf conf,GameObject obj, Vector2 pos){
        this.conf = conf;
        this.obj = obj;
        this.pos = pos;
    }

}
public class InGameMapPointData{
    public Vector2 pos;
    public MazeCreate.PointType type;
    public List<MapPointObj> objList = new List<MapPointObj>();

    public InGameMapPointData(MazeCreate.PointType type,Vector2 pos){
        this.type = type;
        this.pos = pos;
    }

    public void AddObj(MapPointObj obj){
        objList.Add(obj);
    }
}

public class MazeMapManager : BaseGameMapManager {

    const float mapscale = 1;
    int row = 100, col = 100;
    int UPDATE_MAP_SIZE = 30;
    MazeCreate mazeCreate;
    int Accumulation = 95;//障碍堆积系数
    int Erosion = 20;//障碍侵蚀系数

    int mapGroup = 1;

    public InGameMapPointData[,] map;  
    public int[,] astarArray;// 0 null,1 路 ,2 障碍
    GameObject mapObj;

    Dictionary<int, List<GameObject>> objPool = new Dictionary<int, List<GameObject>>();

    List<InGameMapPointData> lastScreenObj = new List<InGameMapPointData>();

    public override void Init()
    {
        mapObj = new GameObject("map");

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

        map = new InGameMapPointData[row, col];
        astarArray = new int[row, col];

        startPoint = mazeCreate.tree.position;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (IsPointType(i, j, MazeCreate.PointType.way) )
                {
                    astarArray[i, j] = 1;
                    //map[i, j] = new InGameMapPointData(MazeCreate.PointType.way,new Vector2(i,j));

                    if (mazeCreate.mapList[i][j] == (int)MazeCreate.PointType.fullway) continue;
                    int scale = GetMaxFullSpace(i, j,MazeCreate.PointType.way);

                    while(scale > 1 && !ConfigManager.mapObjConfManager.groupMap[mapGroup].ContainsKey(scale)){
                        scale--;
                    }

                    for (int x = 0; x < scale; x++)
                    {
                        for (int y = 0; y < scale; y++)
                        {
                            mazeCreate.mapList[i + x][j + y] = (int)MazeCreate.PointType.fullway;
                            map[i+x, j+y] = new InGameMapPointData(MazeCreate.PointType.way, new Vector2(i + x, j + y));
                        }
                    }

                    Vector3 objpos = new Vector3(i + (float)(scale - 1f) / 2f, j + (float)(scale - 1f) / 2f, 0);
                    CreateGround(objpos, mapGroup, scale);

                }
                else if(IsNearFullGround(i,j))
                {
                    map[i, j] = new InGameMapPointData(MazeCreate.PointType.wall, new Vector2(i , j ));

                    CreateGround(new Vector3(i, j), mapGroup , 1);
                    CreateGround(new Vector3(i, j), mapGroup + 100000, 1);

                    mazeCreate.mapList[i][j] = (int)MazeCreate.PointType.wallfull;
                }
                else{
                    if (mazeCreate.mapList[i][j] == (int)MazeCreate.PointType.wallfull) continue;
                    int scale = GetMaxFullSpace(i, j, MazeCreate.PointType.wall);

                    while (scale > 1 && !ConfigManager.mapObjConfManager.groupMap[mapGroup + 100000].ContainsKey(scale))
                    {
                        scale--;
                    }
                    if(scale < 2){
                        continue;
                    }

                    for (int x = 0; x < scale; x++)
                    {
                        for (int y = 0; y < scale; y++)
                        {
                            map[i+x, j+y] = new InGameMapPointData(MazeCreate.PointType.wall, new Vector2(i, j));
                            CreateGround(new Vector3(i+x, j+y), mapGroup, 1);
                            mazeCreate.mapList[i + x][j + y] = (int)MazeCreate.PointType.wallfull;
                        }
                    }

                    CreateGround(new Vector3(i + (float)(scale - 1f) / 2f, j + (float)(scale - 1f) / 2f, 0), mapGroup + 100000, scale);

                }
            }
        }

        StaticBatchingUtility.Combine(mapObj);

    }

    //动态生成地面
    public override void Update()
    {
        //return;
        Vector2 startPos = InGameManager.GetInstance().inGameCameraManager.GetCameraPos();

        Vector2 startMapPos = GameCommon.GetMapPos(startPos);

        int startX = (int)startMapPos.x - UPDATE_MAP_SIZE / 2;
        int startY = (int)startMapPos.y - UPDATE_MAP_SIZE / 2;

        for (int i = 0; i < lastScreenObj.Count; i ++){
            Vector2 pos = lastScreenObj[i].pos;
            if (pos.x >= startX && pos.x < startX + UPDATE_MAP_SIZE-1 && 
                pos.y >= startY && pos.y < startY + UPDATE_MAP_SIZE-1) continue;

            List<MapPointObj> _list = lastScreenObj[i].objList;
            for (int j = 0; j < _list.Count; j ++){
                if (_list[j].obj == null) continue;
                AddPoolObj(_list[j].conf.id,_list[j].obj);
                _list[j].obj = null;
            }
        }

        lastScreenObj.Clear();

        for (int x = 0; x < UPDATE_MAP_SIZE; x++){
            for (int y = 0; y < UPDATE_MAP_SIZE; y++)
            {
                int sx = startX + x;
                int sy = startY + y;
                if (sx < 0 || sx >= map.GetLength(0)) break;
                if (sy < 0 || sy >= map.GetLength(1)) continue;

                InGameMapPointData data = map[sx,sy];
                if(data == null){
                    continue;
                }
                for (int i = 0; i < data.objList.Count; i ++){
                    if(data.objList[i].obj == null){
                        GameObject obj = GetPoolObj(data.objList[i].conf);
                        obj.transform.position = GameCommon.GetWorldPos(data.objList[i].pos);
                        GameCommon.SetObjZIndex(obj, 0);
                        data.objList[i].obj = obj;
                    }
                }
                lastScreenObj.Add(data);
            }
        }
    }

    //把物体放到池里
    void AddPoolObj(int id,GameObject obj){
        obj.transform.position = new Vector3(-10000, 0, 0);
        if (!objPool.ContainsKey(id))
        {
            objPool.Add(id, new List<GameObject>());
        }

        objPool[id].Add(obj);
    }

    //从池里取出一个物体
    GameObject GetPoolObj(MapObjConf conf){
        GameObject obj = null;
        if (!objPool.ContainsKey(conf.id) || objPool[conf.id].Count <= 0){
            GameObject column = (GameObject)Resources.Load(conf.path);
            obj = MonoBehaviour.Instantiate(column);
            obj.transform.parent = mapObj.transform;
        }else {
            obj = objPool[conf.id][objPool[conf.id].Count - 1];
            objPool[conf.id].RemoveAt(objPool[conf.id].Count - 1);
        }
        return obj;
    }


    int GetMaxFullSpace(int i, int j,MazeCreate.PointType type)
    {
        int z = 0;
        while (true)
        {
            z++;
            if (i + z >= row || j + z >= col) break;
            bool isfull = true;
            for (int x = 0; x <= z; x++)
            {
                int val1 = mazeCreate.mapList[i + x][j + z  ];
                int val2 = mazeCreate.mapList[i + x][j      ];
                if (val1== (int)type && val2 == (int)type)
                {

                }
                else
                {
                    isfull = false;
                    break;
                }
            }
            if (!isfull) break;

            for (int y = 0; y <= z; y++)
            {
                int val1 = mazeCreate.mapList[i + z ][j + y];
                int val2 = mazeCreate.mapList[i     ][j + y];
                if (val1 == (int)type && val2 == (int)type)
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

    //判断一个障碍是否具有某种属性 例如way fullway startpoint endpoint 都具有way的属性
    bool IsPointType(int i ,int j,MazeCreate.PointType type){

        if (i < 0 || i >= mazeCreate.mapList.Count) return false;
        if (j < 0 || j >= mazeCreate.mapList[i].Count) return false;

        return (mazeCreate.mapList[i][j] & (int)type) == (int)type;
    }

    //障碍侵蚀
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

    //道路侵蚀
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

    //道路侵蚀
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

    void CreateGround(Vector3 v,int group, int scale)
    {
        if(!ConfigManager.mapObjConfManager.groupMap[group].ContainsKey(scale)){
            return ;
        }
        List<MapObjConf> list = ConfigManager.mapObjConfManager.groupMap[group][scale];
        MapObjConf conf = list[Random.Range(0, list.Count)];
        //GameObject column = (GameObject)Resources.Load(conf.path);
        //column = MonoBehaviour.Instantiate(column);

        //Vector3 worldpos = GameCommon.GetWorldPos(v);
        //column.transform.position = worldpos ;

        //GameCommon.SetObjZIndex(column, 0);

        //column.transform.parent = mapObj.transform;

        map[(int)v.x, (int)v.y].AddObj(new MapPointObj(conf, null,v));
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

    //获取一个随机的路径点
    //随机一个点,向周围放大寻找
    public Vector3 GetRandomWay(){
        int x = Random.Range(0, map.GetLength(0));
        int y = Random.Range(0, map.GetLength(1));

        if(GetPointType(x, y) != MazeCreate.PointType.way){
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

    //判断一个点是否路径
    public bool IsPointWay(int x,int y){
        return GetPointType(x, y) == MazeCreate.PointType.way;
    }
    //获取地图上一个点的属性
    public MazeCreate.PointType GetPointType(int x, int y)
    {
        if (x < 0 || x >= map.GetLength(0)) return MazeCreate.PointType.nullpoint;
        if (y < 0 || y >= map.GetLength(1)) return MazeCreate.PointType.nullpoint;
        if(map[x, y] == null) return MazeCreate.PointType.nullpoint;
        return map[x, y].type ;
    }



}
