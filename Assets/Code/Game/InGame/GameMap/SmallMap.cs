using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SmallMapUnit{
    
}

public class SmallMap : BaseGameObject {
    Color maskColor = new Color(0.3f,0.3f,0.3f, 1);
    Color obsColor = new Color(0, 0, 0, 1);
    Color wayColor = new Color(1, 1, 1, 1);
    //小地图每个格子单位的像素尺寸
    const int unitSize = 1;
    const int mapScale = 3;
    //小地图刷新频率
    const float  updateMaxTime = 1f;

    float updateTime = 0.8f;

    //每个刷新周期内,数据是否更新
    bool updateData = false;

    //地图尺寸
    int mapSizeX = 0, mapSizeY = 0,textureWidth = 0,textureHeight = 0;

    Texture2D mapTexture;
    //SpriteRenderer sr;

    RawImage rawImage;

    public int[,] dataArray; // 0 null,1 路 ,2 障碍

    public virtual void Init(int row, int col)
    {
        mapSizeX = row;
        mapSizeY = col;
        dataArray = new int[row, col];
        InitSprite();

    }

    public void InitSprite()
    {
        textureWidth = mapSizeX * unitSize;
        textureHeight = mapSizeY * unitSize;

        mapTexture = new Texture2D(textureWidth, textureHeight);
        for (int w = 0; w < textureWidth; w++)
        {
            for (int h = 0; h < textureHeight; h++)
            {
                mapTexture.SetPixel(w, h, maskColor);
            }
        }
        mapTexture.Apply();

        //GameObject obj = new GameObject("smallMap");
        //sr = obj.AddComponent<SpriteRenderer>();
        ApplySprite();

        rawImage = GameObject.Find("Canvas").transform.Find("SmallMap").Find("RawImage").GetComponent<RawImage>();
        rawImage.texture = mapTexture;
        rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(textureWidth * mapScale,textureHeight*mapScale);
    }

    public virtual void Update()
    {
        updateTime += Time.deltaTime;
        if(updateTime >= updateMaxTime){
            updateTime = 0f;
            if(updateData){
                ApplySprite();
            }

            Vector3 rolePos = InGameManager.GetInstance().inGamePlayerManager.GetRolePos();
            Vector2 arrPos = GameCommon.GetMapPos(rolePos);
            rawImage.transform.localPosition = -mapScale * new Vector2(
                arrPos.x /mapSizeX * textureWidth,arrPos.y / mapSizeY * textureHeight
            );
        }


    }

    public virtual void Destory()
    {

    }

    /// <summary>
    /// Opens the unit.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="type">Type.0 null,1 路 ,2 障碍</param>
    public void OpenUnit(int x ,int y ,int type){
        
        if(dataArray[x,y] != 0){
            return;
        }
        //Debug.Log("OpenUnit : " + x + " y:"+ y);
        dataArray[x, y] = 1;
        updateData = true;

        int w = x * unitSize;
        int h = y * unitSize;

        Color c;
        if (type == 1)
        {
            c = wayColor;
        }
        else
        {
            c = obsColor;
        }

        for (int i = 0; i < unitSize; i++)
        {
            for (int j = 0; j < unitSize; j++)
            {
                mapTexture.SetPixel(w+i, h+j, c);
            }
        }
    }


    void ApplySprite()
    {
        mapTexture.Apply();
        updateData = false;
        //Sprite pic = Sprite.Create(mapTexture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f));
        //sr.sprite = pic;
    }
}
