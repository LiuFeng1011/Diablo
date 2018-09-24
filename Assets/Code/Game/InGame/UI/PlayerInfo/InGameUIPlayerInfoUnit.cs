using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIPlayerInfoUnit : MonoBehaviour {

    public InGameBaseCharacter role{get ; private set;}

    public UILabel	nameLabel{get ; private set;}
    public UILabel eliteNameLabel { get; private set; }
	public UISprite	lifeSprite{get ; private set;}

	Vector3 basePos;
	void Awake(){
        lifeSprite 	= transform.Find("LifeLine").Find("life").GetComponent<UISprite>();
        nameLabel 	= transform.Find("nameLabel").GetComponent<UILabel>();
        eliteNameLabel = transform.Find("eliteNameLabel").GetComponent<UILabel>();

		basePos = transform.localPosition;
	}

    public void Init(InGameBaseCharacter role){
		this.role = role;
        if(role.GetIsElite()){
            
            eliteNameLabel.gameObject.SetActive(true);
            nameLabel.gameObject.SetActive(false);
        }else{
            eliteNameLabel.gameObject.SetActive(false);
            nameLabel.gameObject.SetActive(true);
        }
        nameLabel.text = role.charactername;
        eliteNameLabel.text = role.charactername;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
    public void InGameUIPlayerInfoUnitUpdate () {
        
		//血条

        float toval = (float)role.life / (float)role.propertys.GetProperty(enCharacterProperty.life);
        float addval = (toval - lifeSprite.fillAmount) * 0.1f;

        lifeSprite.fillAmount = toval + addval;
		//更新位置
		if(Camera.main == null || UICamera.currentCamera == null){
			return;
		}

		transform.position = GameCommon.WorldPosToNGUIPos(
            Camera.main,
            UICamera.currentCamera,
            role.transform.position + new Vector3(0,role.boxSize.y,0)) ;//+ basePos;
		transform.localPosition += basePos;
	}
}
