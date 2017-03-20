using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Prototype.NetworkLobby
{
	public class lobbyPlayerControl : MonoBehaviour {

		public LobbyPlayer lp;

		public void updatePlayerName(string name){
			Debug.Log ("lobby player control called change name");
			gameObject.GetComponent<LobbyPlayer> ().CmdNameChanged (name);
		}
	}
}