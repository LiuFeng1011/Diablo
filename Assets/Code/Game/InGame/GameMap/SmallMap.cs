using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

//每一个角色的信息
public class EnemyInfo{
    public int x,y;//记录上一次绘制的位置，用于下一次还原
    public InGameBaseCharacter character;
    public bool isupdateInfo = false;
}

public class SmallMap : BaseGameObject
{

    Dictionary<MazeCreate.PointType, Color> mapColors = new Dictionary<MazeCreate.PointType, Color>();

    //小地图每个格子单位的像素尺寸
    const int unitSize = 1;
    //小地图刷新频率
    const float  updateMaxTime = 0.5f;

    float updateTime = 0.45f;

    //每个刷新周期内,数据是否更新
    bool updateData = false;

    //地图尺寸
    int mapSizeX = 0, mapSizeY = 0,textureWidth = 0,textureHeight = 0;

    Texture2D mapTexture;

    //地图位置信息
    MazeCreate.PointType[,] dataArray; // 0 null,1 路 ,2 障碍

    //敌人位置信息
    List<EnemyInfo> enemyInfoList = new List<EnemyInfo>();

    public virtual void Init(int row, int col)
    {
        mapColors.Add(MazeCreate.PointType.non      , new Color(0, 0, 0, 0));
        mapColors.Add(MazeCreate.PointType.wall     , new Color(89 / 255f, 51 / 255f, 44 / 255f, 0.5f));
        mapColors.Add(MazeCreate.PointType.wallfull , new Color(89 / 255f, 51 / 255f, 44 / 255f, 0.5f));
        mapColors.Add(MazeCreate.PointType.nullpoint, new Color(89 / 255f, 51 / 255f, 44 / 255f, 0.5f));
        mapColors.Add(MazeCreate.PointType.way       , new Color(200 / 255f, 180 / 255f, 160 / 255f, 0.5f));
        mapColors.Add(MazeCreate.PointType.fullway   , new Color(200 / 255f, 180 / 255f, 160 / 255f, 0.5f));
        mapColors.Add(MazeCreate.PointType.startpoint, new Color(200 / 255f, 180 / 255f, 160 / 255f, 0.5f));
        mapColors.Add(MazeCreate.PointType.endpoint  , new Color(200 / 255f, 180 / 255f, 160 / 255f, 0.5f));
        mapColors.Add(MazeCreate.PointType.characterpoint, new Color(255 / 255f, 0 / 255f, 0 / 255f, 0.5f));
        mapColors.Add(MazeCreate.PointType.enemypoint    , new Color(255 / 255f, 0 / 255f, 0 / 255f, 0.5f));
        mapColors.Add(MazeCreate.PointType.rolepoint    , new Color(0 / 255f, 255f / 255f, 0 / 255f, 0.5f));


        mapSizeX = row;
        mapSizeY = col;
        dataArray = new MazeCreate.PointType[row, col];
        InitSprite();

        EventManager.Register(this,
                              EventID.EVENT_GAME_CHARACTER_BORN,
                              EventID.EVENT_GAME_CHARACTER_DIE);
        
    }

    public void InitSprite()
    {
        
        textureWidth = mapSizeX * unitSize;
        textureHeight = mapSizeY * unitSize;

        mapTexture = new Texture2D(textureWidth, textureHeight);
        mapTexture.filterMode = FilterMode.Point;
        for (int w = 0; w < textureWidth; w++)
        {
            for (int h = 0; h < textureHeight; h++)
            {
                mapTexture.SetPixel(w, h, mapColors[(int) MazeCreate.PointType.non]);
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

    public bool IsOpenPoint(int x, int y){
        return dataArray[x, y] != MazeCreate.PointType.non;
    }
    /// <summary>
    /// Opens the unit.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="type">Type.</param>
    public void OpenUnit(int x ,int y ,MazeCreate.PointType type){
        
        if(dataArray[x,y] != MazeCreate.PointType.non){
            return;
        }
        //Debug.Log("OpenUnit : " + x + " y:"+ y);
        dataArray[x, y] = type;
        updateData = true;

        int w = x * unitSize;
        int h = y * unitSize;

        SetPixel(w,h,type);
    }


    void ApplySprite()
    {
        //重置全部enemy位置信息
        for (int i = 0; i < enemyInfoList.Count; i++)
        {
            EnemyInfo info = enemyInfoList[i];

            Vector2 mapPos = GameCommon.GetMapPos(info.character.transform.position);

            //如果坐标位置没有改变 ，不做任何操作
            if((int)mapPos.x == info.x && (int)mapPos.y == info.y){
                info.isupdateInfo = false;
            }else{
                //设置更新标记 并 更新位置信息
                SetPixel(info.x, info.y, dataArray[info.x, info.y]);
                info.x = (int)mapPos.x;
                info.y = (int)mapPos.y;
                info.isupdateInfo = true; 
            }
        }

        //重新绘制enemy位置信息
        for (int i = 0; i < enemyInfoList.Count; i++)
        {
            EnemyInfo info = enemyInfoList[i];
            if(info.isupdateInfo){

                if (info.character.GetObjType() == InGameBaseObj.enObjType.character)
                { 
                    SetPixel(info.x, info.y, MazeCreate.PointType.rolepoint);
                }else{
                    SetPixel(info.x, info.y, MazeCreate.PointType.enemypoint);
                }

            }
        }

        mapTexture.Apply();
        updateData = false;
    }

    //根据type绘制某一位置信息
    void SetPixel(int x , int y ,MazeCreate.PointType type){
        if (dataArray[x, y] == MazeCreate.PointType.non)
        {
            return;
        }
        Color c = mapColors[type];

        for (int i = 0; i < unitSize; i++)
        {
            for (int j = 0; j < unitSize; j++)
            {
                mapTexture.SetPixel(x + i, y + j, c);
            }
        }
    }

    public override void HandleEvent(EventData resp)
    {
        base.HandleEvent(resp);
        switch (resp.eid)
        {
            case EventID.EVENT_GAME_CHARACTER_BORN:
                InGameBaseCharacter bornobj = (InGameBaseCharacter)resp.sUserData[0];
                EnemyInfo info = new EnemyInfo();
                info.character = bornobj;
                enemyInfoList.Add(info);
                break;
            case EventID.EVENT_GAME_CHARACTER_DIE:
                InGameBaseCharacter dieobj = (InGameBaseCharacter)resp.sUserData[0];

                for (int i = 0; i < enemyInfoList.Count; i++){
                    if(enemyInfoList[i].character.instanceId == dieobj.instanceId){

                        Vector2 mapPos = GameCommon.GetMapPos(dieobj.transform.position);

                        SetPixel((int)mapPos.x,(int)mapPos.y,dataArray[(int)mapPos.x, (int)mapPos.y]);
                        enemyInfoList.RemoveAt(i);
                        return;
                    }
                }

                break;
        }
    }
}
