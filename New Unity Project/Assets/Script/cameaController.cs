using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class cameaController : NetworkBehaviour {

	[SyncVar]Quaternion syncCameraRotation;
	[SyncVar]Quaternion syncPlayerRotation;

	public Transform playerTransform;
	public Transform camTransform;
	public float lerpRate = 15;
	public GameObject refObj;
	// Update is called once per frame
	void Update () {

		TransmitRotation ();
		LerpRotations ();
		
	}


	void LerpRotations(){
		if (!isLocalPlayer) {
			//Debug.Log ("lerp rotation called");
			//Debug.Log ("synced rotation is " + syncCameraRotation);
			playerTransform.rotation = Quaternion.Lerp (playerTransform.rotation, syncPlayerRotation, Time.deltaTime * lerpRate);
			//camTransform.rotation = Quaternion.Lerp (camTransform.rotation, syncCameraRotation, Time.deltaTime * lerpRate);
			camTransform.rotation = syncCameraRotation;
			refObj.transform.rotation = syncCameraRotation;
		}
	}

	[Command]
	void CmdProvideRotationsToServer(Quaternion playerRot, Quaternion camRot){
		syncPlayerRotation = playerRot;
		syncCameraRotation = camRot;

		camTransform.rotation = syncCameraRotation;
	}

	[ClientCallback]
	void TransmitRotation(){
		if (isLocalPlayer) {
			CmdProvideRotationsToServer (playerTransform.rotation, camTransform.rotation);
			refObj.SetActive (false);
			//refObj.GetComponent<MeshRenderer> ().enabled = false;
			//refObj.transform.rotation = camTransform.rotation;
		}
	}
}
