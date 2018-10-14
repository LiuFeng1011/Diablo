using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 

public class DelParentObj : MonoBehaviour {

    public const string KeyName = "Tools/删除 &x";  
  
    [MenuItem(KeyName)]  
    static void SeletDelete()  
    {  
        Debug.Log("111");
        GameObject[] gos = GetSelectedGameObjects() as GameObject[];  
  
        foreach (GameObject go in gos)  
        {
            GameObject obj = go.transform.parent.gameObject;
            DestroyImmediate(obj);
        }  
    }  

    //获得选中的物体  
    static GameObject[] GetSelectedGameObjects()  
    {  
        return Selection.gameObjects;  
    }  
}
