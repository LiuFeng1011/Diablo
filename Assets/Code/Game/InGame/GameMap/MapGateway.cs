using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGateway : InGameBaseMapObj {

    public int targetid = -1;

    public override int ActionDistance()
    {
        return 0;
    }

    public override bool HandleFuntion(InGameBaseObj obj)
    {
        Debug.Log("MapGateway HandleFuntion :targetid : " + targetid);
        GameGlobal.selLevelConfId = targetid;
        (new EventChangeScene(GameSceneManager.SceneTag.Game)).Send();
        return false;
    }

    public override void Serialize(DataStream writer)
    {
        writer.WriteSInt32(1);
        writer.WriteSInt32(targetid);

        writer.WriteSInt32(-1);
    }

    public override void Deserialize(DataStream reader)
    {

        int dataid = reader.ReadSInt32();

        while (dataid != -1)
        {
            switch (dataid)
            {
                case 1: targetid = reader.ReadSInt32(); break;

            }
            dataid = reader.ReadSInt32();
        }
    }
}
