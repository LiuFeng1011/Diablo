using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLifeLabel : MonoBehaviour {



    public AnimationCurve positionAnim;
    public AnimationCurve scaleAnim;
    public AnimationCurve opacityAnim;

    public float actionTime = 2,baseScale = 1;
    float actionTimeUpdate = 0;

    Vector3 basePos,actionPos;

    public UILabel label;

    public static GameObject labelPrefab;

    public static ChangeLifeLabel CreateChangeLifeLabel(Vector3 worldPos , Color c, string txt){

        if(labelPrefab == null){
            labelPrefab = Resources.Load("Prefabs/UI/ChangeLifeLabel") as GameObject;
        }

        GameObject labelgo = NGUITools.AddChild(InGameManager.GetInstance().inGameUIManager.gameUICanvas, labelPrefab);

        ChangeLifeLabel unit = labelgo.GetComponent<ChangeLifeLabel>();
        unit.basePos = worldPos;

        unit.SetLabel(c,txt);

        return unit;
    }

	// Use this for initialization
	void Start () {
        
        actionPos = new Vector3(-0.2f + Random.Range(0f,0.4f), 0.2f + Random.Range(0f, 0.4f), 0);

        baseScale = transform.localScale.x;

        transform.localScale = Vector3.zero;
	}

    public void SetLabel(Color c,string txt){
        label.color = c;
        label.text = txt;
    }
	
	// Update is called once per frame
	void Update () {
        actionTimeUpdate += Time.deltaTime;

        if(actionTimeUpdate >=actionTime ){
            Destroy(gameObject);
            return;
        }

        float rate = Mathf.Clamp(actionTimeUpdate / actionTime, 0f, 1f);


        Vector3 p = basePos + actionPos * positionAnim.Evaluate(rate);
        transform.position = GameCommon.WorldPosToNGUIPos(
            Camera.main,
            UICamera.currentCamera,
            p);

        transform.localScale = Vector3.one * baseScale * scaleAnim.Evaluate(rate);

        label.color = new Color(label.color.r,label.color.g,label.color.b,opacityAnim.Evaluate(rate));

	}
}
