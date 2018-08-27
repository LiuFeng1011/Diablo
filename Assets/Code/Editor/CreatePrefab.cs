using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;  

public class CreatePrefab : MonoBehaviour {

    public const string MenuName = "Tools/Create Prefab";  
  

    [MenuItem(MenuName)]
    public static void CreatePrefabs()
    {
        //获取场景中全部道具
        Object[] objects = Object.FindObjectsOfType(typeof(GameObject));
        GameObject prefabListObj = null;
        foreach (GameObject sceneObject in objects)
        {
            if (sceneObject.name == "prefabs")
            {
                prefabListObj = sceneObject;
                break;
            }
        }

        if(prefabListObj != null) DestroyImmediate(prefabListObj);

        prefabListObj = new GameObject("prefabs");

        //==========================生成预制体================================
        List<MapObjConf> confs = ConfigManager.GetEditorMapObjConfManager().datas;
        MapObjConfManager confMgr = ConfigManager.GetEditorMapObjConfManager();

        UnityEngine.Object[] arr = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
        //for (int i = 0; i < arr.Length; i++)
        //{
        //    string path = AssetDatabase.GetAssetPath(arr[i]);
        //    Debug.Log(path);
        //}
        //GameObject[] gos = Selection.gameObjects;
        Material mat = Resources.Load<Material>("Materials/item_obj");

        for (int i = 0; i < arr.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(arr[i]);
            Debug.Log(path);
            int indexnum = path.LastIndexOf('/')+1;
            string prefabFullName = path.Substring(indexnum,path.Length -indexnum);
            Debug.Log(prefabFullName);
            indexnum = prefabFullName.LastIndexOf('.')+1;
            string prefabName = prefabFullName.Substring(0, indexnum-1);
            Debug.Log(prefabName);
            MapObjConf conf = confMgr.GetConfByPrefabName(prefabName);
            if(conf == null){
                Debug.LogError(prefabName + " config is null!!");
                continue;
            }
            GameObject gameObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/" + conf.path + ".prefab");
            if (gameObj != null)
            {
                continue;
            }
            Debug.Log("create " + prefabName + " prefab !");
            //创建gameobject
            GameObject prefabObj = new GameObject(prefabName);
            InGameBaseMapObj mapobj = prefabObj.AddComponent<InGameBaseMapObj>();
            mapobj.confid = conf.id;

            //加载sprite
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if(sprite == null){
                Debug.LogError(prefabName + " sprite is null !!!!");
                continue;
            }
            //创建spriterenderer
            GameObject spriteObj = new GameObject("sprite");
            SpriteRenderer sr = spriteObj.AddComponent<SpriteRenderer>();
            spriteObj.transform.parent = prefabObj.transform;
            sr.sprite = sprite;

            spriteObj.transform.localScale = new Vector3(1f, 1f, 1f);
            spriteObj.transform.localPosition = new Vector3(0f, -0.613f, 0f);

            spriteObj.GetComponent<Renderer>().material = mat;
            //创建预制体
            PrefabUtility.CreatePrefab("Assets/Resources/" + conf.path + ".prefab", prefabObj);

            //生成预制体物体
            //GameObject tempObj = Resources.Load(conf.path) as GameObject;
            GameObject pre = AssetDatabase.LoadAssetAtPath("Assets/Resources/" + conf.path + ".prefab", typeof(GameObject)) as GameObject;
            pre = PrefabUtility.InstantiatePrefab(pre) as GameObject;

            //Debug.Log("create : " + prefabName);
            pre.transform.parent = prefabListObj.transform;

            DestroyImmediate(prefabObj);

        }
    }


    [MenuItem("Tools/testselect")]
    public static void testselect()
    {
        UnityEngine.Object[] arr = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
        for (int i = 0; i < arr.Length; i ++){
            string path = AssetDatabase.GetAssetPath(arr[i]);
            Debug.Log(path);
        }
    }
    //public static void CreatePrefabs(){
        
    //    GameObject[] gos = Selection.gameObjects;
    //    MapObjConfManager confMgr = ConfigManager.GetEditorMapObjConfManager();
    //    for (int i = gos.Length - 1; i >= 0; i --){
    //        GameObject go = gos[i];

    //        string prefabName = go.name;

    //        MapObjConf conf =confMgr.GetConfByPrefabName(prefabName) ;
    //        if(conf == null){
    //            Debug.Log(prefabName + " config is null!!!!");
    //            continue;
    //        }
    //        GameObject gameObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/" + conf.path+ ".prefab");
    //        if(gameObj != null){
    //            continue;
    //        }
    //        GameObject prefabObj = new GameObject(prefabName);
    //        InGameBaseMapObj mapobj = prefabObj.AddComponent<InGameBaseMapObj>();
    //        mapobj.confid = conf.id;

    //        GameObject spriteObj = new GameObject("sprite");
    //        SpriteRenderer sr = spriteObj.AddComponent<SpriteRenderer>();
    //        spriteObj.transform.parent = prefabObj.transform;


    //        sr.sprite = go.GetComponent<SpriteRenderer>().sprite;

    //        prefabObj.transform.localScale = new Vector3(2.2f, 2.2f, 1f);
    //        prefabObj.transform.localPosition = new Vector3(0f, -0.07f, 0f);

    //        PrefabUtility.CreatePrefab("Assets/Resources/" + conf.path + ".prefab", prefabObj);

    //        //GameObject tempObj = Resources.Load(conf.path) as GameObject;
    //        GameObject pre = AssetDatabase.LoadAssetAtPath("Assets/Resources/" + conf.path + ".prefab", typeof(GameObject)) as GameObject;
    //        pre = PrefabUtility.InstantiatePrefab(pre) as GameObject;
    //        //Sprite.Instantiate()
    //        //Debug.Log("create : " + prefabName);
    //        pre.transform.parent = go.transform.parent;

    //        DestroyImmediate(go);

    //        DestroyImmediate(prefabObj);

    //    }
    //}

}
