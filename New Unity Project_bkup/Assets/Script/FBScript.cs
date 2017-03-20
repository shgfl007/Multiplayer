using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FBScript : MonoBehaviour {

	public GameObject DialogLoggedIn;
	public GameObject DialogLoggedOut;
	public GameObject DialogUserName;
	public GameObject DialogProfilePic;
	bool initStatus = false;
	void Awake(){
		FacebookManager.Instance.InitFB ();

		//DealWithFbMenus (FacebookManager.Instance.IsLoggedIn);
		Invoke("setFbMenu", 0.1f);
		Debug.Log(FacebookManager.Instance.IsLoggedIn);
	}

	void setFbMenu(){
		bool isLoggedIn = FacebookManager.Instance.IsLoggedIn;
		Debug.Log("set fb menu" + FacebookManager.Instance.IsLoggedIn);
		DealWithFbMenus (isLoggedIn);
	}
	//this function gets called when pressing the login button
	public void FBlogin(){
		FB.LogOut ();
		List<string> permissions = new List<string> ();
		permissions.Add ("public_profile");
		permissions.Add ("user_friends");
		FB.LogInWithReadPermissions (permissions, AuthCallBack);
	}

	void AuthCallBack(IResult result){
		if (result.Error != null) {
			Debug.Log (result.Error);
		} else {
			if (FB.IsLoggedIn) {
				FacebookManager.Instance.IsLoggedIn = true;
				//Debug.Log ("FB is logged in");
				//DealWithFbMenus (FacebookManager.Instance.IsLoggedIn);
				FacebookManager.Instance.GetProfile ();
			} else {
				//Debug.Log ("Fb is not logged in");
			}

			DealWithFbMenus (FB.IsLoggedIn);
		}
	}

	void DealWithFbMenus(bool isLoggedIn){
		if (isLoggedIn) {
			DialogLoggedIn.SetActive (true);
			DialogLoggedOut.SetActive (false);

			if (FacebookManager.Instance.profileName != null) {
				Text userName = DialogUserName.GetComponent<Text> ();
				userName.text = FacebookManager.Instance.profileName.ToUpper();
			} else {
				StartCoroutine ("WaitForProfileName");
			}

			if (FacebookManager.Instance.profileName != null) {
				Image userPic = DialogProfilePic.GetComponent<Image>();
				userPic.sprite = FacebookManager.Instance.profilePic;
			} else {
				StartCoroutine ("WaitForProfilePic");
			}

		} else {
			DialogLoggedIn.SetActive (false);
			DialogLoggedOut.SetActive (true);
		}
	}

	IEnumerator WaitForProfileName(){
		while (FacebookManager.Instance.profileName == null) {
			yield return null;
		}
		DealWithFbMenus (FB.IsLoggedIn);
	}
 
	IEnumerator WaitForProfilePic(){
		while (FacebookManager.Instance.profilePic == null) {
			yield return null;
		}
		DealWithFbMenus (FB.IsLoggedIn);
	}

	public void Share(){
		FacebookManager.Instance.Share ();
	}

	public void Invite(){
		FacebookManager.Instance.Invite ();
	}

	public void ShareWithUsers(){
		FacebookManager.Instance.ShareWithUsers ();
	}

	public void GoToNextScene(){
		SceneManager.LoadSceneAsync (1);

	}

	public void LogOut(){
		FacebookManager.Instance.LogOut ();
		//Debug.Log (FacebookManager.Instance.IsLoggedIn);
		Invoke("setFbMenu",0.1f);
	}


}
