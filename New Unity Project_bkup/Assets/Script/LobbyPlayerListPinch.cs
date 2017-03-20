using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Prototype.NetworkLobby;

public class LobbyPlayerListPinch : MonoBehaviour {

	public static LobbyPlayerListPinch _instance = null;

	public RectTransform playerListContentTransform;

	protected List<LobbyPlayerPinch> _players = new List<LobbyPlayerPinch> ();
	protected VerticalLayoutGroup _layout;

	public GameObject inviteButton;

	public void OnEnable(){
	
	}
	// Use this for initialization
	void Start () {
		_instance = this;
		_layout = playerListContentTransform.GetComponent<VerticalLayoutGroup>();

	}
	
	// Update is called once per frame
	void Update () {
		//this dirty the layout to force it to recompute evryframe (a sync problem between client/server
		//sometime to child being assigned before layout was enabled/init, leading to broken layouting)

//		if(_layout)
//			_layout.childAlignment = Time.frameCount%2 == 0 ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;
		//Debug.Log("_players has " + _players.Count + " members");
		foreach(LobbyPlayerPinch p in _players){
			if (p.gameObject.transform.parent == null) {
				p.gameObject.transform.SetParent(this.gameObject.transform);
			}
		}
		//hide the invite button if there're 2 players in the room
		if (_players.Count == 2 && inviteButton.activeSelf) {
			Debug.Log ("room is full");
			foreach(LobbyPlayerPinch p in _players){
				p.GetComponent<LobbyPlayerPinch> ().readyToBegin = true;
				Debug.Log (p.playerName + "is ready? " + p.readyToBegin);
			}
			inviteButton.SetActive (false);
			LobbyManagerPinch.s_Singleton.OnLobbyServerPlayersReady ();
		} else if(_players.Count < 2 && !inviteButton.activeSelf) {
			inviteButton.SetActive (true);
		}
	}

	public GameObject getGameObj(){
		return this.gameObject;
	}


	public void AddPlayer(LobbyPlayerPinch player)
	{
		if (_players.Contains(player))
			return;

		_players.Add(player);

		player.transform.SetParent(playerListContentTransform, false);
		//addButtonRow.transform.SetAsLastSibling();

		//PlayerListModified();
	}

	public void RemovePlayer(LobbyPlayerPinch player)
	{
		_players.Remove(player);
		//PlayerListModified();
	}

//	public void PlayerListModified()
//	{
//		int i = 0;
//		foreach (LobbyPlayerPinch p in _players)
//		{
//			p.OnPlayerListChanged(i);
//			++i;
//		}
//	}
}
