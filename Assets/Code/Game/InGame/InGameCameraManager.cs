using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCameraManager : InGameBaseManager{
    Camera gameCamera;

    GameObject target = null;

    Vector3 constVector;
	// Use this for initialization
    public override void Init () {
        gameCamera = InGameManager.GetInstance().gamecamera;
        constVector = gameCamera.transform.position;
	}
    public override void Update()
    {
        if (target == null) return;

        gameCamera.transform.position +=
            (target.transform.position + constVector - gameCamera.transform.position) * 0.2f;
    }

    public override void Destory()
    {

    }

    public void SetTarget (GameObject target){
        this.target = target;
    }

}
