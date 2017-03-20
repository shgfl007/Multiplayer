using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NameBillboard : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
			Camera mainCam = Camera.main;
			transform.LookAt (transform.position + mainCam.transform.rotation * Vector3.forward, 
				mainCam.transform.rotation * Vector3.up);
		}		
	}
}
