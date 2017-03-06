using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChangeColour : NetworkBehaviour {

	private int range = 200;
	[SerializeField] private Transform camTransform;
	private RaycastHit hit;
	[SyncVar] private Color objectColor;
	[SyncVar] private GameObject objectID;
	private NetworkIdentity objNetId;
	[SyncVar] private bool showHighLighter;

	void Update () {
//		if (isLocalPlayer) {
//			Debug.Log ("change color script");
//			CheckIfPainting ();
//		}
	}

	public void CheckIfPainting(GameObject obj){
		if(isLocalPlayer) {
			Debug.Log ("change colour");
			//if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 525f)) {
				//Debug.Log ("Raycast hit " + hit.transform.gameObject.GetComponent<NetworkIdentity>().netId);
				//objectID = hit.transform.gameObject;                                    // this gets the object that is hit
			objectID = obj;	
			objectColor = new Color(Random.value, Random.value, Random.value, Random.value);    // I select the color here before doing anything else
			CmdPaint(objectID, objectColor);
			//}
		}
	}
		
	[ClientRpc]
	void RpcPaint(GameObject obj, Color col){
		//obj.GetComponent<Renderer>().material.color = col;        // this is the line that actually makes the change in color happen
		obj.GetComponent<BlockControl>().changeHighlightColour(col);
		obj.GetComponent<BlockControl> ().ChangeColour (col);
	}

	[Command]
	void CmdPaint(GameObject obj, Color col) {
		objNetId = obj.GetComponent<NetworkIdentity> ();        // get the object's network ID
		objNetId.AssignClientAuthority (connectionToClient);    // assign authority to the player who is changing the color
		RpcPaint (obj, col);                                    // usse a Client RPC function to "paint" the object on all clients
		objNetId.RemoveClientAuthority (connectionToClient);    // remove the authority from the player who changed the color
	}

	public void showHighLight(GameObject obj){
		if (isLocalPlayer) {
			objectID = obj;
			showHighLighter = true;
			CmdHighlight (objectID, showHighLighter);
		}
	}

	public void hideHighLight(GameObject obj){
		if (isLocalPlayer) {
			objectID = obj;
			showHighLighter = false;
			CmdHighlight (objectID, showHighLighter);
		}
	}

	[ClientRpc]
	void RpcHighLight(GameObject obj, bool show){
		//obj.transform.GetChild (0).gameObject.SetActive (show);        // this is the line that actually makes the change in color happen
		obj.GetComponent<BlockControl>().controlHighlighter(show);
		//obj.GetComponent<BlockControl>().changeHighlightColour
	}

	[Command]
	void CmdHighlight(GameObject obj, bool show) {
		objNetId = obj.GetComponent<NetworkIdentity> ();        // get the object's network ID
		objNetId.AssignClientAuthority (connectionToClient);    // assign authority to the player who is changing the color
		RpcHighLight(obj, show);                                    // usse a Client RPC function to "paint" the object on all clients
		objNetId.RemoveClientAuthority (connectionToClient);    // remove the authority from the player who changed the color
	}

	public void DeleteObj(GameObject obj){
		if (isLocalPlayer) {
			objectID = obj;
			CmdDelete (objectID);
		}
	}

	[ClientRpc]
	void RpcDelete(GameObject obj){
		//obj.transform.GetChild (0).gameObject.SetActive (show);        // this is the line that actually makes the change in color happen
		obj.GetComponent<BlockControl>().DestroyObject();
		//obj.GetComponent<BlockControl>().changeHighlightColour
	}

	[Command]
	void CmdDelete(GameObject obj) {
		objNetId = obj.GetComponent<NetworkIdentity> ();        // get the object's network ID
		objNetId.AssignClientAuthority (connectionToClient);    // assign authority to the player who is changing the color
		RpcDelete(obj);                                    // usse a Client RPC function to "paint" the object on all clients
		objNetId.RemoveClientAuthority (connectionToClient);    // remove the authority from the player who changed the color
	}

	public void RotateObj(GameObject obj){
		if (isLocalPlayer) {
			objectID = obj;
			CmdRotate (objectID);
		}
	}

	[ClientRpc]
	void RpcRotate(GameObject obj){
		//obj.transform.GetChild (0).gameObject.SetActive (show);        // this is the line that actually makes the change in color happen
		obj.GetComponent<BlockControl>().RotateObject();
		//obj.GetComponent<BlockControl>().changeHighlightColour
	}

	[Command]
	void CmdRotate(GameObject obj) {
		objNetId = obj.GetComponent<NetworkIdentity> ();        // get the object's network ID
		objNetId.AssignClientAuthority (connectionToClient);    // assign authority to the player who is changing the color
		RpcRotate(obj);                                    // usse a Client RPC function to "paint" the object on all clients
		objNetId.RemoveClientAuthority (connectionToClient);    // remove the authority from the player who changed the color
	}
}
