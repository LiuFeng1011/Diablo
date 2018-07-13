using UnityEngine;
using System.Collections.Generic;

public class UserDataTest : MonoBehaviour {
	public string uname = "setname";
	// Use this for initialization
	void Start () {
		UserDataManager.instance.Start();
        UserDataManager.instance.DelEquip(1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI()  
	{  
		//GUI.color = Color.red;  

		if(GUI.Button (new Rect (150, 100, 100, 30), "addequip"))  
		{
            //EquipData e = new EquipData(1,1,1,122,new List<EquipProperty>());
            //UserDataManager.instance.AddEquip(e);
		}  

	} 


}
