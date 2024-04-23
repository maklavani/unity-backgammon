using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour {
	private GameManager gameManager;

	[HideInInspector]
	public ThemeData[] themes;
	public Themes theme;

	public GameObject wrapperLayout;
	public GameObject userLayout;
	public GameObject gameLayout;
	public GameObject userA;
	public GameObject setting;
	public GameObject userB;
	public GameObject exit;

	[HideInInspector]
	public GameObject nutObject;
	[HideInInspector]
	public Nuts nutType;
	[HideInInspector]
	public Inners innerType;
	[HideInInspector]
	public Status leather;
	[HideInInspector]
	public Status triangleLine;
	[HideInInspector]
	public Color bgColor;
	[HideInInspector]
	public Color bgInnerColor;
	[HideInInspector]
	public Color tbColor;
	[HideInInspector]
	public Color hlColor;
	[HideInInspector]
	public Color hlDarkColor;
	[HideInInspector]
	public Color innerColor;
	[HideInInspector]
	public Color timeColor;
	[HideInInspector]
	public Color hoverColor;
	[HideInInspector]
	public Color whiteLineColor;
	[HideInInspector]
	public Color blackLineColor;
	[HideInInspector]
	public Color whiteNutColor;
	[HideInInspector]
	public Color blackNutColor;
	[HideInInspector]
	public Color txtColor;

	// Animation
	private bool playTimeOutStatus = false;
	private float delay;
	private float timeElapsed;
	private Image timeOut;
	private Text timeOutText;

	void Awake(){
		gameManager = GetComponent<GameManager> ();
	}

	void Update(){
		if (playTimeOutStatus) {
			delay += Time.deltaTime;

			if (delay > 1f) {
				delay = 0f;
				timeElapsed -= 1f;

				if(timeElapsed == 0f) {
					playTimeOutStatus = false;
					timeOut.fillAmount = 0f;
					timeOutText.text = "";

					if (gameManager.myTurn == gameManager.turn) {
						gameManager.requestManager.DisableRequest ("Alive");
						gameManager.requestManager.DisableRequest ("Send Data");
						gameManager.requestManager.DisableRequest ("Check Movement");
						gameManager.requestManager.EnableRequest ("Resign");
						Debug.Log ("Resign");
					}
				} else if (timeElapsed <= 45f) {
					timeOutText.text = timeElapsed.ToString ();
					timeOut.fillAmount = 0f;
					gameManager.gameControl.translateLanguage = true;
				} else {
					timeOutText.text = "";

					if(timeElapsed - 60 <= 0)
						timeOut.fillAmount = 1f / 15f * (timeElapsed - 45f);
				}
			}
		}
	}

	public void SetTheme(){
		// Dialog Open
		if (gameManager.myTurn == 1) {
			userA.transform.Find ("Profile").Find ("ProfileLayout").Find ("Dialog").gameObject.SetActive (true);
			userA.transform.Find ("Profile").Find ("ProfileLayout").Find ("ImageParent").GetComponent<Button> ().interactable = false;
		} else if (gameManager.myTurn == 2) {
			userB.transform.Find ("Profile").Find ("ProfileLayout").Find ("Dialog").gameObject.SetActive (true);
			userB.transform.Find ("Profile").Find ("ProfileLayout").Find ("ImageParent").GetComponent<Button> ().interactable = false;
		} else {
			userA.transform.Find ("Profile").Find ("ProfileLayout").Find ("ImageParent").GetComponent<Button> ().interactable = false;
			userB.transform.Find ("Profile").Find ("ProfileLayout").Find ("ImageParent").GetComponent<Button> ().interactable = false;
		}

		// Change Postion Layout
		if (gameManager.color == "black")
			changePositionLayout ();

		// Change Postion Layout Language
		if (gameManager.language == Languages.Persian)
			changePositionLayoutLanguage ();

		// Rename Line and Select Layout
		renameLayout("Line");
		renameLayout("Select");

		// Select Set Active False
		var objs = GameObject.FindGameObjectsWithTag ("Select");
		foreach (var obj in objs)
			obj.transform.Find("Button").gameObject.SetActive (false);

		// Set Theme
		// Comment
		// theme = Themes.Leather;
		theme = gameManager.gameControl.theme;
		UpdateTheme ();
	}

	public void UpdateTheme(){
		// Get Theme
		nutObject = themes [(int)theme - 1].nut;
		nutType = themes [(int)theme - 1].nutType;
		innerType = themes [(int)theme - 1].innerType;
		leather = themes [(int)theme - 1].leather;
		triangleLine = themes [(int)theme - 1].triangleLine;
		bgColor = themes [(int)theme - 1].backgroundColor;
		bgInnerColor = themes [(int)theme - 1].backgroundInnerColor;
		tbColor = themes [(int)theme - 1].tableColor;
		hlColor = themes [(int)theme - 1].hightLightColor;
		hlDarkColor = themes [(int)theme - 1].hightLightDarkColor;
		innerColor = themes [(int)theme - 1].innerColor;
		timeColor = themes [(int)theme - 1].timeColor;
		hoverColor = themes [(int)theme - 1].hoverColor;
		whiteLineColor = themes [(int)theme - 1].whiteLineColor;
		blackLineColor = themes [(int)theme - 1].blackLineColor;
		whiteNutColor = themes [(int)theme - 1].whiteNutColor;
		blackNutColor = themes [(int)theme - 1].blackNutColor;
		txtColor = themes [(int)theme - 1].textColor;

		// Set Alpha
		bgColor.a = 1;
		bgInnerColor.a = 1;
		tbColor.a = 1;
		hlColor.a = 1;
		hlDarkColor.a = 1;
		timeColor.a = 1f;
		hoverColor.a = 0.6f;
		whiteLineColor.a = 1;
		blackLineColor.a = 1;
		whiteNutColor.a = 1;
		blackNutColor.a = 1;
		txtColor.a = 1;

		// Objects Color
		wrapperLayout.GetComponent<Image> ().color = bgColor;
		gameLayout.transform.Find("LineLayout").Find("Table").GetComponent<Image> ().color = bgColor;
		gameLayout.transform.Find("LineLayout").Find("Table").Find("Left").Find("Body").Find ("InnerBase").GetComponent<Image> ().color = innerColor;
		gameLayout.transform.Find("LineLayout").Find("Table").Find("Right").Find("Body").Find ("InnerBase").GetComponent<Image> ().color = innerColor;
		gameLayout.transform.Find("LineLayout").Find("Table").Find("Right").Find("Body").GetComponent<Image> ().color = tbColor;
		gameLayout.transform.Find("LineLayout").Find("Table").Find("Left").Find("Body").GetComponent<Image> ().color = tbColor;
		gameLayout.transform.Find("LineLayout").Find("Table").Find("Middle").Find("Body").Find("TopLula").GetComponent<Image> ().color = hlColor;
		gameLayout.transform.Find("LineLayout").Find("Table").Find("Middle").Find("Body").Find("BottomLula").GetComponent<Image> ().color = hlColor;

		if (innerType == Inners.Inner) {
			gameLayout.transform.Find ("LineLayout").Find ("Table").Find ("Right").Find ("Body").Find ("InnerShadow").gameObject.SetActive (true);
			gameLayout.transform.Find ("LineLayout").Find ("Table").Find ("Left").Find ("Body").Find ("InnerShadow").gameObject.SetActive (true);
			gameLayout.transform.Find ("LineLayout").Find ("Table").Find ("Right").Find ("Body").Find ("InnerLine").gameObject.SetActive (false);
			gameLayout.transform.Find ("LineLayout").Find ("Table").Find ("Left").Find ("Body").Find ("InnerLine").gameObject.SetActive (false);
			gameLayout.transform.Find ("LineLayout").Find ("Table").Find ("Right").Find ("Body").Find ("InnerShadow").GetComponent<Image> ().color = innerColor;
			gameLayout.transform.Find ("LineLayout").Find ("Table").Find ("Left").Find ("Body").Find ("InnerShadow").GetComponent<Image> ().color = innerColor;
		} else if (innerType == Inners.InnerB) {
			gameLayout.transform.Find ("LineLayout").Find ("Table").Find ("Right").Find ("Body").Find ("InnerShadow").gameObject.SetActive (false);
			gameLayout.transform.Find ("LineLayout").Find ("Table").Find ("Left").Find ("Body").Find ("InnerShadow").gameObject.SetActive (false);
			gameLayout.transform.Find ("LineLayout").Find ("Table").Find ("Right").Find ("Body").Find ("InnerLine").gameObject.SetActive (true);
			gameLayout.transform.Find ("LineLayout").Find ("Table").Find ("Left").Find ("Body").Find ("InnerLine").gameObject.SetActive (true);
			gameLayout.transform.Find ("LineLayout").Find ("Table").Find ("Right").Find ("Body").Find ("InnerLine").GetComponent<Image> ().color = tbColor;
			gameLayout.transform.Find ("LineLayout").Find ("Table").Find ("Left").Find ("Body").Find ("InnerLine").GetComponent<Image> ().color = tbColor;
		}

		// Users Color
		userA.GetComponent<Image> ().color = bgInnerColor;
		userB.GetComponent<Image> ().color = bgInnerColor;
		userA.transform.Find ("Profile").Find ("ProfileLayout").Find ("TimeOutHighLight").GetComponent<Image> ().color = bgInnerColor;
		userB.transform.Find ("Profile").Find ("ProfileLayout").Find ("TimeOutHighLight").GetComponent<Image> ().color = bgInnerColor;
		userA.transform.Find ("Profile").Find ("ProfileLayout").Find ("TimeOut").GetComponent<Image> ().color = timeColor;
		userB.transform.Find ("Profile").Find ("ProfileLayout").Find ("TimeOut").GetComponent<Image> ().color = timeColor;
		userA.transform.Find ("Profile").Find ("ProfileLayout").Find ("TimeOutBackground").GetComponent<Image> ().color = bgInnerColor;
		userB.transform.Find ("Profile").Find ("ProfileLayout").Find ("TimeOutBackground").GetComponent<Image> ().color = bgInnerColor;
		userA.transform.Find ("Username").GetComponent<Text> ().color = txtColor;
		userB.transform.Find ("Username").GetComponent<Text> ().color = txtColor;
		userA.transform.Find ("LevelParent").Find ("Level").GetComponent<Text> ().color = txtColor;
		userA.transform.Find ("LevelParent").Find ("LevelText").GetComponent<Text> ().color = hoverColor;
		userB.transform.Find ("LevelParent").Find ("Level").GetComponent<Text> ().color = txtColor;
		userB.transform.Find ("LevelParent").Find ("LevelText").GetComponent<Text> ().color = hoverColor;
		userA.transform.Find ("ScoreParent").Find ("Score").GetComponent<Text> ().color = txtColor;
		userA.transform.Find ("ScoreParent").Find ("ScoreText").GetComponent<Text> ().color = hoverColor;
		userB.transform.Find ("ScoreParent").Find ("Score").GetComponent<Text> ().color = txtColor;
		userB.transform.Find ("ScoreParent").Find ("ScoreText").GetComponent<Text> ().color = hoverColor;
		setting.GetComponent<Image> ().color = bgInnerColor;
		setting.transform.Find ("RemainderA").GetComponent<Image> ().color = hlColor;
		setting.transform.Find ("RemainderB").GetComponent<Image> ().color = hlColor;
		setting.transform.Find ("RemainderA").Find ("Remainder").GetComponent<Text> ().color = txtColor;
		setting.transform.Find ("RemainderB").Find ("Remainder").GetComponent<Text> ().color = txtColor;

		// Leather
		GameObject[] leatherObjs = GameObject.FindGameObjectsWithTag ("Leather");

		if (leather == Status.Yes) {
			foreach (GameObject obj in leatherObjs)
				obj.SetActive (true);
		} else if (leather == Status.No) {
			foreach (GameObject obj in leatherObjs)
				obj.SetActive (false);
		}

		// Triangle Line
		GameObject[] triangleLineObjs = GameObject.FindGameObjectsWithTag ("TriangleLine");

		if (triangleLine == Status.Yes) {
			foreach (GameObject obj in triangleLineObjs)
				obj.SetActive (true);
		} else if (leather == Status.No) {
			foreach (GameObject obj in triangleLineObjs)
				obj.SetActive (false);
		}

		// Line Color
		var lines = GameObject.FindGameObjectsWithTag ("Line");

		foreach (var line in lines) {
			if (line.name != "Line_userAKick" && line.name != "Line_userBKick") {
				var index = line.transform.GetSiblingIndex () + 1;

				if (line.name != "Line_0" && line.name != "Line_25") {
					var bT = line.transform.Find ("Number").Find ("BackgroundText");
					var image = bT.GetComponent<Image> ();
					var text = bT.Find (line.name + "Text").GetComponent<Text> ();
					var hoverText = bT.Find (line.name + "Text").Find ("Hover").GetComponent<Image> ();

					image.color = index % 2 == 1 ? hlDarkColor : hlColor;
					text.color = txtColor;
					hoverText.color = hoverColor;
				}

				var triangle = line.transform.Find ("Body").Find ("Triangle").GetComponent<Image> ();
				var hover = line.transform.Find ("Body").Find ("Triangle").Find ("Hover").GetComponent<Image> ();

				if (line.name != "Line_0" && line.name != "Line_25") {
					var triangleLine = line.transform.Find ("Body").Find ("Triangle").Find ("TriangleLine").GetComponent<Image> ();

					triangle.color = index % 2 == 1 ? whiteLineColor : blackLineColor;
					triangleLine.color = index % 2 == 1 ? blackLineColor : whiteLineColor;
				} else {
					triangle.color = hlDarkColor;
				}

				hover.color = hoverColor;
			}
		}
	}

	// get Hover Color
	public Color GetHoverColor(){
		return hoverColor;
	}

	// Change Position Layout
	public void changePositionLayout(){
		userLayout.transform.SetSiblingIndex (1);
		gameLayout.transform.Find("LineLayout").Find("Table").transform.SetSiblingIndex (1);
		gameLayout.transform.Find("NutsLayout").Find("DiceLayout").Find("DiceNut").transform.SetSiblingIndex (1);
		gameLayout.transform.Find("SelectLayout").Find("Table").transform.SetSiblingIndex (1);
	}

	// Change Position Layout Language
	public void changePositionLayoutLanguage(){
		userA.transform.Find ("LevelParent").Find ("Level").transform.SetSiblingIndex (0);
		userB.transform.Find ("LevelParent").Find ("Level").transform.SetSiblingIndex (0);
		userA.transform.Find ("ScoreParent").Find ("Score").transform.SetSiblingIndex (0);
		userB.transform.Find ("ScoreParent").Find ("Score").transform.SetSiblingIndex (0);
	}

	// Set Users Value
	public void SetUsersValue() {
		// Value Set
		if (gameManager.offline) {
			userA.transform.Find ("LevelParent").Find ("Level").GetComponent<Text> ().text = "";
			userA.transform.Find ("LevelParent").Find ("LevelText").GetComponent<Text> ().text = "";
			userB.transform.Find ("LevelParent").Find ("Level").GetComponent<Text> ().text = "";
			userB.transform.Find ("LevelParent").Find ("LevelText").GetComponent<Text> ().text = "";
		} else {
			userA.transform.Find ("LevelParent").Find ("Level").GetComponent<Text> ().text = gameManager.levelA.ToString ();
			userB.transform.Find ("LevelParent").Find ("Level").GetComponent<Text> ().text = gameManager.levelB.ToString ();
		}

		if (gameManager.offline) {
			userA.transform.Find ("ScoreParent").Find ("Score").GetComponent<Text> ().text = gameManager.scoreA.ToString ();
			userB.transform.Find ("ScoreParent").Find ("Score").GetComponent<Text> ().text = gameManager.scoreB.ToString ();
		} else {
			userA.transform.Find ("ScoreParent").Find ("Score").GetComponent<Text> ().text = "";
			userA.transform.Find ("ScoreParent").Find ("ScoreText").GetComponent<Text> ().text = "";
			userB.transform.Find ("ScoreParent").Find ("Score").GetComponent<Text> ().text = "";
			userB.transform.Find ("ScoreParent").Find ("ScoreText").GetComponent<Text> ().text = "";
		}
	}

	// Rename Layout
	public void renameLayout(string tagName){
		var objs = GameObject.FindGameObjectsWithTag (tagName);

		foreach(var obj in objs){
			var index = obj.transform.GetSiblingIndex () + 1;
			string indexName = "";

			if (gameManager.color == "white") {
				if (obj.transform.parent.name == "Top") {
					if (obj.transform.parent.parent.name == "Table") {
						if(index == 7)
							indexName = "userBKick";
						else
							index = 12 + index + (index > 7 ? -1 : 0);
					} else if (obj.transform.parent.parent.name == "Right")
						index = 18 + index;
					else if (obj.transform.parent.parent.name == "Left")
						index = 12 + index;
				} else if (obj.transform.parent.name == "Bottom") {
					if (obj.transform.parent.parent.name == "Table") {
						if(index == 7)
							indexName = "userAKick";
						else
							index = 13 - index + (index > 7 ? 1 : 0);
					} else if (obj.transform.parent.parent.name == "Right")
						index = 7 - index;
					else if (obj.transform.parent.parent.name == "Left")
						index = 13 - index;
				} else if (obj.transform.parent.name == "Middle") {
					if(obj.transform.name == "Top")
						indexName = "userBKick";
					else
						indexName = "userAKick";
				} else if (obj.transform.parent.name == "Home") {
					if(obj.transform.parent.parent.name == "LineLayout")
						indexName = index == 2 ? "25" : "0";
					else
						indexName = index == 1 ? "25" : "0";
				}
			} else {
				if (obj.transform.parent.name == "Top") {
					if (obj.transform.parent.parent.name == "Table") {
						if(index == 7)
							indexName = "userAKick";
						else
							index = 25 - index + (index > 7 ? 1 : 0);
					} else if (obj.transform.parent.parent.name == "Right")
						index = 19 - index;
					else if (obj.transform.parent.parent.name == "Left")
						index = 25 - index;
				} else if(obj.transform.parent.name == "Bottom"){
					if (obj.transform.parent.parent.name == "Table") {
						if(index == 7)
							indexName = "userBKick";
						else
							index = index + (index > 7 ? -1 : 0);
					} else if (obj.transform.parent.parent.name == "Right")
						index = 6 + index;
					else if (obj.transform.parent.parent.name == "Left")
						index = index;
				} else if (obj.transform.parent.name == "Middle") {
					if(obj.transform.name == "Top")
						indexName = "userAKick";
					else
						indexName = "userBKick";
				} else if (obj.transform.parent.name == "Home") {
					if(obj.transform.parent.parent.name == "LineLayout")
						indexName = index == 2 ? "0" : "25";
					else
						indexName = index == 1 ? "0" : "25";
				}
			}

			// Change Name
			obj.name = tagName + "_" + (indexName != "" ? indexName : index.ToString());

			// Change Text
			if(tagName == "Line" && indexName == ""){
				var text = obj.transform.Find ("Number").Find ("BackgroundText").GetChild(0);
				text.name = obj.name + "Text";
				text.GetComponent<Text> ().text = index.ToString();
			}
		}

		// Translate Language
		gameManager.gameControl.translateLanguage = true;
	}

	// PLay Timeout
	public IEnumerator PlayTimeOut(float delayInput){
		yield return new WaitForSeconds(delayInput);
	
		if (gameManager.turn == 1) {
			var timeOutTextObject = userA.transform.Find ("Profile").Find ("ProfileLayout").Find ("ImageParent").Find ("Image").Find ("Second");
			timeOut = userA.transform.Find ("Profile").Find ("ProfileLayout").Find ("TimeOut").GetComponent<Image> ();
			timeOutText = timeOutTextObject.GetComponent<Text> ();
	
			timeOut.fillAmount = 1f;
			timeOutTextObject.gameObject.SetActive (true);
			userB.transform.Find ("Profile").Find ("ProfileLayout").Find ("TimeOut").GetComponent<Image> ().fillAmount = 0f;
			userB.transform.Find ("Profile").Find ("ProfileLayout").Find ("ImageParent").Find ("Image").Find ("Second").gameObject.SetActive (false);
		} else {
			var timeOutTextObject = userB.transform.Find ("Profile").Find ("ProfileLayout").Find ("ImageParent").Find ("Image").Find ("Second");
			timeOut = userB.transform.Find ("Profile").Find ("ProfileLayout").Find ("TimeOut").GetComponent<Image> ();
			timeOutText = timeOutTextObject.GetComponent<Text> ();

			timeOut.fillAmount = 1f;
			timeOutTextObject.gameObject.SetActive (true);
			userA.transform.Find ("Profile").Find ("ProfileLayout").Find ("TimeOut").GetComponent<Image> ().fillAmount = 0f;
			userA.transform.Find ("Profile").Find ("ProfileLayout").Find ("ImageParent").Find ("Image").Find ("Second").gameObject.SetActive (false);
		}

		playTimeOutStatus = true;
		timeOutText.text = "";
		delay = 0f;
		timeElapsed = gameManager.maxDelay + gameManager.maxDelayOpportunity;
		gameManager.gameControl.translateLanguage = true;
	}

	// Disable Play TimeOut
	public void DisablePlayTimeOut() {
		playTimeOutStatus = false;
		timeOut.fillAmount = 0f;
		timeOutText.text = "";
	}
}