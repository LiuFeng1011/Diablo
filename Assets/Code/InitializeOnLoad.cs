using UnityEngine;
using System.Collections;
using System.Runtime.Hosting;
using UnityEngine.SceneManagement;

public class InitializeOnLoad : MonoBehaviour {

    static string[] jumpSceneList = { "Logo","Menu","Game"};

	[RuntimeInitializeOnLoadMethod]
	static void Initialize()
	{
        for (int i = 0; i < jumpSceneList.Length; i ++){
            if(SceneManager.GetActiveScene().name == jumpSceneList[i]){

                SceneManager.LoadScene("Gate");
                return;
            }
        }
  //      if (SceneManager.GetActiveScene().name == "Gate" || SceneManager.GetActiveScene().name == "TestScene")
		//{
		//	return;
		//}
        //SceneManager.LoadScene("Gate");
	}
}
