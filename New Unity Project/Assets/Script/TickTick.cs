using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;
using UnityEngine.Networking;
public class TickTick : MonoBehaviour {

	public float time = 11;
	public Text txt;
	//public NetworkManager lm;
	public NetworkLobbyManager lm;
	public string NextScene;

	//public SceneAsset 
	// Use this for initialization
	void Start () {
	
		StartCoroutine (Ticker());
		lm = GameObject.Find ("LobbyManagerPinch").GetComponent<LobbyManagerPinch> ();

	}
	private IEnumerator Ticker() {

		float elapsedTime = time;

		while (elapsedTime > 0) {


			elapsedTime -= Time.deltaTime;
			txt.text = ((int)elapsedTime).ToString ();

			yield return new WaitForEndOfFrame ();
		}

		//Load the New Scene Right Here Dawg!
		lm.ServerChangeScene(NextScene);
//		GoToxScene.instance.sceneIndex = 3;
//		GoToxScene.instance.GoToScene ();
	}

}
