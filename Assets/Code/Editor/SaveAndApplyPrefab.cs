using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 

public class SaveAndApplyPrefab : MonoBehaviour {

    public const string KeyName = "Tools/应用并删除预制体 &d";  
  

    [MenuItem(KeyName)]  
    static void SeletEnable()  
    {  
        bool enable = false;  
        GameObject[] gos = GetSelectedGameObjects() as GameObject[];  
  
        foreach (GameObject go in gos)  
        {
            GameObject prefab = go.transform.parent.gameObject;
            if (PrefabUtility.GetCorrespondingObjectFromSource(prefab) != null)
            {
                PrefabUtility.ReplacePrefab(prefab, PrefabUtility.GetCorrespondingObjectFromSource(prefab), ReplacePrefabOptions.ConnectToPrefab);
                DestroyImmediate(prefab);
            }
        }  
    }  

    //获得选中的物体  
    static GameObject[] GetSelectedGameObjects()  
    {  
        return Selection.gameObjects;  
    }  
}
