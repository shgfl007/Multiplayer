using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyOrientationControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (Screen.orientation != ScreenOrientation.Portrait)
			Screen.orientation = ScreenOrientation.Portrait;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
