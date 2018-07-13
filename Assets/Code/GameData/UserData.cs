using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class UserData : BaseUserData{
    public int instanceID = 1;
    public int chooseRoleId = -1;
    public int packageCount = 30;
    public int stroageCount = 30;

    public override UserDataManager.UserDataType GetDataType()
    {
        return UserDataManager.UserDataType.UserData;
    }

    public override void Deserialize(DataStream stream)
    { 
        short type = stream.ReadSInt16();
        while (type != -1)
        {
            switch (type)
            {
                case 0:
                    instanceID = stream.ReadSInt32();
                    break;
                case 3:
                    chooseRoleId = stream.ReadSInt32();
                    break;
                case 4:
                    packageCount = stream.ReadSInt32();
                    break;
                case 5:
                    stroageCount = stream.ReadSInt32();
                    break;
            }
            type = stream.ReadSInt16();
        }
    }

    public override void Serialize(DataStream stream)
    {
        stream.WriteSInt16(0);
        stream.WriteSInt32(instanceID);

        stream.WriteSInt16(3);
        stream.WriteSInt32(chooseRoleId);

        stream.WriteSInt16(4);
        stream.WriteSInt32(packageCount);

        stream.WriteSInt16(5);
        stream.WriteSInt32(stroageCount);

        stream.WriteSInt16(-1);
        
    }

}
