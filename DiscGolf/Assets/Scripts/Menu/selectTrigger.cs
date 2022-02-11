using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class selectTrigger : MonoBehaviour, ISelectHandler
{

    public GameObject startButtonGameObject;
    private nextScene sceneClass;
    // Start is called before the first frame update
    void Start()
    {
        sceneClass = startButtonGameObject.GetComponent<nextScene>();
    }

    public void OnSelect (BaseEventData eventData) 
	{
        sceneClass.setSelected(this.gameObject.name);
		//Debug.Log (this.gameObject.name + " was selected");
	}

}
