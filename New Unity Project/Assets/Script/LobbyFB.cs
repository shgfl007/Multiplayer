using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using Prototype.NetworkLobby;
public class LobbyFB : MonoBehaviour {

	public GameObject loginPanel;
	public GameObject loginButton;
	public Text loginPanelText;
	void Awake(){
		//if (!FacebookManager.Instance)
			FacebookManager.Instance.InitFB ();

	}

	public void ShareWithUsers(){
		if (FacebookManager.Instance.IsLoggedIn) {
			FacebookManager.Instance.ShareWithUsersLobby ();
		} else {
			//facebook is not logged in
			loginPanel.SetActive(true);
		}
	}

	public void cancelLoginPanel(){
		loginPanel.SetActive (false);

	}


	public void LobbyLogin(){
		//FB.LogOut ();
		List<string> permissions = new List<string> ();
		permissions.Add ("public_profile");
		permissions.Add ("user_friends");
		FB.LogInWithReadPermissions (permissions, AuthCallBack);
	}

	void AuthCallBack(IResult result){
		if (result.Error != null) {
			Debug.Log (result.Error);
			loginPanelText.text = "Login failed! " + result.Error;
		} else {
			if (FB.IsLoggedIn) {
				FacebookManager.Instance.IsLoggedIn =FB.IsLoggedIn;
				loginPanelText.text = "Login success! ";
				//hide login button after success login
				loginButton.SetActive(false);
				FacebookManager.Instance.GetProfile ();
				if (FacebookManager.Instance.profileName != null) {
					updatePlayerName ();
				} else {
					StartCoroutine ("WaitForProfileName");
				}
				if (loginPanel.activeSelf) {
					Invoke ("cancelLoginPanel", 2.3f);
					Invoke ("ShareWithUsers", 2.35f);
				}
			} else {
				//Debug.Log ("Fb is not logged in");
			}
		}
	}

	IEnumerator WaitForProfileName(){
		while (FacebookManager.Instance.profileName == null) {
			yield return null;
		}
		updatePlayerName ();
	}

	IEnumerator WaitForProfilePic(){
		while (FacebookManager.Instance.profilePic == null) {
			yield return null;
		}
		//DealWithFbMenus (FB.IsLoggedIn);
	}

	void updatePlayerName(){
		GameObject[] lobbyPlayer = GameObject.FindGameObjectsWithTag ("Player");
		string playerName = FacebookManager.Instance.profileName;
		//update player name
		for(int i = 0; i < lobbyPlayer.Length; i++){
			lobbyPlayer [i].GetComponent<LobbyPlayer> ().ChangePlayerName(playerName);
		}
	}
}
