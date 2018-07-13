using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewItem : MonoBehaviour {

    public ScrollViewItemData data;  
    // Use this for initialization  
    void Awake () {  
    }  
  
    public virtual void SetData(ScrollViewItemData data){  
        this.data = data;  
    }  
  
    // Update is called once per frame  
    void Update () {  
          
    }  
}
