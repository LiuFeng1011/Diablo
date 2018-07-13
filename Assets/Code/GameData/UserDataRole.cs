using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataRole : BaseUserData {

    public List<RoleData> roleList = new List<RoleData>();

    public override UserDataManager.UserDataType GetDataType()
    {
        return UserDataManager.UserDataType.UserDataRole;
    }

    public override void Deserialize(DataStream stream)
    {
        short type = stream.ReadSInt16();
        while (type != -1)
        {
            switch (type)
            {
                case 0:
                    int rolecount = stream.ReadSInt32();
                    for (int i = 0; i < rolecount; i++)
                    {
                        RoleData r = new RoleData();
                        r.Deserialize(stream);
                        roleList.Add(r);
                    }
                break;
            }
            type = stream.ReadSInt16();
        }
    }

    public override void Serialize(DataStream stream)
    {
        stream.WriteSInt16(0);
        stream.WriteSInt32(roleList.Count);
        for (int i = 0; i < roleList.Count; i++)
        {
            roleList[i].Serialize(stream);
        }
        stream.WriteSInt16(-1);
    }
}
