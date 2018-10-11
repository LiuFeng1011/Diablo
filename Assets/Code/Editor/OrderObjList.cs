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

            Transform transform = go[i].transform;
            int size = (int)Mathf.Sqrt(transform.childCount);
            for (int j = 0; j < transform.childCount;j ++){
                Transform child = transform.GetChild(j);
                child.transform.localPosition = GameCommon.GetWorldPos(new Vector3(j / size , j % size , 0));
            }


        }

    }  
  
}
