using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewItemData : MonoBehaviour {
    public int index;  
    public object data;  
    public ScrollViewItemData(int index,object data){  
        this.index = index;  
        this.data = data;  
    }  
}
