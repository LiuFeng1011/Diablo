using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class BaseGameMapManager : BaseGameObject {

    Vector3 startPoint = Vector3.zero;

    public virtual void Init()
    {
    }

    public virtual void Update()
    {

    }

    public virtual void Destory()
    {

    }
    public virtual Vector3 GetStartPosition()
    {
        return startPoint;
    }
}
