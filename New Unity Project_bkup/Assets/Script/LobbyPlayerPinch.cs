using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
using Facebook.Unity;
using Prototype.NetworkLobby;

public class LobbyPlayerPinch : NetworkLobbyPlayer {

	//OnMyName function will be invoked on clients when server change the value of playerName
	public Text pName;
	public Image pProfile;

	[SyncVar(hook = "OnMyName")]
	public string playerName = "";
	[SyncVar(hook = "OnMyFBID")]
	public string playerFBid = "";


	public override void OnClientEnterLobby()
	{
		base.OnClientEnterLobby();

		if (LobbyManagerPinch.s_Singleton != null) LobbyManagerPinch.s_Singleton.OnPlayersNumberModified(1);

		LobbyPlayerListPinch._instance.AddPlayer(this);
		//Invoke("addPlayer", 0.3f);
		//LobbyPlayerListPinch._instance.DisplayDirectServerWarning(isServer && LobbyManagerPinch.s_Singleton.matchMaker == null);

		if (isLocalPlayer)
		{
			if (FB.IsLoggedIn) {
				FacebookManager.Instance.GetProfile ();
				if (FacebookManager.Instance.profileName != null) {
					SetupLocalPlayer();
				} else {
					StartCoroutine ("WaitForProfileNameLobby");
				}
			}else
				SetupLocalPlayer();

		}
		else
		{
			SetupOtherPlayer();
		}

		//setup the player data on UI. The value are SyncVar so the player
		//will be created with the right value currently on server

		OnMyName(playerName);
		OnMyFBID (playerFBid);


		//OnMyProfile(playerProfile);
	}

	void addPlayer(){
		LobbyPlayerListPinch._instance.AddPlayer (this);
	}

	public override void OnStartAuthority()
	{
		base.OnStartAuthority();

		//if we return from a game, color of text can still be the one for "Ready"
		//readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;

		if (FB.IsLoggedIn) {
			FacebookManager.Instance.GetProfile ();
			//Invoke ("SetupLocalPlayer", 0.4f);
			if (FacebookManager.Instance.profileName != null) {
				SetupLocalPlayer();
			} else {
				StartCoroutine ("WaitForProfileNameLobby");
			}
		}else
			SetupLocalPlayer();
	}

	IEnumerator WaitForProfileNameLobby(){
		while (FacebookManager.Instance.profileName == null) {
			yield return null;
		}
		SetupLocalPlayer();
	}

	void SetupOtherPlayer()
	{
		OnClientReady(false);
	}

	void SetupLocalPlayer()
	{
		Debug.Log ("function setup local player called, fb login " + FacebookManager.Instance.IsLoggedIn + "player name is " + FacebookManager.Instance.profileName);
		this.readyToBegin = true;
		//have to use child count of player prefab already setup as "this.slot" is not set yet
		if (playerName == "") {
			if(LobbyPlayerListPinch._instance)
				CmdNameChanged ("Player" + (LobbyPlayerListPinch._instance.playerListContentTransform.childCount - 1));
		}

		if (FacebookManager.Instance.IsLoggedIn) {
			playerFBid = AccessToken.CurrentAccessToken.UserId;
			playerName = FacebookManager.Instance.profileName.ToUpper();
			pProfile.sprite = FacebookManager.Instance.profilePic;
			Debug.Log ("user name is " + playerName);
			CmdNameChanged (playerName);
			CmdIDChange (playerFBid);
			//CmdProfileChange (playerProfile);
		}

		//when OnClientEnterLobby is called, the loval PlayerController is not yet created, so we need to redo that here to disable
		//the add button if we reach maxLocalPlayer. We pass 0, as it was already counted on OnClientEnterLobby
		if (LobbyManagerPinch.s_Singleton != null) LobbyManagerPinch.s_Singleton.OnPlayersNumberModified(0);
	}

	///===== callback from sync var

	public void OnMyName(string newName)
	{
		playerName = newName;
		pName.text = playerName;
	}

	public void OnMyFBID(string userID)
	{
		playerFBid = userID;
		//if (!isLocalPlayer)
			//StartCoroutine ("GetPlayerPic");
		GetUserPic (playerFBid);
	}

	IEnumerator GetPlayerPic(){
		Debug.Log ("fetch user profile");
		WWW url = new WWW ("https://graph.facebook.com/" + playerFBid 
			+ "/picture?type=square&height=128&width=128");
		Texture2D textFb2 = new Texture2D (128, 128, TextureFormat.DXT5, false);
		yield return url;
		url.LoadImageIntoTexture (textFb2);
		//FB.API("/" + playerFBid + "/picture?type=square&height=128&width=128", HttpMethod.GET,UpdatePlayerProfile);
		pProfile.sprite = Sprite.Create(textFb2, new Rect(0,0,128,128), new Vector2());
	}

	void GetUserPic(string id){
	//	Debug.Log ("fetch user profile, user ID is " + id);
//		WWW url = new WWW ("https://graph.facebook.com/" + playerFBid 
//			+ "/picture?type=large");
//		Texture2D textFb2 = new Texture2D (128, 128, TextureFormat.DXT5, false);
//		yield return url;
//		url.LoadImageIntoTexture (textFb2);
		FB.API("/" + id + "/picture?type=square&height=128&width=128", HttpMethod.GET,UpdatePlayerProfile);
		//pProfile.sprite = Sprite.Create(textFb2, new Rect(0,0,128,128), new Vector2());
	}

	void UpdatePlayerProfile(IGraphResult result){
		//Debug.Log ("result is " + result.ToString());
		if (result.Texture != null) {
			//Debug.Log ("result is " + result.ToString());
			pProfile.sprite = Sprite.Create (result.Texture, new Rect (0, 0, 128, 128), new Vector2 ());

		}
	}

	[Command]
	public void CmdNameChanged(string name)
	{
		playerName = name;
	}

	[Command]
	public void CmdIDChange(string uID)
	{
		playerFBid = uID;
	}

	//change name after lobby login
	public void ChangePlayerName(string name){
		//only change local player name
		//Debug.Log("change player name to " + name);
		if (isLocalPlayer) {
			Debug.Log("change player name to " + name);
			CmdNameChanged (name);
			CmdIDChange (AccessToken.CurrentAccessToken.UserId);
		}
	}

	[ClientRpc]
	public void RpcUpdateCountdown(int countdown)
	{
		LobbyManagerPinch.s_Singleton.countdownPanel.UIText.text = "Match Starting in " + countdown;
		LobbyManagerPinch.s_Singleton.countdownPanel.gameObject.SetActive(countdown != 0);
	}

	//Cleanup thing when get destroy (which happen when client kick or disconnect)
	public void OnDestroy()
	{
		LobbyPlayerListPinch._instance.RemovePlayer(this);
		if (LobbyManagerPinch.s_Singleton != null) LobbyManagerPinch.s_Singleton.OnPlayersNumberModified(-1);

	}


}
