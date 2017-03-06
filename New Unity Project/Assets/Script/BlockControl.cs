using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockControl : MonoBehaviour {
	//this script manages the local statues of the block
	//will call network methods from the player if neccessary
	public GameObject Highlight;
	GameObject canvasHolder;
	public bool triggered = false;
	public GameObject objToModify;
	// Use this for initialization
	void Start () {
		canvasHolder = GameObject.Find ("CanvasHolder");
		Highlight.SetActive (false);
	}

	public void summonUI(){
		BlockMenu.instance.TurnOn ();
		canvasHolder.transform.SetParent(this.transform);
		canvasHolder.transform.localPosition = new Vector3(0,3,0);
		BlockMenu.instance.curObject = this.gameObject;
	
	}

	public void controlHighlighter(bool show){
		Color color;
		if (objToModify != null) {
			color = objToModify.GetComponent<MeshRenderer> ().material.color;
		}else
			color = this.gameObject.GetComponent<MeshRenderer> ().material.color;
		Highlight.GetComponent<MeshRenderer> ().material.color = new Color (color.r, color.g, color.b, 0.3f);
		Highlight.SetActive (show);

	}

	public void changeHighlightColour(Color col){
		Color color = col;
		color.a = 0.3f;
		Highlight.GetComponent<MeshRenderer> ().material.color = color;
	}

	public void DestroyObject(){
		if (BlockMenu.instance.curObject == this.gameObject) {
			BlockMenu.instance.TurnOff ();
			canvasHolder.transform.SetParent(null);
		}
		Destroy (this.gameObject);
	}

	public void RotateObject(){
		Debug.Log ("block control rotation called");
		if (objToModify != null) {
			objToModify.transform.Rotate (new Vector3 (0f, 0f, 90f));
		} else {
			this.gameObject.transform.Rotate (new Vector3 (0f, 0f, 90f));
		}
	}

	public void ChangeColour(Color col){
		if (objToModify != null) {
			objToModify.GetComponent<MeshRenderer> ().material.color = col;
		} else {
			this.gameObject.GetComponent<MeshRenderer> ().material.color = col;
		}
	}

	// Update is called once per frame
	void Update () {
		
		
	}
}
