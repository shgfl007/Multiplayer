using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MoveObject : NetworkBehaviour {

	private Vector2 mouseOver;
	private Vector2 touchOver;
	private Vector3 screenPoint;


	public Vector3 currentPos;


	// Use this for initialization
	void Start () {
		currentPos = gameObject.transform.position;
	
	}
	
	// Update is called once per frame
	void Update () {

		//do a raycast
		updateMouseOver();
	}

	void OnMouseDown(){
		screenPoint = Camera.main.WorldToScreenPoint (gameObject.transform.position);

	}


	void OnMouseDrag(){
		Vector3 currentScreenPoint = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 currentPosT = Camera.main.ScreenToWorldPoint (currentScreenPoint);
		transform.position = currentPosT;
		CmdOnChangePosition (currentPosT, gameObject.name);

	}
	[Command]
	void CmdOnChangePosition(Vector3 currentPosTemp, string GameObjName){
		Debug.Log ("on change position function called");
		currentPos = currentPosTemp;
		GameObject.Find (GameObjName).transform.position = currentPos;

	}

	void updateMouseOver(){
		RaycastHit hit;

		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 525f)) {
			Debug.Log ("doing raycast");
//			mouseOver.x = (int)(hit.point.x);
//			mouseOver.y = (int)(hit.point.z);
			if(hit.collider.gameObject.tag == "Object"){
				Debug.Log ("hit object at " + hit.point);
				if (Input.GetMouseButtonDown (0)) {
					
				}
			}
		} 
	}
}
