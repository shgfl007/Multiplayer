using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using Prototype.NetworkLobby;


public class LobbyManagerPinch : NetworkLobbyManager {

	static short MsgKicked = MsgType.Highest + 1;

	static public LobbyManagerPinch s_Singleton;

	[Header("Pinch UI Lobby")]
	[Tooltip("Time in second between all players ready & match start")]
	public float prematchCountdown = 5.0f;

	[Space]
	[Header("UI Reference")]

	//public RectTransform mainMenuPanel;
	//public RectTransform lobbyPanel;
	public LobbyTopPanel topPanel;
	public LobbyInfoPanel infoPanel;
	public LobbyCountdownPanel countdownPanel;
	public GameObject mainPanel;
	//public GameObject addPlayerButton;

	//protected RectTransform currentPanel;


	//public Text statusInfo;
	//public Text hostInfo;
	public Button startButton;
	//added by Danning to send to facebook
	public string RoomID;
	public string ServerIPAddress;
	//Client numPlayers from NetworkManager is always 0, so we count (throught connect/destroy in LobbyPlayer) the number
	//of players, so that even client know how many player there is.
	[HideInInspector]
	public int _playerNumber = 0;

	//used to disconnect a client properly when exiting the matchmaker
	[HideInInspector]
	public bool _isMatchmaking = false;

	protected bool _disconnectServer = false;

	protected ulong _currentMatchID;

	protected LobbyHook _lobbyHooks;

	bool isHost = false;

	// Use this for initialization
	void Start () {
		s_Singleton = this;
		_lobbyHooks = GetComponent<Prototype.NetworkLobby.LobbyHook>();
		//currentPanel = mainMenuPanel;
		Debug.Log("lobby hooks is " + _lobbyHooks.name);
		GetComponent<Canvas>().enabled = true;
		startButton.interactable = false;
		DontDestroyOnLoad(gameObject);
		
	}
	void Update(){
		if ((SceneManager.GetActiveScene ().buildIndex == 1) && !mainPanel.activeSelf) {
			Debug.Log ("!!!!!!!!!");
			mainPanel.SetActive (true);
		}
	}

	public override void OnLobbyClientSceneChanged(NetworkConnection conn)
	{
		if (SceneManager.GetSceneAt(0).name == lobbyScene)
		{	//mainPanel.SetActive(true);
			if (topPanel.isInGame)
			{
				//ChangeTo(lobbyPanel);
				mainPanel.SetActive(true);
				if (_isMatchmaking)
				{
					if (conn.playerControllers[0].unetView.isServer)
					{
						backDelegate = StopHostClbk;
					}
					else
					{
						backDelegate = StopClientClbk;
					}
				}
				else
				{
					if (conn.playerControllers[0].unetView.isClient)
					{
						backDelegate = StopHostClbk;
					}
					else
					{
						backDelegate = StopClientClbk;
					}
				}
			}
			else
			{
				//ChangeTo(mainMenuPanel);
				//go back to scene 0
				Debug.Log("go back to scene 1");
				SceneManager.LoadScene(0);
			}

			topPanel.ToggleVisibility(true);
			topPanel.isInGame = false;
		}
		else
		{
			//ChangeTo(null);
			//enter game scene, hide the main panel
			mainPanel.SetActive(false);
			Destroy(GameObject.Find("MainMenuUI(Clone)"));

			//backDelegate = StopGameClbk;
			topPanel.isInGame = true;
			topPanel.ToggleVisibility(false);
		}
	}

	public void DisplayIsConnecting()
	{
		var _this = this;
		infoPanel.Display("Connecting...", "Cancel", () => { _this.backDelegate(); });
	}

	public void SetServerInfo(string status, string host)
	{
//		statusInfo.text = status;
//		hostInfo.text = host;
	}


	public delegate void BackButtonDelegate();
	public BackButtonDelegate backDelegate;
	public void GoBackButton()
	{
		backDelegate();
		topPanel.isInGame = false;
		mainPanel.SetActive (false);
		GameStatesManager.Instance.RoomID = null;
		SceneManager.LoadScene (0);SceneManager.LoadScene (0);
	}



	// ----------------- Server management

	public void AddLocalPlayer()
	{
		TryToAddPlayer();
	}

	public void RemovePlayer(LobbyPlayerPinch player)
	{
		player.RemovePlayer();
	}

	public void SimpleBackClbk()
	{
		//ChangeTo(mainMenuPanel);
	}

	public void StopHostClbk()
	{
		if (_isMatchmaking)
		{
			matchMaker.DestroyMatch((NetworkID)_currentMatchID, 0, OnDestroyMatch);
			_disconnectServer = true;
		}
		else
		{
			StopHost();
		}


		//ChangeTo(mainMenuPanel);
	}

	public void StopClientClbk()
	{
		StopClient();

		if (_isMatchmaking)
		{
			StopMatchMaker();
		}

		//ChangeTo(mainMenuPanel);
	}

	public void StopServerClbk()
	{
		StopServer();
		//ChangeTo(mainMenuPanel);
	}

	class KickMsg : MessageBase { }
	public void KickPlayer(NetworkConnection conn)
	{
		conn.Send(MsgKicked, new KickMsg());
	}




	public void KickedMessageHandler(NetworkMessage netMsg)
	{
		infoPanel.Display("Kicked by Server", "Close", null);
		netMsg.conn.Disconnect();
	}

	//===================

	public override void OnStartHost()
	{
		base.OnStartHost();

		//ChangeTo(lobbyPanel);
		backDelegate = StopHostClbk;
		SetServerInfo("Hosting", networkAddress);
	}

