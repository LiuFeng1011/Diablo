using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutTest : MonoBehaviour {

    List<GameObject> objList = new List<GameObject>();

	// Use this for initialization
	void Start () {
        for (int i = 0; i < 2; i ++){
            GameObject obj = Resources.Load("Prefabs/Test/Cube") as GameObject;

            obj = MonoBehaviour.Instantiate(obj);

            int randomScale = Random.Range(1, 5);

            obj.transform.position = Vector3.zero;
            obj.transform.localScale = new Vector3(randomScale, randomScale, 1);

            objList.Add(obj);
        }
	}
	
	// Update is called once per frame
	void Update () {

        for (int i = 0; i < objList.Count; i ++){
            GameObject obj = objList[i];
            Rect r = GetObjRect(obj);
            Debug.Log("1:" + r);
            for (int j = 0; j < objList.Count; j ++){
                GameObject nextobj = objList[j];
                Rect nextr = GetObjRect(nextobj);

                Debug.Log("nextr:" + nextr);
                float minx = Mathf.Max(r.x, nextr.x);
                float miny = Mathf.Max(r.y, nextr.y);
                float maxx = Mathf.Min(r.x + r.width, nextr.x + nextr.width);  
                float maxy = Mathf.Min(r.y + r.height, nextr.y + nextr.height);

                if(minx > maxx || miny > maxy){
                    continue;
                }

                float dx = maxx - minx ,dy = maxy - miny;
                Debug.Log(dx + "," + dy);
                if(dx > dy){
                    if(nextobj.transform.position.x > obj.transform.position.x){
                        nextobj.transform.position =new Vector3(
                            obj.transform.position.x + obj.transform.localScale.x / 2 + nextobj.transform.localScale.x / 2,
                            nextobj.transform.position.y, 0);
                    }else{
                        nextobj.transform.position =new Vector3(
                            obj.transform.position.x - obj.transform.localScale.x / 2 - nextobj.transform.localScale.x / 2,
                            nextobj.transform.position.y, 0);
                    }

                }else{
                    if (nextobj.transform.position.y > obj.transform.position.y)
                    {
                        nextobj.transform.position = new Vector3(
                            obj.transform.position.y + obj.transform.localScale.y / 2 + nextobj.transform.localScale.y / 2,
                            nextobj.transform.position.y, 0);
                    }
                    else
                    {
                        nextobj.transform.position = new Vector3(
                            obj.transform.position.x - obj.transform.localScale.x / 2 - nextobj.transform.localScale.x / 2,
                            nextobj.transform.position.y, 0);
                    }
                }
            }
        }

	}

    Rect GetObjRect(GameObject obj){
        Vector2 pos = obj.transform.position;

        Vector2 startPos = new Vector2(pos.x - obj.transform.localScale.x / 2,
                                       pos.y - obj.transform.localScale.y / 2);

        Rect r = new Rect(startPos, obj.transform.localScale);
        return r;
    }
}
