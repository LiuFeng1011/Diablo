using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class MapEditor : MonoBehaviour {

    public Color sideColor = Color.white;
    public Color insideColor = Color.black;

	public int mapHeight = 100;
	public int mapWidth = 100;

    public bool isAutoPos = true;
	public float autoDPos = 0.1f;

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
		
        for (float y = 0; y < mapHeight; y ++)
		{
            Gizmos.DrawLine(GameCommon.GetWorldPos(new Vector3(0, y, -1f)),
                            GameCommon.GetWorldPos(new Vector3(mapWidth, y, -1f)));
			
            //Handles.Label(new Vector3(-1f,y+0.5f , 0f), "" + y);
		}
		
		for (float x = 0; x < mapWidth; x ++)
		{
            Gizmos.DrawLine(GameCommon.GetWorldPos(new Vector3(x, 0, -1f)),
                            GameCommon.GetWorldPos(new Vector3(x, mapHeight, -1f)));
			
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
        Vector2 mappos = GameCommon.GetMapPos(pos);

        obj.transform.position = GameCommon.GetWorldPos(mappos);
        GameCommon.SetObjZIndex(obj, 1);
        return;
        //MapEditor me = GameObject.Find("levelOption").transform.GetComponent<MapEditor>();

        //if (me == null)
        //{
        //    EditorUtility.DisplayDialog("ERROR", "Cant't find the 'levelOption' !!", "ENTRY");
        //    return;
        //}

        float autoDPosx = 1;
        float autoDPosy = 1;

        float alignedx = 0;
        float alignedy = 0;

        if (autoDPosx == 0)
        {
            alignedx = pos.x;
        }
        else
        {
            alignedx = Mathf.Floor(pos.x / autoDPosx) * autoDPosx ;
        }
        if (autoDPosy == 0)
        {
            alignedy = pos.y;
        }
        else
        {
            alignedy = Mathf.Floor(pos.y / autoDPosy) * autoDPosy ;
        }
        Vector3 aligned = new Vector3(alignedx, alignedy, 0.0f);
        obj.transform.position = aligned;
    }
}
