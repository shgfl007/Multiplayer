using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SetupLocalPlayer : NetworkBehaviour {
	[SyncVar]
	public string pname = "player";
	public Text name;

	[Command]
	public void CmdChangeName(string newName){
		pname = newName;
		name.text = pname;
	}

//	void OnGUI(){
//		if (isLocalPlayer) {
//			pname = GUI.TextField (new Rect (25, Screen.height - 40, 100, 30), pname);
//			if (GUI.Button (new Rect (130, Screen.height - 40, 80, 30), "Change")) {
//				CmdChangeName (pname);
//			}
//		}
//	}

	void Update(){
		name.text = pname;
	}
}
