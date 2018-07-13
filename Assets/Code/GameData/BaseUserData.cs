using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BaseUserData{

    public virtual UserDataManager.UserDataType GetDataType(){
        return UserDataManager.UserDataType.UserData;
    }

    public virtual void Deserialize(DataStream stream)
    { 
        
    }

    public virtual void Serialize(DataStream stream)
    { 
        
    }
}
