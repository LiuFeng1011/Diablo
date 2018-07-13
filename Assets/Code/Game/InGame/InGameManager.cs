using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InGameManager : BaseUnityObject {
    static InGameManager instance;

    GameTouchController gameTouchController;

    public PlayerCtrlManager playerCtrlManager;
    public InGamePlayerManager inGamePlayerManager;
    public InGameObjManager inGameObjManager;
    public InGameCameraManager inGameCameraManager;
    public InGameLevelManager inGameLevelManager;
    public InGameUIManager inGameUIManager;


    public Camera gamecamera;

    int reviveCount = 0;

    enGameState gameState = enGameState.ready;

    public static float gameTime = 0f;

    public static InGameManager GetInstance(){
        return instance;
    }

    private void Awake()
    {
        instance = this;
        gamecamera = Camera.main;
        gameState = enGameState.ready;

        //StartCoroutine(StartLoading("Map1"));
        GameInit();
    }

    // Use this for initialization
    void GameInit () {
        
        gameTouchController = new GameTouchController();

        inGameUIManager = new InGameUIManager();
        inGameUIManager.Init();

        inGameCameraManager = new InGameCameraManager();
        inGameCameraManager.Init();

        inGameObjManager = new InGameObjManager();
        inGameObjManager.Init();

        playerCtrlManager = new PlayerCtrlManager();
        playerCtrlManager.Init();

        inGameLevelManager = new InGameLevelManager();
        inGameLevelManager.Init();

        inGamePlayerManager = new InGamePlayerManager();
        inGamePlayerManager.Init();

        gameState = enGameState.playing;
    }

	// Update is called once per frame
	void Update () {
        gameTime += Time.deltaTime;
        if(gameState != enGameState.playing){
            return;
        }

        if (gameTouchController != null) gameTouchController.Update();

        inGameObjManager.Update();
        playerCtrlManager.Update();
        inGamePlayerManager.Update();
        inGameCameraManager.Update();
        inGameLevelManager.Update();
        inGameUIManager.Update();
	}

    private void OnDestroy()
    {
        if (gameState <= enGameState.ready)
        {
            return;
        }
        playerCtrlManager.Destory();
        inGamePlayerManager.Destory();
        inGameObjManager.Destory();
        inGameCameraManager.Destory();
        inGameLevelManager.Destory();
        inGameUIManager.Destory();
        instance = null;
        EventManager.Remove(this);
    }

    public void GameOver(){
        gameState = enGameState.over;

        Invoke("ShowOverLayer", 1.0f);
    }

    public enGameState GetGameState(){
        return gameState;
    }

    public void ShowOverLayer(){

        gameState = enGameState.over;

        reviveCount += 1;
    }

    public void Pause(){
        gameState = enGameState.pause;
    }

    public void Resume(){
        gameState = enGameState.playing;
    }

    public void Revive(){

        gameState = enGameState.playing;

    }

    public void Restart(){
        reviveCount = 0;

    }

    private IEnumerator StartLoading(string mapname)
    {
        AsyncOperation op = 
            SceneManager.LoadSceneAsync(mapname, LoadSceneMode.Additive);

        //allowSceneActivation 来控制异步加载的场景暂时不进入。播放完动画后通过触发
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }
        while (!op.allowSceneActivation)
        {

            op.allowSceneActivation = true;
            yield return new WaitForEndOfFrame();
        }

        GameInit();
    }

}
