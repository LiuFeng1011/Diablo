using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModelLevel : BaseGameModelManager {

    public override void Update()
    {
        for (int i = 0; i < enemyPoints.Count; i++)
        {
            enemyPoints[i].ObjUpdate();
        }
    }
}
