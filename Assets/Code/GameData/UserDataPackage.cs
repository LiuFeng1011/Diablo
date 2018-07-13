using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataPackage : BaseUserData {

    public List<AHInt> packageList = new List<AHInt>();


    public override UserDataManager.UserDataType GetDataType()
    {
        return UserDataManager.UserDataType.UserDataPackage;
    }

    public override void Deserialize(DataStream stream)
    { 
        short type = stream.ReadSInt16();
        while (type != -1)
        {
            switch (type)
            {
                case 0:
                    int packagecount = stream.ReadSInt32();
                    for (int i = 0; i < packagecount; i++)
                    {
                        packageList.Add(stream.ReadSInt32());
                    }
                    break;
            }
            type = stream.ReadSInt16();
        }
    }

    public override void Serialize(DataStream stream)
    { 

        stream.WriteSInt16(0);
        stream.WriteSInt32(packageList.Count);
        for (int i = 0; i < packageList.Count; i++)
        {
            stream.WriteSInt32(packageList[i]);
        }

        stream.WriteSInt16(-1);
    }
}
