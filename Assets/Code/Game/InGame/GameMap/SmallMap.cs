using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SmallMapUnit{
    
}

public class SmallMap : BaseGameObject {
    Color maskColor = new Color(0.3f,0.3f,0.3f, 0);
    Color obsColor = new Color(0, 0, 0, 0.5f);
    Color wayColor = new Color(89 / 255f, 51 / 255f, 44 / 255f, 1);
    //小地图每个格子单位的像素尺寸
    const int unitSize = 1;
    //小地图刷新频率
    const float  updateMaxTime = 1f;

    float updateTime = 0.8f;

    //每个刷新周期内,数据是否更新
    bool updateData = false;

    //地图尺寸
    int mapSizeX = 0, mapSizeY = 0,textureWidth = 0,textureHeight = 0;

    Texture2D mapTexture;

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

        ApplySprite();

        EventData.CreateEvent(EventID.EVENT_UI_INITMAPTEXTURE).AddData(mapTexture).Send();
    }

    public virtual void Update()
    {
        updateTime += Time.deltaTime;
        if(updateTime >= updateMaxTime){
            updateTime = 0f;
            if(updateData){
                ApplySprite();
            }
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
    }
}
