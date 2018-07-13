using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {
    static MenuManager instance;
    public static MenuManager GetInstance() { return instance; }

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        GameObject startBtn = GameObject.Find("UI Root").transform.Find("StartGame").gameObject;
        GameUIEventListener.Get(startBtn).onClick = StartCB;
        GameObject chooseRoleBtn = GameObject.Find("UI Root").transform.Find("ChooseRoleBtn").gameObject;
        GameUIEventListener.Get(chooseRoleBtn).onClick = ChooseRoleCB;

        GameObject clearDataBtn = GameObject.Find("UI Root").transform.Find("ClearDataBtn").gameObject;
        GameUIEventListener.Get(clearDataBtn).onClick = ClearDataCB;

        GameObject equipobj = (GameObject)Resources.Load("Prefabs/items/item_icon");
        equipobj = MonoBehaviour.Instantiate(equipobj);

        InGameBaseEquip equip = equipobj.GetComponent<InGameBaseEquip>();
        equip.Init(-1, 200001, enMSCamp.en_camp_item);
        equip.level = 1;
    }

	// Update is called once per frame
	void Update () {
       
	}

    void StartCB(GameObject go)
    {
        (new EventChangeScene(GameSceneManager.SceneTag.Game)).Send();
    }

    void ChooseRoleCB(GameObject go)
    {
        (new EventChangeScene(GameSceneManager.SceneTag.ChooseRole)).Send();
    }
    void ClearDataCB(GameObject go)
    {
        UserDataManager.instance.ClearData();
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
