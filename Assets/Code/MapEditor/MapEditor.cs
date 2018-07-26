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
