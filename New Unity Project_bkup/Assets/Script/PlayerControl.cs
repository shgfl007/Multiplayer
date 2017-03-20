using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerControl : NetworkBehaviour {
	public GameObject bulletPrefab;
	public Transform bulletSpawn;
	Vector3 screenPoint;
	private bool isTriggered = false;
	Vector3 prePosition;
	Vector3 offset;
	Vector3 lastPosition;
	GameObject objToMove;
	GameObject objToChangeMatColor;
	bool init = true;
	public GameObject ChildCamera;
	public float minX = -8, minZ = -8 , maxX = 8, maxZ = 8;
	public GameObject objToSpawn;
	//public static bool islocal = false;
//	public GameObject GvrViewer;
//	public GameObject GvrControllerMain;
//	public GameObject[] GvrObjToAttachToCamera;
	public override void OnStartLocalPlayer(){
		GetComponent<MeshRenderer> ().material.color = Color.blue;

		ChildCamera.tag = "MainCamera";
		ChildCamera.AddComponent<StereoController> ();
		//Camera.main.gameObject.AddComponent<StereoController> ();
		//gameObject.AddComponent<GvrReticlePointer> ();

		//move the player to a random position
		Vector3 newPos = new Vector3(Random.Range(minX, maxX), 0f, Random.Range(minZ,maxZ));
		var spawnRotation = Quaternion.Euler (0f, Random.Range(0,180),
			0f);
		gameObject.transform.rotation = spawnRotation;
		gameObject.transform.position = newPos;

	}
	// Update is called once per frame
	void Update()
	{
		if (!isLocalPlayer) {
			//GameObject cameraOBJ = gameObject.transform.FindChild ("Camera").gameObject;
			//cameraOBJ.transform.SetParent (null);
			if (init) {
				//ChildCamera.SetActive (false);
				//ChildCamera.transform.parent = null;
				//Destroy (ChildCamera);
				ChildCamera.GetComponent<AudioListener> ().enabled = false;
				ChildCamera.GetComponent<Camera> ().enabled = false;
				ChildCamera.SetActive (false);
				//Destroy (ChildCamera);
				init = false;
			}
			//gameObject.transform.FindChild ("Camera").gameObject.SetActive(false);
			//Destroy (cameraOBJ);
			return;
		} else {
			if(this.gameObject.tag!="Player")
				this.gameObject.tag = "Player";

		}

		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

		transform.Rotate(0, x, 0);
		transform.Translate(0, 0, z);

		if (Input.GetKeyDown (KeyCode.Space)) {
			CmdFire ();
		}

		//do raycast here
		//Check if we are running either in the Unity editor or in a standalone build.
		#if UNITY_STANDALONE || UNITY_WEBPLAYER
		//call TryMove()
		TryMove();
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			CmdTryCreate();//CmdChangeMatColour(objToChangeMatColor);
			//Debug.Log("go is " + go.name + " position is " + go.transform.position);
		}
		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
		//call trymovetouch
		TryMoveTouch();
		#endif

	}
	public void TryMoveTouch(){
		RaycastHit hit;
		Touch touch;
		//Debug.Log (Input.touchCount);
		if (Input.touchCount > 0) {
			touch = Input.touches [0];

			if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved) {
				// do ray cast when touch is detected
				//Vector2 touchPos = touch.position;
				if (Physics.Raycast (Camera.main.ScreenPointToRay (touch.position), out hit, 500f)) {
					Debug.Log ("hit something!");
					if ((hit.collider.gameObject.tag == "Object")) {
						if (!isTriggered) {
							isTriggered = true;
						}
						offset = hit.collider.transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (touch.position.x, touch.position.y, screenPoint.z));
						objToMove = hit.collider.gameObject;
						screenPoint = Camera.main.WorldToScreenPoint (hit.collider.gameObject.transform.position);
//						Vector3 curScreenPoint = new Vector3 (touch.position.x, touch.position.y, screenPoint.z);
//						Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint);
//						//if (objToMove != null)
//							CmdMoveObj (curPosition, objToMove);
					}
				}

			} else if (isTriggered && touch.phase == TouchPhase.Ended) {
				isTriggered = false;
				objToMove = null;
			}


		} else {
			isTriggered = false;
			objToMove = null;
		}
		if (isTriggered) {

			print ("dragging ");
			Vector3 curScreenPoint = new Vector3 (touch.position.x, touch.position.y, screenPoint.z);
			Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint)+ offset;
			//if (objToMove != null)
			//CmdMoveObj (curPosition, objToMove);
			if(objToMove != null)
				CmdMoveObj (curPosition,objToMove);
			//this.transform.transform.position = curPosition;
		}




	}

	public void TryMove(){
		RaycastHit hit;
		GameObject[] objs = GameObject.FindGameObjectsWithTag ("Object");
		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 525f)) {
			Debug.Log ("hit " + hit.transform.gameObject.name);
			//hit object and start dragging
			if ((hit.collider.gameObject.tag == "Object")) {
				//turn on the highlight 
				gameObject.GetComponent<ChangeColour> ().showHighLight (hit.transform.gameObject);
				//go through all the other objects and turn off the highlight
				hit.transform.gameObject.GetComponent<BlockControl>().triggered = true;
				for (int i = 0; i < objs.Length; i++) {
					if (objs [i] != hit.transform.gameObject && objs [i] != BlockMenu.instance.curObject) {
						gameObject.GetComponent<ChangeColour> ().hideHighLight (objs [i]);
						objs [i].GetComponent<BlockControl> ().triggered = false;
					}
				}
				if (Input.GetMouseButtonDown (0)) {
					objToMove = hit.collider.gameObject;
					if (!isTriggered) {
						isTriggered = true;
						prePosition = hit.collider.gameObject.transform.position;
						screenPoint = Camera.main.WorldToScreenPoint (hit.collider.gameObject.transform.position);
						offset = hit.collider.transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
						//hit.transform.gameObject.GetComponent<BlockControl> ().enableHighlighter ();
						//gameObject.GetComponent<ChangeColour>().showHighLight(hit.transform.gameObject);
					} else if (isTriggered && Input.GetMouseButtonUp (0)) {
						print ("end dragging");
						isTriggered = false;
						lastPosition = hit.collider.gameObject.transform.position;
					
					}
//					Vector3 currentScreenPoint = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
//					Vector3 currentPosT = Camera.main.ScreenToWorldPoint (currentScreenPoint);
					//transform.position = currentPosT;
					//hit.collider.gameObject.transform.position = currentPosT;

				} else if (Input.GetMouseButtonUp (0)) {
					isTriggered = false;
					objToMove = null;
					//call menu when button is released
					BlockControl ctrl = hit.transform.gameObject.GetComponent<BlockControl> ();
					ctrl.summonUI ();
				}

			} else {
				Debug.Log ("not hitting anything");
				for (int i = 0; i < objs.Length; i++) {
					if (objs [i] != BlockMenu.instance.curObject) {
						gameObject.GetComponent<ChangeColour> ().hideHighLight (objs [i]);
					}
				}
			}

		}
		if (Input.GetMouseButtonUp (0))
			isTriggered = false;
		
		if (isTriggered) {

			print ("dragging ");
			Vector3 curScreenPoint = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
			Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint) + offset;
			if(objToMove != null)
				CmdMoveObj (curPosition,objToMove);
			//this.transform.transform.position = curPosition;
		}


	}

	[Command]
	void CmdTryCreate(){
		//this function creates an obj 
		GameObject go = Instantiate(objToSpawn, Vector3.zero, Quaternion.identity) as GameObject;
		go.GetComponent<MeshRenderer> ().material.color = Color.green;
		go.tag = "Object";
		objToChangeMatColor = go;
		NetworkServer.Spawn (go);
		//CmdChangeMatColour (go);
		//return go;
	}

	[Command]
	void CmdChangeMatColour(GameObject obj){
		//this function changes the colour of the given object
		Debug.Log("change colour " + obj.name);
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Object");
		for (int i = 0; i < objs.Length; i++) {
			if(objs[i] == obj){
				objs[i].GetComponent<MeshRenderer>().material.color = Color.green;
			}
		}
	}

	[Command]
	void CmdMoveObj(Vector3 pos, GameObject obj){
		//Debug.Log ("moving obj " + obj.name + "to " + pos);
		GameObject[] objs = GameObject.FindGameObjectsWithTag ("Object");
		for (int i = 0; i < objs.Length; i++) {
			if (objs[i] == obj) {
				objs [i].transform.position = pos;
			}
		}
	}

	[Command]
	void CmdFire(){
		//create the bullet from the bullet prefab
		var bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation) as GameObject;
		bullet.GetComponent<Rigidbody> ().velocity = bullet.transform.forward * 6;

		NetworkServer.Spawn (bullet);

		Destroy (bullet, 2.0f);
	}

}
