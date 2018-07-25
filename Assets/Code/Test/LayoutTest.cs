using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutTest : MonoBehaviour {

    public int[,] maparr;// 0 null,1 路 ,2 障碍
    public int rol = 30, col = 20,objcount = 10;
	// Use this for initialization
	void Start () {
        maparr = new int[rol,col];

        for (int i = 0; i < objcount; i ++){
            GameObject obj = Resources.Load("Prefabs/Test/Cube") as GameObject;

            obj = MonoBehaviour.Instantiate(obj);

            int randomScale = Random.Range(3, 8);

            obj.transform.position = Vector3.zero;
            obj.transform.localScale = new Vector3(randomScale, randomScale, 1);

            SetObjPos(obj);
        }

	}

    void SetObjPos(GameObject go){
        List<Vector2> poslist = new List<Vector2>();
        int size = (int)go.transform.localScale.x;
        for (int i = 0; i < rol; i ++){
            for (int j = 0; j < col;j ++){
                Vector2 v = new Vector2(i, j);
                if(haveSpace(v,size)){
                    poslist.Add(v);
                }
            }
        }
        if (poslist.Count <= 0) {
            Destroy(go);
            return;
        }

        Vector2 selpos = poslist[Random.Range(0,poslist.Count)];
        go.transform.position = selpos+new Vector2(size / 2f,size/2f);
        SetSpace(selpos,size);
    }

    bool haveSpace(Vector2 pos,int size){
        int x = (int)pos.x,y = (int)pos.y;

        for (int i = x; i < x+size; i ++){
            if (i >= rol) return false;
            for (int j = y; j < y+size; j++){
                if (j >= col) return false;
                if(maparr[i,j] == 1){
                    return false;
                }
            }
        }
        return true;
    }

    void SetSpace(Vector2 pos, int size){
        int x = (int)pos.x, y = (int)pos.y;
        for (int i = x; i < x + size; i++)
        {
            if (i >= rol) return ;
            for (int j = y; j < y + size; j++)
            {
                if (j >= col) return ;
                maparr[i, j] = 1;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        

	}


}
