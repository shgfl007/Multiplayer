using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMenu : MonoBehaviour {
	//this is a local behaviour, menu only shows on local side
	// Use this for initialization

	private static BlockMenu mInstance;
	public static BlockMenu instance{get{ return mInstance; }}

	public GameObject uiOn;

	public GameObject curObject;

	void Start () {
		if (mInstance == null) {
			mInstance = this;
		}	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TurnOn(){
		uiOn.SetActive (true);
	}

	public void TurnOff(){
		uiOn.SetActive (false);
	}

	public void Delete(){
		TurnOff ();
		GameObject ctrl = GameObject.FindGameObjectWithTag ("Player");
		//if(ctrl.isLocalPlayer)
		ctrl.GetComponent<ChangeColour>().DeleteObj(curObject);
		//Destroy (curObject.gameObject);//shoulb be called as rpc
	}

	public void ChangeColour(){
		GameObject ctrl = GameObject.FindGameObjectWithTag ("Player");
		//if(ctrl.isLocalPlayer)
		ctrl.GetComponent<ChangeColour>().CheckIfPainting(curObject);
	}

	public void Rotate(){
		Debug.Log ("rotate button clicked");
		GameObject ctrl = GameObject.FindGameObjectWithTag ("Player");
		//if(ctrl.isLocalPlayer)
		ctrl.GetComponent<ChangeColour>().RotateObj(curObject);
	}
}
