using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine;


namespace Prototype.NetworkLobby
{
	public class AcceptInviteEnterLobbyy : MonoBehaviour {

		public LobbyManager lm;
		public List<MatchInfoSnapshot> matchList;
		void Awake(){
			matchList = new List<MatchInfoSnapshot> ();
			lm = GameObject.Find ("LobbyManager").GetComponent<LobbyManager> ();
			if (GameStatesManager.Instance.RoomID != null) {
				initMatchMake ();
				//List<MatchInfoSnapshot> list = lm.matches;
				//Debug.Log (list.Count);
				lm.matchMaker.ListMatches (0, 6, "", false, 0, 0, appendList);
				//lm.matchMaker.ListMatches(0, 20, "", true, 0, 0, appendList);
//				for (int i = 0; i < list.Count; i++) {
//					if(list[i].networkId.ToString() == GameStatesManager.Instance.RoomID){
//						Debug.Log ("found room");
//						AcceptAndEnter (list [i].networkId);
//					}
//				}
			}
		}

		public void appendList(bool success, string extendedInfo, List<MatchInfoSnapshot> sublist){
		
			//append the list here
			//matchList.AddRange(sublist);

			for (int i = 0; i < sublist.Count; i++) {
				if (sublist [i].networkId.ToString () == GameStatesManager.Instance.RoomID) {
					Debug.Log ("Found room");
					AcceptAndEnter (sublist [i].networkId);
				}

			}
		}


		public void AcceptAndEnter(NetworkID networkid){
			
			JoinMatch (networkid, lm);
		}

		void initMatchMake(){
			lm.StartMatchMaker ();
			lm.backDelegate = lm.SimpleBackClbk;
		}

		void JoinMatch(NetworkID networkID, LobbyManager lobbyManager)
		{
			lobbyManager.matchMaker.JoinMatch(networkID, "", "", "", 0, 0, lobbyManager.OnMatchJoined);
			lobbyManager.backDelegate = lobbyManager.StopClientClbk;
			lobbyManager._isMatchmaking = true;
			lobbyManager.DisplayIsConnecting();
		}

		public void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo){
			Debug.Log ("success is " + success);
			Debug.Log ("extendedInfo is " + extendedInfo);
			Debug.Log ("match info access token is " + matchInfo.accessToken);
		}
	}
}
