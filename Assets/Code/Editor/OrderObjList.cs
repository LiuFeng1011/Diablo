using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 

public class OrderObjList : MonoBehaviour {

    //根据当前有没有选中物体来判断可否用快捷键  
    [MenuItem("Tools/OrderObjList")]  
    static void OrderSelObjList()  
    {  
        GameObject[] go = Selection.gameObjects;

        for (int i = 0; i < go.Length;  i++){
            go[i].transform.localPosition = GameCommon.GetWorldPos(go[i].transform.parent.position + new Vector3(0,i,0));
        }

    }  
  
}
