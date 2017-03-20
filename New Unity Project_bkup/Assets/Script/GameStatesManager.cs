using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatesManager : MonoBehaviour {

	public string RoomID{ get; set;}
	public string ServerIP{ get; set; }
	private static GameStatesManager _instance;
	public static GameStatesManager Instance
	{
		get{ 
			if (_instance == null) {
				GameObject gsm = new GameObject ("GameManager");
				gsm.AddComponent<GameStatesManager> ();
			}

			return _instance;
		}	
	}

	void Awake(){
		_instance = this;
		DontDestroyOnLoad (this.gameObject);
	}
}
