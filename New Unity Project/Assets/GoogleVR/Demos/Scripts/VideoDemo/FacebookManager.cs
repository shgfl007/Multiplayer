using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System;


public class FacebookManager : MonoBehaviour {
	public bool IsLoggedIn{ get; set;}
	public string profileName{ get; set;}
	public Sprite profilePic{ get; set;}
	public string AppLinkUrl{ get; set;}

	private static FacebookManager _instance;
	public static FacebookManager Instance
	{
		get{ 
			if (_instance == null) {
				GameObject fbm = new GameObject ("FBManager");
				fbm.AddComponent<FacebookManager> ();
			}

			return _instance;
		}	
	}

	void Awake(){
		_instance = this;
		DontDestroyOnLoad (this.gameObject);
	}



	public void InitFB(){
		if (!FB.IsInitialized) {
			FB.Init (SetInit, OnHideUnity);
		} else {
			IsLoggedIn = FB.IsLoggedIn;
		}
	}

	public void GetProfile(){
		Debug.Log ("get profile called");
		FB.API ("/me?fields=first_name", HttpMethod.GET, DisplayUserName);
		FB.API ("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
		FB.GetAppLink (DealWithAppLink);
		Debug.Log ("access token is " + AccessToken.CurrentAccessToken.TokenString);
		IsLoggedIn = FB.IsLoggedIn;
		//GetRequests ();
		//CleanUpRequests ();
	}

	void DealWithAppLink(IAppLinkResult result){
		if (!String.IsNullOrEmpty (result.Url)) {
			AppLinkUrl = result.Url + "";
			var index = (new Uri (result.Url)).Query.IndexOf ("request_ids");
			if (index != -1) {
				string temp = (new Uri (result.Url)).Query.Substring (index);
				string[] splitString = temp.Split ('&');
				Debug.Log ((new Uri (result.Url)).Query.Substring (index));
				Debug.Log (splitString [0]);
				string[] temp0 = splitString [0].Split ('=');
				string request_id = temp0 [1];
				var i = request_id.IndexOf ("%2C");
				if (i != -1) {
					string requestList = request_id;
					string[] request_id_list = requestList.Split (new string[] {"%2C"}, StringSplitOptions.None);
					request_id = request_id_list [request_id_list.Length - 1];
					CleanUpAllRequests (request_id_list);
				}
				Debug.Log (request_id);
				GetRoomID (request_id);
				if (i == -1) {
					CleanUpRequests (request_id);
				}
			}
		} else {
			AppLinkUrl = "http://google.com";
		}

		Debug.Log ("app link is " + AppLinkUrl);
	}
	void SetInit(){
		if (FB.IsLoggedIn) {
			Debug.Log ("FB is logged in");
			GetProfile ();
		} else {
			Debug.Log ("FB is not logged in");
		}
		IsLoggedIn = FB.IsLoggedIn;

	}

	void OnHideUnity(bool isGameShown){
		if (!isGameShown) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}
	}

	void DisplayUserName(IResult result){
		if (result.Error == null) {
			profileName ="" + result.ResultDictionary ["first_name"];
			Debug.Log (profileName);
		} else {
			Debug.Log (result.Error);
		}
	}

	void DisplayProfilePic(IGraphResult result){
		if (result.Texture != null) {
			profilePic = Sprite.Create (result.Texture, new Rect (0, 0, 128, 128), new Vector2 ());
		}
	}

	public void Share(){
		FB.FeedShare (
			string.Empty, new Uri(AppLinkUrl),
			"This is the title",
			"This is the caption",
			"check out this game", null,string.Empty,
			ShareCallback
		);
	}

	void ShareCallback(IResult result){
		if (result.Cancelled) {
			Debug.Log ("share cancelled");
		} else if (!string.IsNullOrEmpty (result.Error)) {
			Debug.Log ("Error on share");
		} else if (!string.IsNullOrEmpty (result.RawResult)) {
			Debug.Log ("success");
		}
	}

