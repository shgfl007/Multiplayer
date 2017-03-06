using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ColourUpdater : NetworkBehaviour {

	[SyncVar]Color MatColour;
	// Use this for initialization
	void Start () {
		//when object gets instantiated, assign a different colour
		gameObject.GetComponent<MeshRenderer>().material.color = Color.grey;
		MatColour = gameObject.GetComponent<MeshRenderer> ().material.color;

		//sync color to server
		CmdUpdateColourToServer(MatColour);

		if (isServer)
		{
			RpcSetColor(MatColour);
		}
	}
		
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			Debug.Log ("change colour at " + this.netId);
			MatColour = Color.red;
			gameObject.GetComponent<MeshRenderer>().material.color = MatColour;

			CmdUpdateColourToServer (MatColour);

			if (isServer)
				RpcSetColor (MatColour);
		}

		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			Debug.Log ("change colour at " + this.netId);
			MatColour = Color.blue;
			gameObject.GetComponent<MeshRenderer>().material.color = MatColour;

			CmdUpdateColourToServer (MatColour);

			if (isServer)
				RpcSetColor (MatColour);
		}

	}

	[Command]
	void CmdUpdateColourToServer(Color c){
		MatColour = c;
		gameObject.GetComponent<MeshRenderer> ().material.color = MatColour;
	}

	[ClientRpc]
	void RpcSetColor( Color c )
	{
		MatColour = c;
		gameObject.GetComponent<MeshRenderer> ().material.color = MatColour;

	}
}
