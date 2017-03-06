using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class callMenu : NetworkBehaviour {
	public GameObject UI;
	GameObject tempUI;
	// Use this for initialization
	void Start () {
		tempUI = GameObject.FindGameObjectWithTag(UI.tag);
		if (tempUI != null) {
			tempUI.SetActive (false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//check to see if the user click on an object
		//do this only if there's a local player
		tempUI = GameObject.Find ("CanvasHolder");
		if(isLocalPlayer){
			//closeMenu ();
			checkIfClick ();

		}

	}

	void checkIfClick(){
		

		//Debug.Log ("ui name is " + tempUI.name);
		RaycastHit hit;
		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 525f)) {
			if (Input.GetMouseButtonDown (0)) {
				Debug.Log ("Raycast hit " + hit.transform.gameObject.GetComponent<NetworkIdentity> ().netId);
				//check if there's already a obj menu in scene
				//tempUI = GameObject.Find (UI.name);
				if (tempUI == null)
					tempUI = Instantiate (UI, hit.transform) as GameObject;
				else {
					tempUI.transform.SetParent (hit.transform);
				}

				tempUI.transform.localPosition = Vector3.zero;
				tempUI.SetActive (true);
			}
		} else {
			if (Input.GetMouseButtonUp (0)) {
				BlockMenu.instance.TurnOff();
			}
		} 

	}

	void closeMenu(){
		if(tempUI != null){
			//if (Input.GetMouseButtonDown (0) && tempUI.GetComponent<BlockMenu>().uiOn.activeSelf) {
				BlockMenu.instance.TurnOff();
			//}
			
		}
		
	}
}
