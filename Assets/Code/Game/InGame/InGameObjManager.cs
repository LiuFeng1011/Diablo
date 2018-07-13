using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameObjManager : InGameBaseManager {

    int gameObjInstanceId = 0;


    public List<InGameBaseObj> objList = new List<InGameBaseObj>();
    public List<InGameBaseObj> addList = new List<InGameBaseObj>();
    public List<InGameBaseObj> delList = new List<InGameBaseObj>();

    public override void Init()
    {
        base.Init();
    }
	
    public override void Update()
    {
        base.Update();

        for (int i = 0; i < objList.Count; i++)
        {
            InGameBaseObj obj = objList[i];

            obj.ObjUpdate();

            if (obj.IsDie())
            {
                delList.Add(obj);
            }
        }

        for (int i = 0; i < delList.Count; i++)
        {
            InGameBaseObj obj = delList[i];
            objList.Remove(obj);
            obj.Die();
        }
        delList.Clear();

        if (addList.Count > 0)
        {
            objList.AddRange(addList);
            addList.Clear();
        }

    }


    public InGameBaseObj AddObj(int objid,enMSCamp camp)
    {
        gameObjInstanceId++;

        MapObjConf conf = ConfigManager.mapObjectManager.dic[objid];
        GameObject obj = Resources.Load(conf.prefabName) as GameObject;

        obj = MonoBehaviour.Instantiate(obj);

        InGameBaseObj objscript = obj.GetComponent<InGameBaseObj>();
        objscript.Init(gameObjInstanceId,objid,camp);
        addList.Add(objscript);
        return objscript;
    }


    public InGameBaseObj AddSkill(int skill,enMSCamp camp)
    {
        gameObjInstanceId++;
        SkillConf conf = ConfigManager.skillConfManager.dataMap[skill];
        GameObject obj = Resources.Load(conf.filepath) as GameObject;

        obj = MonoBehaviour.Instantiate(obj);

        InGameBaseObj objscript = obj.GetComponent<InGameBaseObj>();
        objscript.Init(gameObjInstanceId, skill,camp);
        addList.Add(objscript);
        return objscript;
    }
}
