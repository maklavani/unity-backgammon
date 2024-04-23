using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonObject : MonoBehaviour {
	public GameObject textObject;
	public GameObject loadingObject;
	public bool loading;
	public bool disabled;
	[Range(1 , 10)]
	public float delay = 1f;

	private Button button;
	private Image image;
	private Text text;
	private Text loadingText;

	void Awake(){
		button = GetComponent<Button> ();
		image = GetComponent<Image> ();
		text = textObject.GetComponent<Text> ();
		loadingText = loadingObject.GetComponent<Text> ();
		checkButton ();
	}

	void FixedUpdate(){
		checkButton ();
	}

	public void checkButton(){
		var loadingColor = loadingText.color;

		if (loading) {
			loadingColor.a = 1;
			loadingObject.transform.Rotate (Vector3.forward * -90 * (delay * Time.deltaTime));
		} else {
			loadingColor.a = 0;
		}

		var imageColor = image.color;
		var textColor = text.color;

		if (disabled) {
			button.interactable = false;
			imageColor.a = 0.2f;
			textColor.a = 0.3f;
		} else {
			button.interactable = true;
			imageColor.a = 0.2f;
			textColor.a = 1f;
		}

		loadingText.color = loadingColor;
		image.color = imageColor;
		text.color = textColor;
	}
}