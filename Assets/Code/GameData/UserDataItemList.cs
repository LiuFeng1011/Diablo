using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataItemList : BaseUserData {

    public List<GoodsData> itemList = new List<GoodsData>();

    public override UserDataManager.UserDataType GetDataType()
    {
        return UserDataManager.UserDataType.UserDataItemList;
    }

    public override void Deserialize(DataStream stream)
    {
        short type = stream.ReadSInt16();
        while (type != -1)
        {
            switch (type)
            {
                case 0:
                    int count = stream.ReadSInt32();
                    for (int i = 0; i < count; i++)
                    {
                        EquipData e = new EquipData();
                        e.Deserialize(stream);
                        itemList.Add(e);
                    }
                    break;
            }
            type = stream.ReadSInt16();
        }
    }

    public override void Serialize(DataStream stream)
    {
        stream.WriteSInt16(0);
        stream.WriteSInt32(itemList.Count);
        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].Serialize(stream);
        }

        stream.WriteSInt16(-1);
    }
}