	public void Invite(){
		FB.Mobile.AppInvite (
			new Uri(AppLinkUrl),
			null,
			InviteCallback
		);
	}

	void InviteCallback(IResult result){
		if (result.Cancelled) {
			Debug.Log ("Invite cancelled");
		} else if (!string.IsNullOrEmpty (result.Error)) {
			Debug.Log ("Error on invite");
		} else if (!string.IsNullOrEmpty (result.RawResult)) {
			Debug.Log ("Success on invite");
		}
	}

	public void ShareWithUsers(){
		FB.AppRequest (
			"Come and join me",
			null,
			new List<object> (){ "app_users" },
			null,
			null,
			string.Empty,
			null,
			ShareWithUsersCallback

		);
	}

	void ShareWithUsersCallback(IAppRequestResult result){
		Debug.Log (result.RawResult);
	}

	public void ShareWithUsersLobby(){
//		FB.AppRequest (
//			"Come and join me",
//			null,
//			new List<object> (){ "app_users" },
//			null,
//			null,
//			"{\"room\":" + GameStatesManager.Instance.RoomID +"," + "\"server\":" + GameStatesManager.Instance.ServerIP + "}",
//			"Join Multiplayer!",
//			ShareWithUsersLobbyCallback
//
//		);
		FB.AppRequest (
			"Come and join me",
			null,
			new List<object> (){ "app_users" },
			null,
			null,
			GameStatesManager.Instance.RoomID,
			"Join Multiplayer!",
			ShareWithUsersLobbyCallback

		);
	}

	void ShareWithUsersLobbyCallback(IAppRequestResult result){
		Debug.Log (result.RawResult);
		Debug.Log (""+result.ResultDictionary ["request"]);
		//Debug.Log("client token is " + AccessToken.CurrentAccessToken);
	}

//	public void GetRequests(){
//		//this function is to read all the request for the user when they enter the app
//		FB.API ("me/apprequests?access_token=" + AccessToken.CurrentAccessToken.TokenString,
//			HttpMethod.GET, DisplayRequests);
//		//use request object id to test the app
//		/*FB.API ("1194305877355320?access_token=" + AccessToken.CurrentAccessToken.TokenString,
//			HttpMethod.GET, DisplayRequests);*/
//	}

	public void GetRoomID(string requestID){
		FB.API(requestID+"?access_token=" + AccessToken.CurrentAccessToken.TokenString,
			HttpMethod.GET, DisplayRequests);
	}

	void DisplayRequests(IResult result){
		Debug.Log ("get request callback "+result.RawResult);
		List<object> dataList = new List<object>();
		//dataList = result.ResultDictionary ["data"];
//		foreach (object Obj in dataList) {
//			Debug.Log ("data is " + Obj.ToString ());
//		}
		Debug.Log ("data is " + result.ResultDictionary ["data"]);
		GameStatesManager.Instance.RoomID = result.ResultDictionary ["data"] as string;

	}

	void CleanUpRequests(string request_id){
		Debug.Log ("clean up request");

		FB.API (""+ request_id+"?access_token=" + AccessToken.CurrentAccessToken.TokenString,
			HttpMethod.DELETE, DisplayCleanUpRequests);
	}

	void CleanUpAllRequests(string[] request_id_list){
		Debug.Log ("clean up all requests");
		for (int i = 0; i < request_id_list.Length; i++) {
			FB.API (""+ request_id_list[i] + "?access_token=" + AccessToken.CurrentAccessToken.TokenString,
				HttpMethod.DELETE, DisplayCleanUpRequests);
		}
	}

	void DisplayCleanUpRequests(IResult result){
		Debug.Log ("get clean up request callback "+result.RawResult);

	}

	public void LogOut(){
		FB.LogOut ();
		Debug.Log ("log out: access token is " + AccessToken.CurrentAccessToken.ToString() );
		IsLoggedIn = false;
	}

}
