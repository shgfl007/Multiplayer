using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class PinchNetworkLobbyHook : LobbyHook {

	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer){
		LobbyPlayerPinch lobby = lobbyPlayer.GetComponent<LobbyPlayerPinch> ();
		SetupLocalPlayer localPlayer = gamePlayer.GetComponent<SetupLocalPlayer> ();
		localPlayer.pname = lobby.playerName;



	}
}
