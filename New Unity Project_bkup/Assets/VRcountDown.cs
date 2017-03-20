using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class VRcountDown : MonoBehaviour {
	public float time = 11;
	public Camera Main;
	public Text txt;
	public GameObject VRStarter;
	GameObject[] viewerList;
	GameObject viewer;
	// Use this for initialization
	void Start () {
//		Debug.Log ("gvrViewer is " + GameObject.FindGameObjectWithTag("gvrView"));
//		try{
//			Debug.Log("in 'try' ");
//		//GameObject.Find ("GvrViewer").GetComponent<GvrViewer> ().VRModeEnabled = false;
//			viewerList = GameObject.FindGameObjectsWithTag("gvrView");
//			Debug.Log("viewer list has a length of " + viewerList.Length);
//			for(int i = 0; i<viewerList.Length; i++){
//				viewerList[i].GetComponent<GvrViewer>().VRModeEnabled = false;
//			}
//		}catch(System.Exception exp){
//			Debug.Log (exp.ToString ());
//		}
		//get a list of players
		//GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		if (Main != null)
			Main.enabled = true;
		//StartCoroutine (getViewer ());
		//StartCoroutine (Ticker());
		//Invoke()
		getViewer();
	}

	private void getViewer(){
		Debug.Log ("getViewer called");
		viewer = GameObject.FindGameObjectWithTag ("gvrView");
		Debug.Log ("viewer is " + viewer.name);
		viewer.GetComponent<GvrViewer> ().VRModeEnabled = false;
		StartCoroutine (Ticker ());
	}


	private IEnumerator getViewerE(){
		viewer = GameObject.FindGameObjectWithTag ("gvrView");
		if (viewer == null) {
			yield return null;
		}
		viewer.GetComponent<GvrViewer> ().VRModeEnabled = false;
		StartCoroutine (Ticker ());
	}

	private IEnumerator Ticker(){
		float elapsedTime = time;
		while (elapsedTime > 0) {


			elapsedTime -= Time.deltaTime;
			txt.text = ((int)elapsedTime).ToString ();

			yield return new WaitForEndOfFrame ();
		}
		VRStarter.SetActive (false);
		Main.enabled = false;
		Main.gameObject.SetActive (false);

		GameObject.Find ("GvrViewer").GetComponent<GvrViewer> ().VRModeEnabled = true;
		try{
			viewer.GetComponent<GvrViewer> ().VRModeEnabled = true;
			//GameObject.Find ("GvrViewer").GetComponent<GvrViewer> ().VRModeEnabled = true;
//			for(int i = 0; i<viewerList.Length; i++){
//				viewerList[i].GetComponent<GvrViewer>().VRModeEnabled = true;
//			}
		}catch(System.Exception exp){
			Debug.Log (exp.ToString ());
		}
	}
	

}