	public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
	{
		base.OnMatchCreate(success, extendedInfo, matchInfo);
		_currentMatchID = (System.UInt64)matchInfo.networkId;

		//Added by Danning for testing purpose
		Debug.Log("network ID is " + matchInfo.networkId);
		RoomID = matchInfo.networkId +"";
		ServerIPAddress = Network.player.ipAddress;
		Debug.Log ("network address is " + ServerIPAddress);
		GameStatesManager.Instance.RoomID = RoomID;
		GameStatesManager.Instance.ServerIP = Network.player.ipAddress;
		isHost = true;
		//once the match is created, store the network id and ip address 
	}

	public override void OnDestroyMatch(bool success, string extendedInfo)
	{
		base.OnDestroyMatch(success, extendedInfo);
		if (_disconnectServer)
		{
			StopMatchMaker();
			StopHost();
		}
	}

	// ----------------- Server callbacks ------------------

	//we want to disable the button JOIN if we don't have enough player
	//But OnLobbyClientConnect isn't called on hosting player. So we override the lobbyPlayer creation
	public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
	{
		GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

		LobbyPlayerPinch newPlayer = obj.GetComponent<LobbyPlayerPinch>();
		//newPlayer.ToggleJoinButton(numPlayers + 1 >= minPlayers);


//		for (int i = 0; i < lobbySlots.Length; ++i)
//		{
//			LobbyPlayer p = lobbySlots[i] as LobbyPlayer;
//
//			if (p != null)
//			{
//				p.RpcUpdateRemoveButton();
//				p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
//			}
//		}

		return obj;
	}

	public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
	{
		for (int i = 0; i < lobbySlots.Length; ++i)
		{
			LobbyPlayerPinch p = lobbySlots[i] as LobbyPlayerPinch;

//			if (p != null)
//			{
//				p.RpcUpdateRemoveButton();
//				p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
//			}
		}
	}

	public override void OnLobbyServerDisconnect(NetworkConnection conn)
	{
		for (int i = 0; i < lobbySlots.Length; ++i)
		{
			LobbyPlayerPinch p = lobbySlots[i] as LobbyPlayerPinch;

//			if (p != null)
//			{
//				p.RpcUpdateRemoveButton();
//				p.ToggleJoinButton(numPlayers >= minPlayers);
//			}
		}

	}

	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
	{
		//This hook allows you to apply state data from the lobby-player to the game-player
		//just subclass "LobbyHook" and add it to the lobby object.

		if (_lobbyHooks)
			_lobbyHooks.OnLobbyServerSceneLoadedForPlayer(this, lobbyPlayer, gamePlayer);

		return true;
	}

	// --- Countdown management

	public override void OnLobbyServerPlayersReady()
	{
		Debug.Log ("ready");
		bool allready = true;
		for(int i = 0; i < lobbySlots.Length; ++i)
		{
			if(lobbySlots[i] != null)
				allready &= lobbySlots[i].readyToBegin;
		}

		if(allready){
			//ServerChangeScene(playScene);//StartCoroutine(ServerCountdownCoroutine());
			if(isHost){
				//enable start button
				startButton.interactable = true;
			}
		}
	}

	//press start to enter game
	public void EnterGame(){
		StartCoroutine ("ServerCountdownCoroutine");
	}

	public IEnumerator ServerCountdownCoroutine()
	{
		float remainingTime = prematchCountdown;
		int floorTime = Mathf.FloorToInt(remainingTime);

		while (remainingTime > 0)
		{
			yield return null;

			remainingTime -= Time.deltaTime;
			int newFloorTime = Mathf.FloorToInt(remainingTime);

			if (newFloorTime != floorTime)
			{//to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
				floorTime = newFloorTime;

				for (int i = 0; i < lobbySlots.Length; ++i)
				{
					if (lobbySlots[i] != null)
					{//there is maxPlayer slots, so some could be == null, need to test it before accessing!
						(lobbySlots[i] as LobbyPlayerPinch).RpcUpdateCountdown(floorTime);
					}
				}
			}
		}

		for (int i = 0; i < lobbySlots.Length; ++i)
		{
			if (lobbySlots[i] != null)
			{
				(lobbySlots[i] as LobbyPlayerPinch).RpcUpdateCountdown(0);
			}
		}

		ServerChangeScene(playScene);
	}

	// ----------------- Client callbacks ------------------

	public override void OnClientConnect(NetworkConnection conn)
	{
		base.OnClientConnect(conn);

		infoPanel.gameObject.SetActive(false);

		conn.RegisterHandler(MsgKicked, KickedMessageHandler);

		if (!NetworkServer.active)
		{//only to do on pure client (not self hosting client)
			//ChangeTo(lobbyPanel);
			backDelegate = StopClientClbk;
			SetServerInfo("Client", networkAddress);
		}
	}


	public override void OnClientDisconnect(NetworkConnection conn)
	{
		base.OnClientDisconnect(conn);
		//ChangeTo(mainMenuPanel);
	}

	public override void OnClientError(NetworkConnection conn, int errorCode)
	{
		//ChangeTo(mainMenuPanel);
		infoPanel.Display("Cient error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
	}

	public void OnPlayersNumberModified(int count)
	{
		_playerNumber += count;

		int localPlayerCount = 0;
		foreach (PlayerController p in ClientScene.localPlayers)
			localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

		//addPlayerButton.SetActive(localPlayerCount < maxPlayersPerConnection && _playerNumber < maxPlayers);
	}
}
