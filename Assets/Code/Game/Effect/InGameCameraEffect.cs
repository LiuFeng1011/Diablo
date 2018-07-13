using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] 
public class InGameCameraEffect : MonoBehaviour {

    public Material m;

    public float _Camber = 1.05f, _Radius = 1.65f;

    float targetVal,nowVal;

    private void Awake()
    {
        targetVal = _Radius;
        nowVal  = _Radius;
    }

    // Use this for initialization
    void Start () {
        m = new Material(Shader.Find("Custom/CameraFog"));  

        m.SetFloat("_Camber", _Camber);  
        m.SetFloat("_Radius", _Radius);  

	}
	
	// Update is called once per frame
	void Update () {

        nowVal = nowVal + (targetVal - nowVal) * 0.3f;

        m.SetFloat("_Camber", _Camber);  
        m.SetFloat("_Radius", nowVal);  

	}

    public void Hide(){
        targetVal = 0;
    }

    public void Show(){
        targetVal = _Radius;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)  
    {  
        Graphics.Blit(src, dest, m);  
    }  
}
