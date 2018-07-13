using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CaptureModeTexture : MonoBehaviour {

    [MenuItem("Tools/CaptureModelOneTexture &c")]
    private static void CaptureOne()
    {
        Camera c = null;
        string name = "";

        Transform[] transforms = Selection.transforms;
        if(transforms.Length <= 0){

            Debug.LogError("Cant find model!!");
            return;
        }

        name = transforms[0].name;

        //获取场景中全部道具
        Object[] objects = Object.FindObjectsOfType(typeof(GameObject));
        foreach (GameObject sceneObject in objects)
        {
            if (sceneObject.name == "Camera")
            {
                c = sceneObject.GetComponent<Camera>();
            }
        }

        if (!c)
        {
            Debug.LogError("Cant find camera!!");
            return;
        }


        Capture(c, new Rect(0, 0, Screen.width, Screen.height), name);
    }

    [MenuItem("Tools/CaptureModelAllTexture")]
    private static void Capture()
    {
        Camera c = null;
        GameObject model = null;

        Vector3 capturePos = new Vector3(1000, 0.386f, 0);

        //获取场景中全部道具
        Object[] objects = Object.FindObjectsOfType(typeof(GameObject));
        string name = "";
        foreach (GameObject sceneObject in objects)
        {
            if (sceneObject.name == "Camera")
            {
                c = sceneObject.GetComponent<Camera>();
            }else if(sceneObject.name == "Model"){
                model = sceneObject;
            }
        }
        if(!model){
            Debug.LogError("Cant find model!!");
            return;
        }
        if (!c){
            Debug.LogError("Cant find camera!!");
            return;
        }

        //c.transform.position = new Vector3(1000, 0, -2);

        Transform old = null;
        foreach(Transform t in model.transform){
            if(old != null){
                old.gameObject.SetActive(false);
            }
            old = t;
            t.gameObject.SetActive(true);
            name = t.name;
            Capture(c, new Rect(0, 0, Screen.width, Screen.height), name);
        }

    }

    public static Texture2D Capture(Camera came, Rect r,string name)
    {
        RenderTexture rt = new RenderTexture((int)r.width, (int)r.height, 0);

        came.targetTexture = rt;
        came.Render();

        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)r.width, (int)r.height, TextureFormat.ARGB32, false);

        screenShot.ReadPixels(r, 0, 0);

        screenShot.Apply();

        came.targetTexture = null;
        RenderTexture.active = null;


        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Application.streamingAssetsPath + "/" + name + ".png";
        System.IO.File.WriteAllBytes(filename, bytes);

        return screenShot;
    }
}
