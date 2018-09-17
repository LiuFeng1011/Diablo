﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptTexture : MonoBehaviour {

    public Color color1 = Color.green;
 
    public Color color2 = Color.blue;
 
    public int width = 640, height = 960;
 
    SpriteRenderer sr;
    // Use this for initialization
    void Start()
    {
        GameObject obj = new GameObject("spriet");
 
        sr = obj.AddComponent<SpriteRenderer>();

        obj.transform.parent = transform;

        //width = Screen.width;
        //height = Screen.height;

        GenerateSprite();

        obj.transform.localPosition = new Vector3(0,0,999);

        //设置尺寸
        float cameraHeight = Camera.main.orthographicSize * 2;
        Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);
        Vector2 spriteSize = sr.sprite.bounds.size;   

        Vector2 scale = transform.localScale;
        if (cameraSize.x >= cameraSize.y)
        { // Landscape (or equal)
            scale *= cameraSize.x / spriteSize.x;
        }
        else
        { // Portrait
            scale *= cameraSize.y / spriteSize.y;
        }

        obj.transform.localScale = scale;
    }
 
    // Update is called once per frame
    void GenerateSprite()
    {
        Texture2D t = new Texture2D(width, height);
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                float wrate = (float)w / (float)width;
                float hrate = (float)h / (float)height;
 
                float bezieratVal = GetBezierat(0, 0.5f, 0.5f, 1, hrate);
 
                float dis = Vector2.Distance(new Vector2(0.5f, 0.5f),new Vector2(wrate,hrate));
                dis = 1 - GetBezierat(0, 0, 1, 2, dis) * 0.6f;  
 
                Color c = new Color();
                c.r = Mathf.Lerp(color1.r, color2.r, bezieratVal) * dis;
                c.g = Mathf.Lerp(color1.g, color2.g, bezieratVal) * dis;
                c.b = Mathf.Lerp(color1.b, color2.b, bezieratVal) * dis;
                c.a = 255;
 
                t.SetPixel(w, h,c);
            }
        }
        t.Apply();
 
        Sprite pic = Sprite.Create(t, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        sr.sprite = pic;
    }
 
    float GetBezierat(float a, float b, float c, float d, float t)
    {
        return (Mathf.Pow(1 - t, 3) * a +
                3 * t * (Mathf.Pow(1 - t, 2)) * b +
                3 * Mathf.Pow(t, 2) * (1 - t) * c +
                Mathf.Pow(t, 3) * d);
    }
 

}
