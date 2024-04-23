using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinnerManager : MonoBehaviour {
	[HideInInspector]
	public List<GameObject> images;
	[HideInInspector]
	public List<Color> colors;
	[Range(100 , 1000)]
	public int numberOfImages = 500;
	private GameManager gameManger;
	private int numberOfImagesCreate = 0;

	void Awake(){
		gameManger = GetComponent<GameManager> ();
	}

	public IEnumerator CreateImage(){
		yield return new WaitForSeconds (0.01f);
		GameObject go = null;
		numberOfImagesCreate++;

		go = Instantiate (images[Random.Range(0 , images.Count - 1)] , gameManger.winnerPageObject.transform);
		go.name = "Paper_" + numberOfImagesCreate;
		go.gameObject.GetComponent<SpriteRenderer> ().color = colors [Random.Range (0 , colors.Count - 1)];

		if (numberOfImagesCreate < numberOfImages)
			StartCoroutine (CreateImage ());
	}
}