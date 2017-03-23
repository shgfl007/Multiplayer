using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using Prototype.NetworkLobby;

public class FBStartButton : MonoBehaviour {
	public LobbyManagerPinch lm;
	public List<MatchInfoSnapshot> matchList;

	public void onClickStartButton(){
		
		matchList = new List<MatchInfoSnapshot> ();
		if (lm == null)
			lm = this.gameObject.GetComponent<LobbyManagerPinch> ();
		lm.mainPanel.SetActive (true);
		lm.fbPanel.SetActive (false);

		if (GameStatesManager.Instance.RoomID != null) {
			initMatchMake ();
			lm.matchMaker.ListMatches (0, 10, "", false, 0, 0, appendList);
		} else {
			//no room id, create a new room
			Debug.Log("start new room");
			lm.StartMatchMaker ();
			lm.matchMaker.CreateMatch(
				((int)Random.Range(0,1000)).ToString(),
				(uint)lm.maxPlayers,
				true,
				"", "", "", 0, 0,
				lm.OnMatchCreate);

			lm.backDelegate = lm.StopHost;
			lm._isMatchmaking = true;
			lm.DisplayIsConnecting ();
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

	void JoinMatch(NetworkID networkID, LobbyManagerPinch lobbyManager)
	{
		lobbyManager.matchMaker.JoinMatch(networkID, "", "", "", 0, 0, lobbyManager.OnMatchJoined);
		lobbyManager.backDelegate = lobbyManager.StopClientClbk;
		lobbyManager._isMatchmaking = true;
		lobbyManager.DisplayIsConnecting();
	}

	public void OnLogoutClick(){
		FBScript fb = GameObject.Find ("FBScriptHolder").GetComponent<FBScript>();
		fb.LogOut ();
	}

	public void OnLoginClick(){
		FBScript fb = GameObject.Find ("FBScriptHolder").GetComponent<FBScript>();
		fb.FBlogin ();
	}
}
