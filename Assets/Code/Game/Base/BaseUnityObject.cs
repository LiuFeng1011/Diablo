using UnityEngine;
using System.Collections;

public class BaseUnityObject : MonoBehaviour,EventObserver {

	public virtual void HandleEvent(EventData resp){

	}
}
