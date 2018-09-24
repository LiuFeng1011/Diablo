using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPointObj{
    public Vector2 pos;
    public Vector3 scale;
    public MapObjConf conf;
    public GameObject obj;
    public MapPointObj(MapObjConf conf,GameObject obj, Vector2 pos,Vector3 scale){
        this.conf = conf;
        this.obj = obj;
        this.pos = pos;
        this.scale = scale;
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

    public enum MapType{
        en_type_1 = 1,//随机迷宫
        en_type_2 = 2,//
        en_type_editormap = 3,
        en_type_fixeditormap = 4,
        en_type_randomgroup = 5,
    }

    const float mapscale = 1;
    public int row = 100, col = 100;
    public int UPDATE_MAP_SIZE = 30;
    public MazeCreate mazeCreate;
    public int Accumulation = 99;//障碍堆积系数
    public int Erosion = 50;//障碍侵蚀系数

    public int mapGroup = 1;

    public InGameMapPointData[,] map;  
    public int[,] astarArray;// 0 null,1 路 ,2 障碍
    public GameObject mapObj;

    public SmallMap smallMap;

    public Dictionary<int, List<GameObject>> objPool = new Dictionary<int, List<GameObject>>();

    public List<InGameMapPointData> lastScreenObj = new List<InGameMapPointData>();

    protected List<Vector2> startPointList = new List<Vector2>();

    /// <summary>
    /// Creates the map manager.
    /// </summary>
    /// <returns>The map manager.</returns>
    /// <param name="type">Type.</param>
    /// <param name="group">Group.fix editor map 时传入关卡id</param>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    public static MazeMapManager CreateMapManager(MapType type,int group,int row,int col){
        MazeMapManager manager;
        switch(type){
            case MapType.en_type_2:
                manager = new MapType2();
                break;
            case MapType.en_type_editormap:
                manager = new MapTypeEditorMap();
                break;
            case MapType.en_type_fixeditormap :
                manager = new FixEditorMap();
                break;
            case MapType.en_type_randomgroup:
                manager = new MapTypeRandomGroup();
                break;
            default:
                manager = new MapType1();
                break;
        }
        manager.InitMap( group,  row,  col);
        return manager;
    }

    public virtual void InitMap(int group, int row, int col)
    { 
        mapObj = new GameObject("map");
        mapGroup = group;
        this.row = row;
        this.col = col;

        map = new InGameMapPointData[row, col];
        astarArray = new int[row, col];

        smallMap = new SmallMap();
        smallMap.Init(row,col);
    }
    //动态生成地面
    public override void Update()
    {
        smallMap.Update();

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

                int _x = UPDATE_MAP_SIZE / 2 - x;
                int _y = UPDATE_MAP_SIZE / 2 - y;

                if (sx < 0 || sx >= map.GetLength(0)) break;
                if (sy < 0 || sy >= map.GetLength(1)) continue;

                bool isopen = false;
                //小地图中的点是否打开，如果已经打开 则不用计算距离
                if(!smallMap.IsOpenPoint(sx, sy)){
                    if (Mathf.Sqrt(_x * _x + _y * _y) <= UPDATE_MAP_SIZE / 2)
                    {
                        isopen = true;
                    }
                }

                InGameMapPointData data = map[sx,sy];
                if(data == null){
                    if(isopen)smallMap.OpenUnit(sx, sy,MazeCreate.PointType.nullpoint);
                    continue;
                }

                if (isopen)smallMap.OpenUnit(sx, sy, data.type);

                for (int i = 0; i < data.objList.Count; i ++){
                    if(data.objList[i].obj == null){
                        MapPointObj mapPointObj = data.objList[i];
                        GameObject obj = GetPoolObj(mapPointObj.conf);
                        obj.transform.position = GameCommon.GetWorldPos(mapPointObj.pos);

                        obj.transform.localScale = mapPointObj.scale;
                        GameCommon.SetObjZIndex(obj, mapPointObj.conf.depth);
                        mapPointObj.obj = obj;
                    }
                }
                lastScreenObj.Add(data);
            }
        }
    }

    //把物体放到池里
    protected void AddPoolObj(int id,GameObject obj){
        //obj.transform.position = new Vector3(-10000, 0, 0);
        if (!objPool.ContainsKey(id))
        {
            objPool.Add(id, new List<GameObject>());
        }

        objPool[id].Add(obj);
    }

    //从池里取出一个物体
    protected  GameObject GetPoolObj(MapObjConf conf){
        GameObject obj = null;
        if (!objPool.ContainsKey(conf.id) || objPool[conf.id].Count <= 0){
            GameObject column = (GameObject)Resources.Load(conf.path);
            if (column == null)
            {
                Debug.LogError(conf.path + "  is null!!!!");
                return null;
            }
            obj = MonoBehaviour.Instantiate(column);

            obj.transform.parent = mapObj.transform;
        }else {
            obj = objPool[conf.id][objPool[conf.id].Count - 1];
            objPool[conf.id].RemoveAt(objPool[conf.id].Count - 1);
        }
        return obj;
    }


    protected int GetMaxFullSpace(int i, int j,MazeCreate.PointType type)
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

    protected bool IsNearFullGround(int i,int j){
        if(IsPointType(i,j+1,MazeCreate.PointType.way) &&
           IsPointType(i,j-1,MazeCreate.PointType.way) &&
           IsPointType(i+1,j,MazeCreate.PointType.way) &&
           IsPointType(i-1,j,MazeCreate.PointType.way) 
           ){
            return true;
        }
        return false;
    }

    protected void InsertGroup(Vector2 pos, int groupid)
    {
        int x = (int)pos.x, y = (int)pos.y;
        MapGroupConf conf = ConfigManager.mapGroupConfManager.dataMap[groupid];
        if (conf == null) return;
        TextAsset text = Resources.Load(conf.path) as TextAsset;
        byte[] data = GameCommon.UnGZip(text.bytes);
        DataStream datastream = new DataStream(data, true);

        //配置LevelOption
        GameObject logo = new GameObject(conf.path);
        //logo.transform.parent = this.mapObj.transform;
        logo.transform.position = GameCommon.GetWorldPos(pos);
        LevelOption me = logo.AddComponent<LevelOption>();
        me.deserialize(datastream);

        //生成物体
        int objcount = datastream.ReadSInt32();

        for (int i = 0; i < objcount; i++)
        {
            //MSBaseObject.CreateObj(datastream);
            //从字节流中获取id
            //int confid = datastream.ReadSInt32();
            //float objx = datastream.ReadSInt32() / 1000f;
            //float objy = datastream.ReadSInt32() / 1000f;
            int confid = 0;
            float objx = 0, objy = 0,objsx = 0, objsy = 0;
            MapObjConf objconf = null;
            GameObject column = null;
            int dataid = datastream.ReadByte();
            string goname = "";
            while (dataid != 0)
            {
                switch (dataid)
                {
                    case 1:
                        confid = datastream.ReadSInt32();
                        objconf = ConfigManager.mapObjConfManager.map[confid];
                        break;
                    case 2: objx = datastream.ReadSInt32() / 1000f; break;
                    case 3: objy = datastream.ReadSInt32() / 1000f; break;
                    case 4:
                        int parentid = datastream.ReadSInt32();
                        break;
                    case 5:
                        int instanceid = datastream.ReadSInt32();
                        break;
                    case 7: goname = datastream.ReadString16(); break;
                    case 8: 
                        objsx = datastream.ReadSInt32() / 1000f;
                        break;
                    case 9: 
                        objsy = datastream.ReadSInt32() / 1000f; 
                        break;
                    case 6:
                        if(objconf.isstatic == 1){
                            column = (GameObject)Resources.Load(objconf.path);
                            column = MonoBehaviour.Instantiate(column);
                            column.transform.parent = mapObj.transform;
                            column.transform.position = GameCommon.GetWorldPos(pos) + new Vector2(objx, objy);
                            InGameBaseObj point = column.GetComponent<InGameBaseObj>();
                            point.Deserialize(datastream);
                            GameCommon.SetObjZIndex(column,objconf.depth);
                        }

                        break;
                }
                dataid = datastream.ReadByte();
            }

            if (confid == 4000002)
            {
                startPointList.Add(pos + GameCommon.GetMapPos(new Vector2(objx, objy)));
                continue;
            }
            if (objconf.isstatic == 1){
                SetWayProperty(pos + GameCommon.GetMapPos(new Vector2(objx, objy)), objconf);
                column.transform.localScale = new Vector3(objsx, objsy, 1);
                continue;
            }

            SetGroupPoint(pos + GameCommon.GetMapPos(new Vector2(objx, objy)), objconf,new Vector3(objsx, objsy,1));
        }
    }

    protected void SetGroupPoint(Vector2 pos, MapObjConf objConf,Vector3 scale)
    {
        int x = (int)pos.x, y = (int)pos.y;
        if (map[x, y] == null)
        {
            map[x, y] = new InGameMapPointData(MazeCreate.PointType.wall, pos);
        }

        map[x, y].AddObj(new MapPointObj(objConf, null, pos ,scale));

        SetWayProperty(pos,objConf);
    }

    protected void SetWayProperty(Vector2 pos, MapObjConf objconf){
        int x = (int)pos.x, y = (int)pos.y;
        MazeCreate.PointType type;
        int arrval = 0;
        if (objconf.depth >= 3)
        {
            type = MazeCreate.PointType.wallfull;
            arrval = 0;
        }
        else
        {
            type = MazeCreate.PointType.way;
            arrval = 1;
        }

        for (int i = 0; i < objconf.sizeX; i++)
        {
            for (int j = 0; j < objconf.sizeY; j++)
            {
                int _x = x + i;
                int _y = y + j;
                if (map[_x, _y] == null)
                {
                    map[_x, _y] = new InGameMapPointData(MazeCreate.PointType.wall, new Vector2(_x, _y));
                }
                if (map[_x, _y].type == MazeCreate.PointType.wallfull) continue;
                map[_x, _y].type = type;
                astarArray[_x, _y] = arrval;
            }
        }
    }

    //判断一个障碍是否具有某种属性 例如way fullway startpoint endpoint 都具有way的属性
    protected bool IsPointType(int i ,int j,MazeCreate.PointType type){

        if (i < 0 || i >= mazeCreate.mapList.Count) return false;
        if (j < 0 || j >= mazeCreate.mapList[i].Count) return false;

        return (mazeCreate.mapList[i][j] & (int)type) == (int)type;
    }

    //障碍侵蚀
    protected void AccumulationMap(MapTree tree,float accumulation)
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
    protected void ErosionMap(MapTree tree){
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
    protected  void SetErosionMapWay(MapTree tree,int x, int y)
    {
        int rate = Random.Range(0, 100);
        if (rate < Erosion)
        {
            mazeCreate.mapList[x][y] = (int)MazeCreate.PointType.way;
            tree.AddChile(new MapTree(new Vector3(x, y)));
            //CreateGround(new Vector3(x, y + 33));
        }
    }

    protected virtual void CreateGround(Vector3 v,int group, int scale)
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

        map[(int)v.x, (int)v.y].AddObj(new MapPointObj(conf, null,v,new Vector3(1,1,1)));
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
