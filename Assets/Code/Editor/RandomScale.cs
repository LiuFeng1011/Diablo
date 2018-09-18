using UnityEngine;
using UnityEditor;  
public class RandomScale : MonoBehaviour
{


    public const string KeyName = "Tools/RandomScale";

    [MenuItem(KeyName)]
    static void SeletEnable()
    {
        GameObject[] gos = GetSelectedGameObjects() as GameObject[];

        foreach (GameObject go in gos)
        {
            float randomscale = Random.Range(0.5f,1.3f);
            go.transform.localScale = new Vector3(randomscale,randomscale,1);
        }
    }

    //获得选中的物体  
    static GameObject[] GetSelectedGameObjects()
    {
        return Selection.gameObjects;
    }

}
