using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipOutAction : MonoBehaviour {
    Vector3 targetPos;
    float actionTime = 0f, actionMaxTime = 0.7f,rotateAngle = 360*1.5f;
    public AnimationCurve positionAnim;
	// Use this for initialization
	void Start () {
		
	}

    public void Init(Vector3 targetPos){
        positionAnim = new AnimationCurve(
            new Keyframe(0f,    0.5f),
            new Keyframe(0.5f,  2f),
            new Keyframe(1f,    0f)
        );

        this.targetPos = targetPos;
    }
	
	// Update is called once per frame
	void Update () {
        actionTime += Time.deltaTime;

        float time = actionTime / actionMaxTime;
        if(time >= 1){
            time = 1;
            Destroy(this);
        }
        transform.position = targetPos + new Vector3(0, positionAnim.Evaluate(time), 0);
        transform.eulerAngles = (new Vector3(0, 0, rotateAngle * (1 - time))) ;

	}
}
