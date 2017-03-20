using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ObjectSpawner : NetworkBehaviour {

	public GameObject objectPrefab;
	public int numberOfObjects;

	public override void OnStartServer(){
		Debug.Log ("it gets here");
		for (int i = 0; i < numberOfObjects; i++) {
			var spawnPosition = new Vector3 (
				Random.Range(-8f,8f),
				0f,
				Random.Range(-8f,8f));

			var spawnRotation = Quaternion.Euler (0f, Random.Range(0,180),
				0f);
			var obj = (GameObject)Instantiate (objectPrefab, spawnPosition, spawnRotation);
			obj.SetActive (true);
			NetworkServer.Spawn (obj);
		}
	}

}
