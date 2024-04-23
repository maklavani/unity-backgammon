using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBoxObject : MonoBehaviour {
	public GameObject icon;
	private Toggle toggle;
	private Text text;

	void Awake(){
		toggle = GetComponent<Toggle> ();
		text = icon.GetComponent<Text> ();
		CheckChecked ();
	}

	void FixedUpdate(){
		CheckChecked ();
	}

	public void CheckChecked(){
		var color = text.color;
	
		if (toggle.isOn) {
			color.a = 1;
		} else {
			color.a = 0;
		}

		text.color = color;
	}
}