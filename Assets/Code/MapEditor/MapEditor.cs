using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class MapEditor : MonoBehaviour {
    
    public Color sideColor = Color.white;
    public Color insideColor = Color.black;

    public LevelOption lo;

    public bool isAutoPos = true;

    public static bool s_isAutoSelMapObj = true;

    static GameObject copyObj = null;

    #if UNITY_EDITOR
    [InitializeOnLoadMethod]
    static void Init()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;

        Selection.selectionChanged = SelectionChange;
    }

    static Vector3 mouseStartDownpos = Vector3.zero;
    static void OnSceneGUI(SceneView sceneView)
    {
        if (Event.current.type == EventType.MouseDown)
        {
            mouseStartDownpos = Event.current.mousePosition;
            return;
        }
        if (Event.current.type == EventType.MouseUp)
        {
            if(Vector3.Distance(mouseStartDownpos,Event.current.mousePosition) > 0.3f){
                return;
            }
            //Event.current.Use();
            if (Selection.gameObjects.Length <= 0) return;
            GameObject obj = Selection.gameObjects[0];
            //Debug.Log(obj.name);

            InGameBaseMapObj mapObj = obj.GetComponent<InGameBaseMapObj>();
            if (mapObj == null) return;
            MapObjConf conf = ConfigManager.GetEditorMapObjConfManager().map[mapObj.confid];
            GameObject tempObj = Resources.Load(conf.path) as GameObject;
            tempObj = (GameObject)Instantiate(tempObj);

            tempObj.transform.position = GetWorldPosition(sceneView);
            tempObj.transform.parent = obj.transform.parent;
            GameCommon.SetObjZIndex(tempObj, conf.depth);

            //聚焦到当前物体
            Selection.activeGameObject = tempObj;
            copyObj = tempObj;
            //Event.current.Use();

        }
    }

    static void SelectionChange()
    {
        //    if (Selection.activeObject == null) return;
        //    Debug.Log("SelectionChange : " + Selection.activeObject.name);
        //    if (copyObj == Selection.activeGameObject) return;
        //    if (copyObj == null) return;
        //    Debug.Log("select!!");
        //    Selection.activeGameObject = copyObj;
        //    copyObj = null;
        //    //GameObject selobj = (GameObject)Selection.activeObject;
        //    //InGameBaseMapObj mapObj = selobj.GetComponent<InGameBaseMapObj>();
        //    //if (mapObj == null) return;
        //    //copyObjId = mapObj.confid;

        if (Selection.activeObject == null) return;
    }
    static Vector3 GetWorldPosition(SceneView sceneView)
    {
        Camera cam = sceneView.camera;
        Vector3 mousepos = Event.current.mousePosition;
        mousepos.z = 0;
        mousepos.x *= 2;
        mousepos.y = (cam.pixelHeight - mousepos.y * 2);

        mousepos = sceneView.camera.ScreenToWorldPoint(mousepos);
        return mousepos;
    }
    #endif
	// Use this for initialization
	void Start () {
	
	}

	void OnDrawGizmos()
	{
        DrawBGLine();
	}

    public void DrawBGLine(){
        #if UNITY_EDITOR
        Gizmos.color = insideColor;
		
        for (float y = 0; y < lo.mapHeight; y ++)
		{
            Gizmos.DrawLine(GameCommon.GetWorldPos(new Vector3(0, y, -1f)),
                            GameCommon.GetWorldPos(new Vector3(lo.mapWidth-1, y, -1f)));
			
            //Handles.Label(new Vector3(-1f,y+0.5f , 0f), "" + y);
		}
		
        for (float x = 0; x < lo.mapWidth; x ++)
		{
            Gizmos.DrawLine(GameCommon.GetWorldPos(new Vector3(x, 0, -1f)),
                            GameCommon.GetWorldPos(new Vector3(x, lo.mapHeight-1, -1f)));
			
            //Handles.Label(new Vector3(x,-0.2f, 0f), "" + x);
        }

        Gizmos.color = sideColor;

        #endif
	}
    void Update()
    {
        AutoSetPos();
        #if UNITY_EDITOR

        if (copyObj == null || copyObj == Selection.activeGameObject) return;
        Selection.activeGameObject = copyObj;
        copyObj = null;
        #endif
    }

    public void AutoSetPos()
    {
        if (!isAutoPos)
        {
            return;
        }

        //foreach (GameObject sceneObject in Object.FindObjectsOfType(typeof(GameObject))){

        foreach (GameObject sceneObject in Selection.gameObjects)
        {
            SetObjPos(sceneObject, sceneObject.transform.position);
        }
    }
    public static void SetObjPos(GameObject obj, Vector3 pos)
    {
        InGameBaseObj ingameobj = obj.GetComponent<InGameBaseObj>();
        if (ingameobj == null) return;

        Vector2 mappos = GameCommon.GetMapPos(pos);

        obj.transform.position = GameCommon.GetWorldPos(mappos);

        MapObjConf conf = ConfigManager.GetEditorMapObjConfManager().map[ingameobj.confid];
        if (conf == null) return;
        GameCommon.SetObjZIndex(obj, conf.depth);
        return;
        //MapEditor me = GameObject.Find("levelOption").transform.GetComponent<MapEditor>();

        //if (me == null)
        //{
        //    EditorUtility.DisplayDialog("ERROR", "Cant't find the 'levelOption' !!", "ENTRY");
        //    return;
        //}

    }
}
