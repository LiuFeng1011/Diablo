using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataStroage : BaseUserData {


    public List<AHInt> stroageList = new List<AHInt>();

    public override UserDataManager.UserDataType GetDataType()
    {
        return UserDataManager.UserDataType.UserDataStroage;
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
                        stroageList.Add(stream.ReadSInt32());
                    }
                    break;
            }
            type = stream.ReadSInt16();
        }
    }

    public override void Serialize(DataStream stream)
    {
        stream.WriteSInt16(0);
        stream.WriteSInt32(stroageList.Count);
        for (int i = 0; i < stroageList.Count; i++)
        {
            stream.WriteSInt32(stroageList[i]);
        }

        stream.WriteSInt16(-1);
    }
}
