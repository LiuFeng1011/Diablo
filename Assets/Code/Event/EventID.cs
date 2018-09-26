using UnityEngine;
using System.Collections;

public enum EventID{
    EVENT_ENTRYGAME = 10001,//登录游戏

    //*************************系统消息******************************
    //==========CONFIG==========
    EVENT_CONFIG_LOADFINISHED = 0x15001,//配置表加载完成
    //==========CHANG SCENE==========
    EVENT_SCENE_CHANGE = 0x16001,//切换场景
    EVENT_SCENE_LOADING_FINISHED = 0x16002,//loading场景加载完成

    //==========TOUCH==========
    EVENT_TOUCH_DOWN = 0x17001,//按下
    EVENT_TOUCH_UP = 0x17002,//抬起
    EVENT_TOUCH_MOVE = 0x17003,//移动
    EVENT_TOUCH_SWEEP = 0x17004,//划动
    //*******************************************************

    EVENT_GAME_TOUCH_GROUND = 0x20001,//点击地面
    EVENT_GAME_TOUCH_OBJ = 0x20002,//点击对象

    EVENT_GAME_CHARACTER_BORN = 0x20003,// 角色出生
    EVENT_GAME_CHARACTER_DIE = 0x20004,// 角色死亡
    EVENT_GAME_INITROLE = 0x20006,// 初始化角色

    EVENT_GAME_CHARACTER_HURT = 0x20008,// 角色受伤

    EVENT_GAME_ROLR_LEVELUP = 0x20009,// 角色升级

    EVENT_ADD_ENEMYPOINT = 0x20100,//添加刷怪点

    EVENT_DATA_USELEVELUPPOINT = 0x21001,//使用升级点
    EVENT_DATA_KILLENEMY = 0x21002,//杀死敌人
    EVENT_DATA_CHANGELIFE = 0x21003,//生命值变化
    EVENT_DATA_CHANGEMANA = 0x21004,//魔法值变化
    EVENT_DATA_CHANGEEXP = 0x21005,//魔法值变化

    EVENT_DATA_REFRESHPROPERTY = 0x210FF,//刷新角色属性


    EVENT_UI_OPENPACKAGE = 0x30001,//打开背包
    EVENT_UI_DELEQUIP = 0x30002,//删除装备
    //EVENT_UI_ADDEQUIP = 0x30003,//添加装备
    EVENT_UI_EQUIP = 0x30004,//使用装备
    EVENT_UI_PACKAGE_SELECT_ITEM = 0x30005,//背包列表选择装备
    EVENT_UI_UNEQUIP = 0x30006,//卸下装备
    EVENT_UI_REFRESH_PACKAGEUI = 0x30007,//刷新背包UI界面

    EVENT_UI_OPENBIGMAP = 0x30008,//打开大地图
    EVENT_UI_INITMAPTEXTURE = 0x30009,//初始化地图贴图



}

