using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTest : MonoBehaviour {

	// Use this for initialization
	void Start () {

        //初始化数据存储系统
        UserDataManager.instance.Start();

        ConfigManager.LoadData();

        //Invoke("Test",1f);
	}
    void Test(){
        //List<PropertyConf> propertys = ConfigManager.propertyConfManager.datas;
        //List<EquipProperty> propertyList = new List<EquipProperty>();

        //int propertyCount = Random.Range(1, 5);
        //for (int i = 0; i < propertyCount; i++){
        //    PropertyConf propertyConf = propertys[Random.Range(0, propertys.Count)];
        //    EquipProperty e = new EquipProperty();
        //    e.id = propertyConf.id;
        //    e.val1 = Random.Range(propertyConf.pro1_min, propertyConf.pro1_max);
        //    e.val2 = Random.Range(propertyConf.pro2_min, propertyConf.pro2_max);
        //    e.val3 = Random.Range(propertyConf.pro3_min, propertyConf.pro3_max);
        //    propertyList.Add(e);
        //}

        //List < EquipConf > datas = ConfigManager.equipConfManager.datas;

        //EquipConf equipConf = datas[Random.Range(0, datas.Count)];

        //float baseval = equipConf.base_property_force * 0.2f;
        //EquipData equipData = new EquipData(
        //    UserDataManager.instance.GetInstanceID(),
        //    equipConf.id,
        //    -1,
        //    (int)(equipConf.base_property_force + Random.Range(-baseval, baseval)),
        //    propertyList);
        //UserDataManager.instance.AddEquip(equipData);
        
    }
    void OnGUI()  
    {  
        //GUI.color = Color.red;  

        if(GUI.Button (new Rect (150, 100, 100, 30), "addequip"))  
        {
            Test();
        }  

    } 
	// Update is called once per frame
	void Update () {
		
	}
}
