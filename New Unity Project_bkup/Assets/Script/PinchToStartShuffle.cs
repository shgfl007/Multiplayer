using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PinchToStartShuffle : MonoBehaviour {


	public Sprite[] shuffleImages;

	int index = 0;

	public Image currentImage;
	Color tempColor;
	public Image second;

	// Use this for initialization
	void Start () {
	
		tempColor = Color.white;
		currentImage.sprite = shuffleImages [index];
		StartCoroutine (shuffleImgFadeOut());
	}


	float pck4Time = 1;
	int counter = 0;


	private IEnumerator shuffleImgFadeOut () {
		if (index == 0) {
			second.gameObject.SetActive (true);
			second.CrossFadeAlpha (1, 0.1f, false);

		}
		if (index == 1)
			second.gameObject.SetActive (false);
		//tempColor.a = 1;
		float elapsedTime = 0;

		while (elapsedTime < pck4Time) {
			tempColor.a = Mathf.Lerp (tempColor.a, 0, (elapsedTime / pck4Time));
			currentImage.color =tempColor;
			elapsedTime += Time.deltaTime;

			if (index != 0)
				yield return new WaitForEndOfFrame ();
			
		}

		counter++;
		StartCoroutine (shuffleImgFadeIn ());

	}
	private IEnumerator shuffleImgFadeIn () {
		tempColor.a = 0;
		float elapsedTime = 0;

		if (counter == 1) {
			index++;
			if (index >= shuffleImages.Length) {
				index = 0;
			
			}
			currentImage.sprite = shuffleImages[index];
			
			counter = -1;
		}

		if (index == 1) {
			currentImage.gameObject.SetActive (false);
		} else {
			currentImage.gameObject.SetActive (true);
		}
		while (elapsedTime < pck4Time) {
			tempColor.a = Mathf.Lerp (tempColor.a, 1, (elapsedTime / pck4Time));
			currentImage.color =tempColor;
			elapsedTime += Time.deltaTime;

			yield return new WaitForEndOfFrame ();
		}

		counter++;
		StartCoroutine ( shuffleImgFadeOut());

	}
		
}
