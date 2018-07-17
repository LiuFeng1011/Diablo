using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseRoleSceneManager : MonoBehaviour {

    private static ChooseRoleSceneManager instance;

    public static ChooseRoleSceneManager GetInstance(){
        return instance;
    }

    public CreateRoleLayerManager createRoleLayerManager;
    public ChooseRoleLayerManager chooseRoleLayerManager;

    public GameObject chooseRole;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        chooseRoleLayerManager = GameObject.Find("UI Root").transform.Find("ChooseRoleLayer").GetComponent<ChooseRoleLayerManager>();

        createRoleLayerManager = GameObject.Find("UI Root").transform.Find("CreateRoleLayer").GetComponent<CreateRoleLayerManager>();
        createRoleLayerManager.Init();
        ChooseRole();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreateRole(){
        chooseRoleLayerManager.gameObject.SetActive(false);
        createRoleLayerManager.gameObject.SetActive(true);
        createRoleLayerManager.Show();
    }
    public void ChooseRole()
    {
        chooseRoleLayerManager.gameObject.SetActive(true);
        createRoleLayerManager.gameObject.SetActive(false);
        chooseRoleLayerManager.Init();

        ShowPlayerRole(UserDataManager.instance.GetChooseRoleID());
    }

    //配置表id
    public InGameBaseObj ShowBaseRole(int id){
        if(chooseRole != null){
            MonoBehaviour.Destroy(chooseRole);
            chooseRole = null;
        }

        CharacterConf conf = ConfigManager.mapObjectManager.dic[id];
        Debug.Log(conf.prefabName);
        GameObject obj = Resources.Load(conf.prefabName) as GameObject;

        obj = MonoBehaviour.Instantiate(obj);
        obj.transform.position = Vector3.zero;

        InGameBaseObj objscript = obj.GetComponent<InGameBaseObj>();
        chooseRole = obj;
        return objscript;
    }

    //玩家数据instance id
    public InGameBaseObj ShowPlayerRole(int id)
    {
        RoleData rd = UserDataManager.instance.GetRoleData(id);
        if (rd.instanceid == -1){
            if (chooseRole != null)
            {
                MonoBehaviour.Destroy(chooseRole);
                chooseRole = null;
            }
            return null;
        }
        return ShowBaseRole(rd.roldid);
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
