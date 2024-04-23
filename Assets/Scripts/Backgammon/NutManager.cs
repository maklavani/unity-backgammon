using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NutManager : MonoBehaviour {
	private GameManager gameManager;
	private ThemeManager themeManager;

	public Camera cameraObject;
	public GameObject nutHomeObject;
	public GameObject dice;
	public GameObject gameButton;
	public GameObject lineObject;
	private float aspect;
	private float aspectSize;

	private GameObject diceLayoutRight;
	private GameObject diceLayoutLeft;

	// Nuts
	private bool initPrefabs = false;
	private GameObject nutSelected;
	private string nutSelectedLine;
	private string nutSelectedColor;
	[HideInInspector]
	public bool initNewNut = false;
	[HideInInspector]
	public bool checkNutPosition = false;
	private bool initSizePosition = false;

	// Dice
	[HideInInspector]
	public List<TempMoveData> tempMoves = new List<TempMoveData> ();
	private GameObject diceA;
	private GameObject diceB;
	private GameObject diceC;
	private GameObject diceD;

	// Button
	private GameObject ok;
	private GameObject back;

	// Animation
	private bool nutAnimate = false;
	private List<int> diceMoved = new List<int> ();
	private List<AnimationData> animations = new List<AnimationData>();
	[HideInInspector]
	public bool setInitAfterAnimate = false;

	// Awake
	void Awake(){
		aspect = (float)(cameraObject.aspect / 1.6f);
		gameManager = GetComponent<GameManager> ();
		themeManager = GetComponent<ThemeManager> ();
		aspect = (float)(cameraObject.aspect / 1.6f);
		diceLayoutRight = GameObject.Find ("DiceLayoutRight");
		diceLayoutLeft = GameObject.Find ("DiceLayoutLeft");

		if (!gameManager.gameEnded) {
			SetSize ();
			SetPosition ();
		}
	}

	// Update
	void Update(){
		if (!gameManager.gameEnded && initPrefabs && !initSizePosition) {
			SetSize ();
			SetPosition ();
			initSizePosition = true;
		}

		if (animations.Count > 0) {
			for (var i = 0; i < animations.Count; i++) {
				AnimationData animation = animations [i];

				if (animation.start) {
					nutAnimate = true;
					var rt = animation.go.GetComponent<RectTransform> ();
					animation.go.transform.position = Vector3.MoveTowards (animation.go.transform.position, GetTargetPosition (animation.target, rt.rect.height, animation.color, animation.move.move), animation.delay);

					// End Animation
					if (V3Equal (animation.go.transform.position, GetTargetPosition (animation.target, rt.rect.height, animation.color, animation.move.move))) {
						nutAnimate = false;
						bool setNextAnimate = false;
						string prevNutName = "";
						string nextNutName = "";
						var no = animation.go.GetComponent<NutObject> ();

						// Opponent Kicked
						if (animation.move.move != 0) {
							// Kicked Nut Animate
							List<NutData> nutDataChildren = new List<NutData> (gameManager.tableGameArray [animation.move.end]);

							// Change Table
							no.line = animation.move.end;
							no.index = ChangeTable (no.index, animation.move, true);

							// Start Next Animation
							for (var j = i + 1; j < animations.Count; j++) {
								if (!animations [j].start) {
									setNextAnimate = true;
									animations [j].start = true;
									prevNutName = animation.go.name;
									nextNutName = animations [j].go.name;

									// Play Shound
									gameManager.soundsManager.EnableSound ("Move", false);
									break;
								}
							}

							// Check Kicked Nut Animate
							if (nutDataChildren.Count > 0 && nutDataChildren [0].color != no.color) {
								GameObject nutChild = GameObject.Find ("Nut_" + nutDataChildren [0].color + "_" + nutDataChildren [0].number);
								nutChild.transform.SetSiblingIndex (30);

								// Set No
								var noChild = nutChild.GetComponent<NutObject> ();
								noChild.line = noChild.color == "white" ? "userBKick" : "userAKick";
								noChild.index = gameManager.tableGameArray [noChild.line].Count - 1;
								noChild.canMove = false;

								// Set Animation Data
								AnimationData animate = new AnimationData ();
								animate.go = nutChild;
								animate.color = noChild.color;
								animate.start = true;
								animate.target = noChild.line;
								animate.move = new SingleMoveData ();
								animate.move.start = noChild.line;
								animate.move.end = animate.target;
								animate.move.move = 0;
								animate.delay = Time.deltaTime * 400;

								// Add To Animations
								animations.Add (animate);

								// Play Shound
								gameManager.soundsManager.EnableSound ("Crack", false);
							}
						}

						if (!setNextAnimate || prevNutName != nextNutName)
							no.canMove = true;

						// Remove Animation
						animations.RemoveAt (i);

						// Show Device Move
						if ((gameManager.turn == gameManager.myTurn || gameManager.offline) && animations.Count == 0)
							ShowDiceMove ();

						// Set Position
						if (animation.move.end == "0" || animation.move.end == "25") {
							SetSize ();
							SetPosition ();
						}
					}
				}
			}

			if (animations.Count == 0) {
				// Init New Nut
				if (initNewNut) {
					SetSize ();
					SetPosition ();
					initNewNut = false;
				}

				if (checkNutPosition && !gameManager.offline) {
					CheckNutObjectPosition ();
					checkNutPosition = false;
				}
			}
		} else if (setInitAfterAnimate && gameManager.initMovement) {
			gameManager.initMovement = false;
			setInitAfterAnimate = false;
		} else if (gameManager.autoplay) {
			gameManager.autoplay = false;
		}
	}

	// Init Nuts
	public void InitNuts(bool initStatus = true){
		// Destroy
		if (gameManager.nutList.Count > 0) {
			foreach (var nut in gameManager.nutList) {
				Destroy (nut);
				Destroy (GameObject.Find (nut.name + "_Home"));
			}

			gameManager.nutList = new List<GameObject> ();
		}

		if (initStatus)
			StartCoroutine (InitializeGameObject ());
	}

	// Initialize GameObject
	private IEnumerator InitializeGameObject(){
		int whiteNumber = 1;
		int blackNumber = 1;

		foreach (var tableItem in gameManager.tableGameArray) {
			var tableKey = tableItem.Key;
			List<NutData> nutData = gameManager.tableGameArray[tableKey];

			if (nutData.Count > 0) {
				int index = 0;
	
				foreach(NutData nut in nutData){
					GameObject go = null;
					string goName ="Nut_" + nut.color + "_";

					if (nut.color == "white") {
						go = Instantiate (themeManager.nutObject, GameObject.Find ("NutsLayout").transform);
						goName += whiteNumber;
						whiteNumber++;

						// Set Color
						if (themeManager.nutType == Nuts.Nut) {
							go.transform.Find ("Background").GetComponent<Image> ().color = themeManager.whiteNutColor;
							go.transform.Find ("Image").GetComponent<Image> ().color = themeManager.whiteNutColor;
						} else if (themeManager.nutType == Nuts.NutB) {
							go.transform.Find ("BackgroundWhite").gameObject.SetActive (true);
							go.transform.Find ("BackgroundBlack").gameObject.SetActive (false);
						}
					} else {
						go = Instantiate (themeManager.nutObject, GameObject.Find ("NutsLayout").transform);
						goName += blackNumber;
						blackNumber++;

						// Set Color
						if (themeManager.nutType == Nuts.Nut) {
							go.transform.Find ("Background").GetComponent<Image> ().color = themeManager.blackNutColor;
							go.transform.Find ("Image").GetComponent<Image> ().color = themeManager.blackNutColor;
						} else if (themeManager.nutType == Nuts.NutB) {
							go.transform.Find ("BackgroundWhite").gameObject.SetActive (false);
							go.transform.Find ("BackgroundBlack").gameObject.SetActive (true);
						}
					}

					go.name = goName;

					// Set Value for Nut Object Class
					var no = go.GetComponent<NutObject> ();
					no.color = nut.color;
					no.line = nut.line;
					no.number = nut.number;
					no.index = index;
					no.canMove = true;
					no.truePosition = true;

					// Hover
					var hover = go.transform.Find("Image").Find("Hover").GetComponent<Image> ();
					hover.color = themeManager.GetHoverColor ();

					// Button
					// Add Event Listener
					Button btn = go.transform.Find("Image").Find("Hover").GetComponent<Button> ();
					btn.onClick.AddListener (delegate { OnClick (btn); });

					// Update Index
					index++;

					// Add to Nut list
					gameManager.nutList.Add(go);
				}
			}
		}

		initPrefabs = true;

		// Set Size and Position
		yield return new WaitForSeconds(0.1f);

		SetSize ();
		SetPosition ();
	}

	// Set Size
	public void SetSize(){
		if (initPrefabs) {
			var nutsObject = GameObject.FindGameObjectsWithTag ("Nut");
			var bodyObject = lineObject.transform.Find ("Body").GetComponent<RectTransform> ();
			var triangleRt = lineObject.transform.Find ("Body").Find("Triangle").GetComponent<RectTransform> ();

			aspectSize = 1;
			if (triangleRt.sizeDelta.x * 5 > bodyObject.rect.height)
				aspectSize = bodyObject.rect.height / (triangleRt.sizeDelta.x * 5);

			foreach (var nut in nutsObject) {
				var rt = nut.GetComponent<RectTransform> ();

				if (rt.sizeDelta.x != triangleRt.sizeDelta.x) {
					rt.sizeDelta = new Vector2(triangleRt.sizeDelta.x , triangleRt.sizeDelta.x);
				}
			}
		}
	}

	// Set Position
	public void SetPosition(){
		if (initPrefabs) {
			foreach(var go in gameManager.nutList){
				if (go.activeSelf) {
					var rt = go.GetComponent<RectTransform> ();
					var no = go.GetComponent<NutObject> ();

					if (no.canMove) {
						string line = no.line;

						if (line == "0" || line == "25") {
							go.SetActive (false);
							var homeNuts = GameObject.Find ("Line_" + line).transform.Find ("Body").Find ("Triangle").Find ("Nuts");
							GameObject nut = Instantiate (nutHomeObject, homeNuts.transform);
							nut.GetComponent<Image> ().color = line == "0" ? themeManager.whiteNutColor : themeManager.blackNutColor;
							nut.name = "Nut_" + no.color + "_" + no.number + "_Home";
							go.name = "Nut_In_Home";
						} else {
							string lineName = "";

							if (line == "userAKick" || line == "userBKick")
								lineName = "Line_" + line;
							else if (gameManager.color == "black")
								line = (25 - int.Parse (line)).ToString ();

							// Set Line Name if empty
							if (lineName == "")
								lineName = "Line_" + line;

							var positionLayout = GetPositionOfLayout (line);
							Vector2 pos = GetPositionOfLine (lineName, positionLayout);

							// Index
							var defaultY = (positionLayout == "top" ? -1 : 1) * rt.rect.height * aspectSize * aspect / 3.26f;

							if (no.index < 5)
								pos.y += defaultY * (no.index + 0.5f);
							else if (no.index < 9)
								pos.y += defaultY * (Mathf.Abs (no.index - 9));
							else if (no.index < 12)
								pos.y += defaultY * (Mathf.Abs (no.index - 12) + 1f);
							else if (no.index < 14)
								pos.y += defaultY * (Mathf.Abs (no.index - 14) + 1.5f);
							else
								pos.y += defaultY * (Mathf.Abs (no.index - 15) + 2.5f);

							// Update Information
							rt.transform.position = new Vector3 (pos.x, pos.y, rt.transform.position.z);
							no.position = rt.position;
						}
					}
				}
			}
		}
	}

	// Get Position Of Layout
	private string GetPositionOfLayout(string line) {
		List<string> topLayout = new List<string>() {"24" , "23" , "22" , "21" , "20" , "19" , "18" , "17" , "16" , "15" , "14" , "13"};
		List<string> bottomLayout = new List<string>() {"12" , "11" , "10" , "9" , "8" , "7" , "6" , "5" , "4" , "3" , "2" , "1"};

		if ((gameManager.color == "white" && line == "0") || (gameManager.color == "black" && line == "25"))
			return "bottom";
		if ((gameManager.color == "white" && line == "25") || (gameManager.color == "black" && line == "0"))
			return "top";

		if (topLayout.Contains (line))
			return "top";
		else if (bottomLayout.Contains (line))
			return "bottom";
		else if ((gameManager.color == "white" && line == "userAKick") ||
		        (gameManager.color == "black" && line == "userBKick"))
			return "bottom";
		else
			return "top";
	}

	// Get Position of Line
	private Vector2 GetPositionOfLine(string line , string positionLayout){
		Vector2 pos = Vector3.zero;
		GameObject triangle = GameObject.Find (line);
		var triangleRT = triangle.GetComponent<RectTransform> ();
		var trianglePos = triangleRT.position;

		pos.x = trianglePos.x;
		//pos.y = trianglePos.y + (positionLayout == "top" ? 1 : -1) * (triangleRT.rect.height * aspect / 2);
		pos.y = trianglePos.y;

		return pos;
	}

	// Animate Dice
	public IEnumerator AnimateDice(){
		yield return new WaitForSeconds(1f);

		// Reset Information
		nutSelected = null;
		nutSelectedLine = "";
		nutSelectedColor = "";
		tempMoves = new List<TempMoveData> ();
		diceMoved = new List<int> ();

		// Destroy
		if (diceA != null)
			Destroy (diceA);
		if (diceB != null)
			Destroy (diceB);
		if (diceC != null)
			Destroy (diceC);
		if (diceD != null)
			Destroy (diceD);

		// Check First Show
		if (gameManager.turns == 0) {
			diceA = Instantiate (dice, diceLayoutRight.transform);
			diceB = Instantiate (dice, diceLayoutLeft.transform);
		} else if (	(gameManager.color == "white" && (gameManager.turn == gameManager.myTurn || gameManager.offline)) ||
					(gameManager.color == "black" && gameManager.turn != gameManager.myTurn)) {
			diceA = Instantiate (dice, diceLayoutRight.transform);
			diceB = Instantiate (dice, diceLayoutRight.transform);
		} else {
			diceA = Instantiate (dice, diceLayoutLeft.transform);
			diceB = Instantiate (dice, diceLayoutLeft.transform);
		}

		// Set Name Of Dice
		diceA.name = "diceA";
		diceB.name = "diceB";

		// Play Shound
		gameManager.soundsManager.EnableSound("Dice" , false);

		yield return new WaitForSeconds(1f);
			
		Animator diceAAnimator = diceA.GetComponent<Animator> ();
		Animator diceBAnimator = diceB.GetComponent<Animator> ();
		diceAAnimator.SetInteger ("Number" , gameManager.diceA);
		diceBAnimator.SetInteger ("Number" , gameManager.diceB);

		// Namayesh Ki aval bazi mikone
		if(gameManager.turns == 0 && !gameManager.tv && !gameManager.offline){
			if (gameManager.myTurn == gameManager.turn)
				StartCoroutine (gameManager.ShowPopup ("_YOU_TURN" , 1f , 2f));
			else
				StartCoroutine (gameManager.ShowPopup ("_OPPONENT_TURN" , 1f , 2f));

			yield return new WaitForSeconds(1f);

			if ((gameManager.color == "white" && gameManager.myTurn == gameManager.turn) || 
				(gameManager.color == "black" && gameManager.myTurn != gameManager.turn)) {
				Destroy (diceB);
				diceB = Instantiate (dice, diceLayoutRight.transform);
				diceBAnimator = diceB.GetComponent<Animator> ();
				diceBAnimator.SetInteger ("Number", gameManager.diceB);
				diceB.name = "diceB";
			} else if (	(gameManager.color == "white" && gameManager.myTurn != gameManager.turn) || 
						(gameManager.color == "black" && gameManager.myTurn == gameManager.turn)) {
				Destroy (diceA);
				diceA = Instantiate (dice, diceLayoutLeft.transform);
				diceAAnimator = diceA.GetComponent<Animator> ();
				diceAAnimator.SetInteger ("Number", gameManager.diceA);
				diceA.name = "diceA";
			}
		}

		// agar baham barabar budand
		if(gameManager.diceA == gameManager.diceB){
			if ((gameManager.color == "white" && (gameManager.myTurn == gameManager.turn || gameManager.offline)) ||
				(gameManager.color == "black" && gameManager.myTurn != gameManager.turn)) {
				diceC = Instantiate (dice, diceLayoutRight.transform);
				diceD = Instantiate (dice, diceLayoutRight.transform);
			} else {
				diceC = Instantiate (dice, diceLayoutLeft.transform);
				diceD = Instantiate (dice, diceLayoutLeft.transform);
			}

			Animator diceCAnimator = diceC.GetComponent<Animator> ();
			diceCAnimator.SetInteger ("Number" , gameManager.diceA);
			Animator diceDAnimator = diceD.GetComponent<Animator> ();
			diceDAnimator.SetInteger ("Number" , gameManager.diceA);

			diceC.name = "diceC";
			diceD.name = "diceD";
		}

		// Add Button
		if (ok != null)
			Destroy (ok);
		if (back != null)
			Destroy (back);

		if ((gameManager.color == "white" && (gameManager.myTurn == gameManager.turn || gameManager.offline)) ||
		    (gameManager.color == "black" && gameManager.myTurn != gameManager.turn)) {
			ok = Instantiate (gameButton, diceLayoutRight.transform);
			back = Instantiate (gameButton, diceLayoutRight.transform);
		} else {
			ok = Instantiate (gameButton, diceLayoutLeft.transform);
			back = Instantiate (gameButton, diceLayoutLeft.transform);
		}

		// Set Image of Button
		ok.GetComponent<Image> ().color = new Color (0.08f , 0.58f , 0.21f , 1f);
		back.GetComponent<Image> ().color = new Color (0.72f , 0.1f , 0.1f , 1f);
		// Set Text of Button
		ok.transform.Find("Text").GetComponent<Text> ().text = "_OK";
		back.transform.Find("Text").GetComponent<Text> ().text = "_BACK";
		// Set Icon of Button
		ok.transform.Find("Icon").name = "okIcon";
		ok.transform.Find("okIcon").GetComponent<Text> ().text = "10";
		back.transform.Find("Icon").name = "backIcon";
		back.transform.Find("backIcon").GetComponent<Text> ().text = "12";
		// Set Button Fucntion
		Button okBtn = ok.GetComponent<Button> ();
		okBtn.onClick.AddListener (delegate { OnClickOk (okBtn); });
		Button backBtn = back.GetComponent<Button> ();
		backBtn.onClick.AddListener (delegate { OnClickBack (backBtn); });
		// Set Name of Button
		ok.name = "ok";
		back.name = "back";
		ok.SetActive (false);
		back.SetActive (false);

		// Show Possible Move
		// Create All Moves
		List<int> moves = new List<int> ();
		moves.Add (gameManager.diceA);
		moves.Add (gameManager.diceB);

		string color = "white";

		if (gameManager.offline) {
			color = gameManager.turn % 2 == 1 ? "white" : "black";
		} else if ((gameManager.color == "white" && (gameManager.myTurn != gameManager.turn || gameManager.offline)) ||
			(gameManager.color == "black" && gameManager.myTurn == gameManager.turn))
			color = "black";

		if (gameManager.myTurn == gameManager.turn || gameManager.offline)
			gameManager.ShowPosibleMove(color , moves);

		// Change Active
		ChangeActiveOfDiceAndButton ();
	}

	// Animate Nuts Move
	public void AnimateNutsMove(){
		gameManager.maxMove = gameManager.lastMovementArray.Count;

		if (gameManager.maxMove > 0) {
			gameManager.turns = gameManager.gameControl.turns;
			AnimateNutsWithArray (gameManager.lastMovementArray);
			setInitAfterAnimate = true;
		} else {
			gameManager.againRequest = true;
			gameManager.requestManager.EnableRequest ("Get Movement");
		}
	}

	// Show Dice For Move
	public void ShowDiceMove() {
		if (!nutAnimate && (gameManager.turn == gameManager.myTurn || gameManager.offline) && !gameManager.autoplay) {
			int tempMovesCount = tempMoves.Count;
			int diceMovedCount = diceMoved.Count;
			ResetToDefaultView ();

			// Show Dice For Move
			if(gameManager.posibleMoves.Count > 0)
				foreach (var posibleMove in gameManager.posibleMoves) {
					if (posibleMove.moves.Count == gameManager.maxMove && posibleMove.moves.Count > diceMovedCount) {
						bool findMove = false;
						int increaseIndexTemp = 0;

						if (diceMovedCount > 0) {
							findMove = true;

							for (var i = 0; i < diceMovedCount; i++) {
								if (tempMoves [i].move.move == 0)
									increaseIndexTemp++;

								if (i + increaseIndexTemp >= tempMovesCount ||
									posibleMove.moves [i].start != tempMoves [i + increaseIndexTemp].move.start ||
								   	posibleMove.moves [i].end != tempMoves [i + increaseIndexTemp].move.end ||
									posibleMove.moves [i].move != tempMoves [i + increaseIndexTemp].move.move) {
									findMove = false;
									break;
								}
							}
						} else {
							findMove = true;
						}

						if (findMove) {
							string name = GetTopNutInLine(posibleMove.moves[diceMovedCount].start);

							if (name != "" && GameObject.Find (name)) {
								GameObject.Find (name).transform.Find ("Image").Find ("Hover").gameObject.SetActive (true);
								GameObject.Find (name).transform.SetSiblingIndex (30);
							}
						}
					}
				}
		}
	}

	// Get Top Nut In Line
	private string GetTopNutInLine(string line){
		foreach (var tableItem in gameManager.tableGameArray) {
			var tableKey = tableItem.Key;
			List<NutData> nutData = gameManager.tableGameArray [tableKey];

			if (nutData.Count > 0 && line == tableKey) {
				string txt = "";
				foreach (NutData nuta in nutData) {
					txt += nuta.number + "-";
				}

				NutData nut = nutData [nutData.Count - 1];
				return "Nut_" + nut.color + "_" + nut.number;
			}
		}
	
		return "";
	}

	// On Click
	public void OnClick(Button btn){
		if (!nutAnimate) {
			GameObject btnObject = btn.gameObject;
			Transform nut = btnObject.transform.parent.parent;
			nutSelected = nut.gameObject;

			// Reset to Default
			var objs = GameObject.FindGameObjectsWithTag ("Nut");

			foreach (var obj in objs) {
				if (obj.name != nut.name)
					obj.transform.Find ("Image").Find ("Hover").gameObject.SetActive (false);
			}

			var no = nut.GetComponent<NutObject> ();
			nutSelectedLine = no.line;
			nutSelectedColor = no.color;
			ShowLineClick (no.line);
			SetSize ();
			SetPosition ();
		}
	}

	// On Click Line
	public void OnClickLine(Button btn){
		if (!nutAnimate) {
			GameObject btnObject = btn.gameObject;
			Transform selectParent = btnObject.transform.parent;
			string name = selectParent.gameObject.name;
			string line = name.Replace ("Select_", "");

			if (line != "0" && line != "25" && gameManager.color == "black")
				line = (25 - int.Parse (line)).ToString ();

			// Find Path
			List<MoveData> pathFinded = new List<MoveData> (FindPath (diceMoved.Count, nutSelectedLine, line));
			MoveData bestPath = FindBestPath (pathFinded, diceMoved.Count, nutSelectedLine, line);

			// Animate Nuts
			AnimateNuts (bestPath, diceMoved.Count, nutSelectedLine, line);
			initNewNut = true;
		}
	}

	// On Click Back
	public void OnClickBack(Button btn){
		if (tempMoves.Count > 0) {
			// Rset Animation
			animations = new List<AnimationData>();
			// Reset Dice
			foreach(var nut in gameManager.nutList){
				var no = nut.GetComponent<NutObject> ();
				no.canMove = true;
			}

			if (tempMoves [tempMoves.Count - 1].move.move == 0 && tempMoves.Count > 1) {
				ChangeTableToPrev (tempMoves.Count - 2);
				ChangeTableToPrev (tempMoves.Count - 1);
				// Remove
				tempMoves.RemoveAt (tempMoves.Count - 1);
				tempMoves.RemoveAt (tempMoves.Count - 1);
			} else {
				ChangeTableToPrev (tempMoves.Count - 1);
				// Remove
				tempMoves.RemoveAt (tempMoves.Count - 1);
			}

			// Dice Removed			
			diceMoved.RemoveAt (diceMoved.Count - 1);

			// Show Dice Move
			ShowDiceMove ();
			ChangeActiveOfDiceAndButton ();
			SetSize ();
			SetPosition ();
		}
	}

	// On Click Ok
	public void OnClickOk(Button btn = null) {
		if (!gameManager.offline) {
			gameManager.sendData = "";

			if (tempMoves.Count > 0)
				foreach (var temp in tempMoves) {
					if (gameManager.sendData != "")
						gameManager.sendData += ";";
	
					if (temp.move.move != 0)
						gameManager.sendData += temp.move.start + "|" + temp.move.end + "|" + temp.move.move;
				}

			ok.SetActive (false);
			back.SetActive (false);

			// Check Position
			gameManager.requestManager.EnableRequest ("Send Data");
		} else if(!gameManager.gameEnded) {
			// Offline OK
			ok.SetActive (false);
			back.SetActive (false);

			gameManager.gameControl.turns += 1;
			gameManager.gameControl.turn = (gameManager.turn % 2) + 1;
			gameManager.gameControl.diceA = Random.Range (1 , 6);
			gameManager.gameControl.diceB = Random.Range (1 , 6);
			gameManager.gameControl.remainderA = gameManager.FindRemaiderMove(gameManager.tableGameArray , "white");
			gameManager.gameControl.remainderB = gameManager.FindRemaiderMove(gameManager.tableGameArray , "black");

			gameManager.turns = gameManager.gameControl.turns;
			gameManager.turn = gameManager.gameControl.turn;
			gameManager.diceA = gameManager.gameControl.diceA;
			gameManager.diceB = gameManager.gameControl.diceB;
			gameManager.remainderA.text = gameManager.gameControl.remainderA.ToString ();
			gameManager.remainderB.text = gameManager.gameControl.remainderB.ToString ();

			// Check Winner
			bool endGame = false;
			if (gameManager.gameControl.remainderA == 0) {
				endGame = true;
				gameManager.gameControl.turn = 1;

				if (new List<NutData> (gameManager.tableGameArray ["25"]).Count != 0)
					gameManager.gameControl.scoreA++;
				else
					gameManager.gameControl.scoreA += 2;

				gameManager.turn = 1;
			} else if (gameManager.gameControl.remainderB == 0) {
				endGame = true;
				gameManager.gameControl.turn = 2;

				if (new List<NutData> (gameManager.tableGameArray ["0"]).Count != 0)
					gameManager.gameControl.scoreB++;
				else
					gameManager.gameControl.scoreB += 2;
			
				gameManager.turn = 2;
			}

			if (endGame) {
				gameManager.gameControl.tableGame = "{\"userAKick\":{},\"userBKick\":{},\"0\":{},\"1\":{\"0\":2,\"1\":2},\"2\":{},\"3\":{},\"4\":{},\"5\":{},\"6\":{\"0\":1,\"1\":1,\"2\":1,\"3\":1,\"4\":1},\"7\":{},\"8\":{\"0\":1,\"1\":1,\"2\":1},\"9\":{},\"10\":{},\"11\":{},\"12\":{\"0\":2,\"1\":2,\"2\":2,\"3\":2,\"4\":2},\"13\":{\"0\":1,\"1\":1,\"2\":1,\"3\":1,\"4\":1},\"14\":{},\"15\":{},\"16\":{},\"17\":{\"0\":2,\"1\":2,\"2\":2},\"18\":{},\"19\":{\"0\":2,\"1\":2,\"2\":2,\"3\":2,\"4\":2},\"20\":{},\"21\":{},\"22\":{},\"23\":{},\"24\":{\"0\":1,\"1\":1},\"25\":{}}";
				gameManager.gameControl.remainderA = 167;
				gameManager.gameControl.remainderB = 167;

				if (gameManager.gameControl.scoreA >= gameManager.gameControl.rounds) {
					gameManager.gameControl.winner = 1;
					gameManager.gameControl.status = 2;
					gameManager.gameEnded = true;
					ok.SetActive (false);
					back.SetActive (false);
				} else if (gameManager.gameControl.scoreB >= gameManager.gameControl.rounds) {
					gameManager.gameControl.winner = 2;
					gameManager.gameControl.status = 2;
					gameManager.gameEnded = true;
					ok.SetActive (false);
					back.SetActive (false);
				}

				gameManager.tableGame = gameManager.gameControl.tableGame;
				gameManager.remainderA.text = gameManager.gameControl.remainderA.ToString ();
				gameManager.remainderB.text = gameManager.gameControl.remainderB.ToString ();
				InitNuts ();
			}

			if (!gameManager.gameEnded)
				StartCoroutine (AnimateDice ());
		}
	}
		
	// Change Table To Prev
	private void ChangeTableToPrev(int indexItem){
		string start = tempMoves [indexItem].move.start;
		string end = tempMoves [indexItem].move.end;
		tempMoves [indexItem].move.start = tempMoves [indexItem].move.end;
		tempMoves [indexItem].move.end = start;

		// Get Index
		int index = gameManager.tableGameArray [tempMoves [indexItem].move.start].Count - 1;


		// ChangeTable
		if (index != -1)
			index = ChangeTable (index, tempMoves [indexItem].move, false);
		else
			index = 0;

		// Set Nut Object
		string nutName = "Nut_" + tempMoves [indexItem].color + "_" + tempMoves [indexItem].number;
		GameObject nutHome = GameObject.Find (nutName + "_Home");

		if (nutHome != null) {
			Destroy (nutHome);

			foreach (var go in gameManager.nutList) {
				var noGo = go.GetComponent<NutObject> ();

				if (noGo.color == tempMoves [indexItem].color && noGo.number == tempMoves [indexItem].number) {
					go.name = nutName;
					go.SetActive (true);
				}
			}
		}

		var no = GameObject.Find (nutName).GetComponent<NutObject> ();
		no.index = index;
		no.line = tempMoves [indexItem].move.end;
	}

	// Show Line Can Click
	private void ShowLineClick(string line){
		// Reset to Default
		var objs = GameObject.FindGameObjectsWithTag ("Select");
		foreach (var obj in objs) {
			obj.transform.Find ("Button").gameObject.SetActive (false);
		}

		objs = GameObject.FindGameObjectsWithTag ("Line");
		foreach (var obj in objs) {
			if (obj.name != "Line_userAKick" && obj.name != "Line_userBKick") {
				if (obj.name != "Line_0" && obj.name != "Line_25")
					obj.transform.Find ("Number").Find ("BackgroundText").Find (obj.name + "Text").Find ("Hover").gameObject.SetActive (false);
				obj.transform.Find ("Body").Find ("Triangle").Find ("Hover").gameObject.SetActive (false);
			}
		}

		// Show Line
		List<string> canLines = LineCanClick(line);

		if (canLines.Count > 0)
			foreach (string canLine in canLines) {
				string lineNumber = canLine;

				if (lineNumber != "0" && lineNumber != "25") {
					if (gameManager.color == "black")
						lineNumber = (25 - int.Parse (lineNumber)).ToString ();

					GameObject.Find ("Line_" + lineNumber).transform.Find ("Number").Find ("BackgroundText").Find ("Line_" + lineNumber + "Text").Find ("Hover").gameObject.SetActive (true);
				}

				GameObject.Find ("Line_" + lineNumber).transform.Find("Body").Find("Triangle").Find("Hover").gameObject.SetActive (true);
				GameObject.Find ("Select_" + lineNumber).transform.Find("Button").gameObject.SetActive (true);
			}
	}

	// Line Can Click
	private List<string> LineCanClick(string line){
		List<int> moves = new List<int> ();
		if(!diceMoved.Contains(gameManager.diceA))
			moves.Add (gameManager.diceA);
		if(!diceMoved.Contains(gameManager.diceB))
			moves.Add (gameManager.diceB);

		if (gameManager.diceA == gameManager.diceB) {
			if(diceMoved.Count < 3)
				moves.Add (gameManager.diceA);
			if(diceMoved.Count < 4)
				moves.Add (gameManager.diceA);
		}

		List<string> listOutput = new List<string> ();
		int nutInUserKick = gameManager.NutsInUserKickNumber (gameManager.tableGameArray , nutSelectedColor);

		foreach (var listItem in gameManager.posibleMoves) {
			if (listItem.moves.Count > diceMoved.Count) {
				string firstLine = line;

				for (var i = diceMoved.Count; i < listItem.moves.Count; i++) {
					if (nutInUserKick < 2 || (listItem.moves [i].start == "userBKick" || listItem.moves [i].start == "userAKick")) {
						if (listItem.moves [i].start == firstLine && moves.Contains (listItem.moves [i].move)) {
							firstLine = listItem.moves [i].end;
							listOutput.Add (listItem.moves [i].end);
						} else {
							firstLine = "";
						}
					}
				}
			}
		}

		return listOutput;
	}

	// Find Path
	private List<MoveData> FindPath(int index , string start , string end){
		List<MoveData> path = new List<MoveData> ();

		foreach (var listItem in gameManager.posibleMoves) {
			if (listItem.moves.Count > index && listItem.moves[index].start == start) {
				string firstStart = start;
				bool existPath = true;

				if (index > 0) {
					var numberOfIgnore = 0;

					for (var i = 0; i < tempMoves.Count; i++) {
						if (tempMoves [i].move.move == 0) {
							numberOfIgnore++;
							continue;
						}
		
						if (tempMoves [i].move.start != listItem.moves [i - numberOfIgnore].start ||
							tempMoves [i].move.end != listItem.moves [i - numberOfIgnore].end ||
							tempMoves [i].move.move != listItem.moves [i - numberOfIgnore].move) {
							existPath = false;
							break;
						}
					}
				}

				if (existPath)
					for (var i = index; i < listItem.moves.Count; i++)
						if (firstStart == listItem.moves [i].start) {
							if (listItem.moves [i].end == end) {
								path.Add (listItem);
								break;
							} else {
								firstStart = listItem.moves [i].end;
							}
						}
			}
		}

		return path;
	}

	// Find Best Path
	private MoveData FindBestPath(List<MoveData> paths , int index , string start , string end){
		MoveData bestPath = paths[0];
		int maxHit = 0;
		int numberOfMoves = gameManager.maxMove;

		foreach (var path in paths) {
			if (path.moves.Count > index && path.moves [index].start == start) {
				for (var i = index; i < path.moves.Count; i++) {
					if (path.moves [i].end == end && path.numberOfHit >= maxHit && (i - index + 1) <= numberOfMoves) {
						maxHit = path.numberOfHit;
						numberOfMoves = i - index + 1;
						bestPath = path;
					}
				}
			}
		}

		return bestPath;
	}

	// AnimateNuts
	public void AnimateNuts(MoveData path , int index , string start , string end){
		bool firstMove = true;
		string firstLine = start;
		bool endStatus = false;

		var no = nutSelected.GetComponent<NutObject> ();
		// Set Over for Animate
		nutSelected.transform.SetSiblingIndex (30);

		for (var i = index; i < path.moves.Count; i++) {
			if (path.moves [i].start == firstLine && !endStatus) {
				// string startLine = path.moves [i].start;
				string endLine = path.moves [i].end;
				int move = path.moves [i].move;

				// Change Table
				no.canMove = false;

				// Set Animation Data
				AnimationData animate = new AnimationData ();
				animate.go = nutSelected;
				animate.color = no.color;
				animate.start = firstMove;
				animate.target = endLine;
				animate.move = path.moves [i];
				animate.delay = Time.deltaTime * ((6 / move) * 400);

				// Add To Animations
				animations.Add (animate);

				// Set Next
				if(endLine != end){
					firstMove = false;
					firstLine = endLine;
				}

				if (endLine == end)
					endStatus = true;

				// Set move of Dice
				diceMoved.Add (path.moves [i].move);
			}
		}

		// Play Shound
		if (diceMoved.Count > 0)
			gameManager.soundsManager.EnableSound ("Move" , false);
		ChangeActiveOfDiceAndButton ();
	}

	// Animate Nuts With Array
	public void AnimateNutsWithArray(List<SingleMoveData> list){
		bool firstMove = true;
		string endLine = "";
		NutData nut = new NutData ();
		List<string> startLine = new List<string> ();

		foreach (var listItem in list) {
			if (endLine == "" || listItem.start != endLine) {
				List<NutData> nutDataChildren = new List<NutData> (gameManager.tableGameArray [listItem.start]);
				int minusIndex = 1;

				if (startLine.Count > 0)
					foreach (var sLItem in startLine)
						if (sLItem == listItem.start)
							minusIndex++;

				int findIndex = nutDataChildren.Count - minusIndex;
				nut = nutDataChildren [findIndex];
				startLine.Add (listItem.start);
			}

			var go = GameObject.Find ("Nut_" + nut.color + "_" + nut.number);
			var no = go.GetComponent<NutObject> ();
			go.transform.SetSiblingIndex (30);

			// Change Table
			no.canMove = false;

			// Set Animation Data
			AnimationData animate = new AnimationData ();
			animate.go = go;
			animate.color = no.color;
			animate.start = firstMove;
			animate.target = listItem.end;
			animate.move = new SingleMoveData ();
			animate.move = listItem;
			animate.delay = Time.deltaTime * 400;

			// Add To Animations
			animations.Add (animate);

			if (firstMove)
				firstMove = false;
			endLine = listItem.end;

			// Set move of Dice
			diceMoved.Add (listItem.move);
		}

		// Play Shound
		if (diceMoved.Count > 0)
			gameManager.soundsManager.EnableSound ("Move" , false);

		// Destroy
		if (diceA != null)
			diceA.SetActive (false);
		if (diceB != null)
			diceB.SetActive (false);
		if (diceC != null)
			diceC.SetActive (false);
		if (diceD != null)
			diceD.SetActive (false);
	}

	// Check Nut Object Position
	public void CheckNutObjectPosition() {
		gameManager.ReadTableJSON ();

		// Reset
		foreach (var go in gameManager.nutList) {
			var no = go.GetComponent<NutObject> ();
			no.truePosition = false;
			go.name = "Nut_In_Home";
		}

		// Find Exist Nut Data
		List<NutData> notFound = new List<NutData>();
		List<string> found = new List<string>();

		foreach (var tableItem in gameManager.tableGameArray) {
			var tableKey = tableItem.Key;
			List<NutData> nutDataList = new List<NutData> (gameManager.tableGameArray [tableKey]);
			int index = 0;

			//if (tableKey != "0" && tableKey != "25")
			foreach (NutData nut in nutDataList) {
				// Find
				bool find = false;

				foreach (var go in gameManager.nutList) {
					var no = go.GetComponent<NutObject> ();

					if (nut.line == no.line && index == no.index && !found.Contains ("Nut_" + nut.color + "_" + nut.number)) {
						// Update Number
						no.number = nut.number;
						no.truePosition = true;
						go.name = "Nut_" + nut.color + "_" + nut.number;

						find = true;
						found.Add (go.name);
						break;
					}
				}

				if (!find) {
					notFound.Add (nut);
					Debug.Log ("Not Found:" + nut.line + "|" + index);
				}

				index++;
			}
		}

		if (notFound.Count > 0) {
			List<GameObject> nutObjectNotFound = new List<GameObject> ();

			// Get
			foreach (var go in gameManager.nutList) {
				var no = go.GetComponent<NutObject> ();
				if (!no.truePosition)
					nutObjectNotFound.Add (go.gameObject);
			}

			foreach (var nut in notFound) {
				foreach (var go in nutObjectNotFound) {
					var no = go.GetComponent<NutObject> ();

					if (nut.color == no.color && !no.truePosition) {
						int index = 0;

						foreach (NutData nutItem in gameManager.tableGameArray [nut.line]) {
							if (nutItem.number == nut.number)
								break;
							index++;
						}

						// Update Number
						no.number = nut.number;
						no.index = index;
						no.truePosition = true;
						go.name = "Nut_" + nut.color + "_" + nut.number;
						found.Add (go.name);
						Debug.Log ("Fixed:" + nut.line + "|" + index);
						break;
					}
				}
			}
		}

		SetSize ();
		SetPosition ();
	}

	// Change Active Of Dice And Button
	public void ChangeActiveOfDiceAndButton(){
		// Dices
		if (diceMoved.Count > 0) {
			if (diceMoved.Count == 1) {
				if (diceMoved [0] == gameManager.diceA) {
					diceA.SetActive (false);
					diceB.SetActive (true);
				} else {
					diceA.SetActive (true);
					diceB.SetActive (false);
				}
			} else {
				diceA.SetActive (false);
				diceB.SetActive (false);
			}

			if(gameManager.diceA == gameManager.diceB){
				// Dice C
				if (diceMoved.Count > 2)
					diceC.SetActive (false);
				else
					diceC.SetActive (true);

				// Dice D
				if (diceMoved.Count > 3)
					diceD.SetActive (false);
				else
					diceD.SetActive (true);
			}
		} else {
			diceA.SetActive (true);
			diceB.SetActive (true);

			if(gameManager.diceA == gameManager.diceB){
				diceC.SetActive (true);
				diceD.SetActive (true);
			}
		}

		if (!setInitAfterAnimate && !gameManager.autoplay) {
			if (diceMoved.Count > 0 && (gameManager.turn == gameManager.myTurn || gameManager.offline)) {
				back.SetActive (true);
			} else {
				back.SetActive (false);
			}

			if (diceMoved.Count == gameManager.maxMove && (gameManager.turn == gameManager.myTurn || gameManager.offline)) {
				if(diceMoved.Count > 0)
					ok.SetActive (true);

				// Set Active False
				if (diceMoved.Count > 0) {
					if (diceA != null)
						diceA.SetActive (false);
					if (diceB != null)
						diceB.SetActive (false);
					if (diceC != null)
						diceC.SetActive (false);
					if (diceD != null)
						diceD.SetActive (false);
				}
			} else {
				ok.SetActive (false);
			}

			// Translate Language
			gameManager.gameControl.translateLanguage = true;
		}
	}

	// Get Target Position
	public Vector2 GetTargetPosition(string line , float size , string color , int move){
		string lineNumber = line;

		if (lineNumber != "0" && lineNumber != "25" && lineNumber != "userAKick" && lineNumber != "userBKick" && gameManager.color == "black")
			lineNumber = (25 - int.Parse (lineNumber)).ToString ();
			
		var positionLayout = GetPositionOfLayout (lineNumber);
		Vector2 pos = GetPositionOfLine("Line_" + lineNumber , positionLayout);
		var defaultY = (positionLayout == "top" ? -1 : 1) * aspectSize * aspect / 3.26f;

		if (positionLayout == "top" && (lineNumber == "25" || lineNumber == "0"))
			defaultY *= -1;

		if(lineNumber == "0" || lineNumber == "25")
			defaultY *= (size / 2);
		else
			defaultY *= size;

		int index = 0;

		foreach (var tableItem in gameManager.tableGameArray) {
			var tableKey = tableItem.Key;

			if (tableKey == line) {
				List<NutData> nutData = new List<NutData> (gameManager.tableGameArray [tableKey]);
				int nutDataCount = nutData.Count;

				if (nutDataCount > 0 && color == nutData [0].color) {
					index = nutDataCount;
					if (move == 0)
						index--;
					break;
				}
			}
		}

		if (lineNumber == "0" || lineNumber == "25") {
			pos.y += defaultY * (index + 0.5f);
		} else {
			if (index < 5)
				pos.y += defaultY * (index + 0.5f);
			else if (index < 9)
				pos.y += defaultY * (Mathf.Abs(index - 9));
			else if (index < 12)
				pos.y += defaultY * (Mathf.Abs(index - 12) + 1f);
			else if (index < 14)
				pos.y += defaultY * (Mathf.Abs(index - 14) + 1.5f);
			else
				pos.y += defaultY * (Mathf.Abs(index - 15) + 2.5f);
		}

		return pos;
	}

	// ChangeTable
	public int ChangeTable(int index , SingleMoveData move , bool addToTemp = false){
		// Move Nut
		List<NutData> nutDataChildren = new List<NutData> (gameManager.tableGameArray [move.start]);
		int countNutDataChildren = nutDataChildren.Count;

		// Check Exist Index
		if (index < 0 || index >= countNutDataChildren)
			return 0;
		
		NutData nutItem = nutDataChildren [index];
		nutDataChildren.RemoveAt (index);
		gameManager.tableGameArray [move.start] = nutDataChildren;
		nutDataChildren = gameManager.tableGameArray [move.end];

		// Add Temp Move
		if (addToTemp) {
			TempMoveData tempMove = new TempMoveData ();
			tempMove.number = nutItem.number;
			tempMove.color = nutItem.color;
			tempMove.move = new SingleMoveData();
			tempMove.move.start = move.start;
			tempMove.move.end = move.end;
			tempMove.move.move = move.move;
			tempMoves.Add (tempMove);
		}

		// Agar Nuti dar an khane bud
		if (nutDataChildren.Count > 0) {
			string colorFind = nutDataChildren [0].color;

			// Hit
			if (colorFind != nutItem.color) {
				gameManager.tableGameArray [nutDataChildren [0].color == "white" ? "userBKick" : "userAKick"].Add (nutDataChildren [0]);

				// Add Temp Move
				if (addToTemp) {
					TempMoveData tempMoveChild = new TempMoveData ();
					tempMoveChild.number = nutDataChildren [0].number;
					tempMoveChild.color = nutDataChildren [0].color;
					tempMoveChild.move = new SingleMoveData ();
					tempMoveChild.move.start = move.end;
					tempMoveChild.move.end = nutDataChildren [0].color == "white" ? "userBKick" : "userAKick";
					tempMoveChild.move.move = 0;
					tempMoves.Add (tempMoveChild);
				}

				// Reset Nut Data
				gameManager.tableGameArray [move.end] = new List<NutData> ();
			}
		}

		gameManager.tableGameArray [move.end].Add (nutItem);
		return gameManager.tableGameArray [move.end].Count - 1;
	}

	// Reset To Default View
	public void ResetToDefaultView(){
		// Reset Nut
		var objs = GameObject.FindGameObjectsWithTag ("Nut");
		foreach (var obj in objs) 
			obj.transform.Find ("Image").Find ("Hover").gameObject.SetActive (false);

		// Reset Line
		objs = GameObject.FindGameObjectsWithTag ("Line");
		foreach (var obj in objs) {
			if (obj.name != "Line_userAKick" && obj.name != "Line_userBKick") {
				if (obj.name != "Line_0" && obj.name != "Line_25")
					obj.transform.Find ("Number").Find ("BackgroundText").Find (obj.name + "Text").Find ("Hover").gameObject.SetActive (false);
				obj.transform.Find ("Body").Find ("Triangle").Find ("Hover").gameObject.SetActive (false);
			}
		}

		// Reset Select
		objs = GameObject.FindGameObjectsWithTag ("Select");
		foreach (var obj in objs) 
			obj.transform.Find ("Button").gameObject.SetActive (false);

		// Reset Messages
		gameManager.messages.SetActive(false);
	}

	// Vector 3 Equal
	public bool V3Equal(Vector3 a , Vector3 b){
		return Vector3.SqrMagnitude (a - b) < 0.0001;
	}
}