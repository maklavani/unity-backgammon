using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MiniJSON;

public class GameManager : MonoBehaviour {
	[HideInInspector]
	public GameControl gameControl = null;
	[HideInInspector]
	public RequestManager requestManager;
	[HideInInspector]
	public SoundsManager soundsManager;
	[HideInInspector]
	public AnimationManager animationManager;
	[HideInInspector]
	public PopUpManager popUpManager;

	public GameObject loadingObject;
	public GameObject winnerPageObject;
	public GameObject messages;
	[HideInInspector]
	public List<GameObject> users;
	public Text usernameA;
	public Text usernameB;
	public Text remainderA;
	public Text remainderB;
	public Image faceA;
	public Image faceB;

	private ThemeManager themeManager;
	private NutManager nutManager;
	private MessageManager messageManager;
	private WinnerManager winnerManager;

	// Game Information
	//[HideInInspector]
	public string code;
	//[HideInInspector]
	public int diceA;
	//[HideInInspector]
	public int diceB;
	[HideInInspector]
	public int turns;
	[HideInInspector]
	public int turn;
	[HideInInspector]
	public string lastMovement;
	[HideInInspector]
	public string tableGame;
	[HideInInspector]
	public int scoreA;
	[HideInInspector]
	public int scoreB;
	[HideInInspector]
	public int levelA;
	[HideInInspector]
	public int levelB;
	[HideInInspector]
	public int messageA;
	[HideInInspector]
	public int messageB;
	[HideInInspector]
	public int maxDelay;
	[HideInInspector]
	public int maxDelayOpportunity;

	// Control
	[HideInInspector]
	public bool tv = false;
	[HideInInspector]
	public bool offline = false;
	[HideInInspector]
	public string color = "";
	[HideInInspector]
	public int myTurn = -1;

	[HideInInspector]
	public bool gameEnded = false;
	[HideInInspector]
	public bool init = false;
	[HideInInspector]
	public bool initMovement = false;
	[HideInInspector]
	public bool showAnimation = false;
	[HideInInspector]
	public bool againRequest = false;
	[HideInInspector]
	public bool autoplay = false;
	[HideInInspector]
	public int maxMove = 0;

	[HideInInspector]
	public List<MoveData> posibleMoves = new List<MoveData> ();
	[HideInInspector]
	public Dictionary<string , List<NutData>> tableGameArray = null;
	[HideInInspector]
	public List<SingleMoveData> lastMovementArray = null;
	[HideInInspector]
	public List<GameObject> nutList = new List<GameObject> ();

	[HideInInspector]
	public int message = 0;
	[HideInInspector]
	public string friend;
	[HideInInspector]
	public Languages language = Languages.English;

	[HideInInspector]
	public bool loading = false;
	[HideInInspector]
	public bool winnerPage = false;
	[HideInInspector]
	public bool showWinnerPage = false;
	[HideInInspector]
	public string sendData = "";
	private bool existTableInformation = true;
	private bool resetForNewSet = false;

	// Timer
	[HideInInspector]
	public bool playTimeOutStatus = false;
	[HideInInspector]
	public float delay = 1f;
	[HideInInspector]
	public float timeElapsed;
	[HideInInspector]
	public Text timeOutText;

	// Awake
	void Awake(){
		themeManager = GetComponent<ThemeManager> ();
		nutManager = GetComponent<NutManager> ();
		requestManager = GetComponent<RequestManager> ();
		messageManager = GetComponent<MessageManager> ();
		popUpManager = GetComponent<PopUpManager> ();
		winnerManager = GetComponent<WinnerManager> ();
	}

	// Fixed Update
	void FixedUpdate() {
		if (gameControl == null && GameObject.Find ("GameControl") != null) {
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();
			soundsManager = gameControl.GetComponent<SoundsManager> ();
			animationManager = gameControl.GetComponent<AnimationManager> ();
		}

		// Initialize
		// Comment
		if (gameControl != null) {
			if (!init) {
				if (gameControl.username == gameControl.userA) {
					color = "white";
					myTurn = 1;
				} else if (gameControl.username == gameControl.userB) {
					color = "black";
					myTurn = 2;
				} else if (gameControl.code != "") {
					tv = true;
					color = "white";
					myTurn = -1;
				} else {
					gameControl.tableGame = "{\"userAKick\":{},\"userBKick\":{},\"0\":{},\"1\":{\"0\":2,\"1\":2},\"2\":{},\"3\":{},\"4\":{},\"5\":{},\"6\":{\"0\":1,\"1\":1,\"2\":1,\"3\":1,\"4\":1},\"7\":{},\"8\":{\"0\":1,\"1\":1,\"2\":1},\"9\":{},\"10\":{},\"11\":{},\"12\":{\"0\":2,\"1\":2,\"2\":2,\"3\":2,\"4\":2},\"13\":{\"0\":1,\"1\":1,\"2\":1,\"3\":1,\"4\":1},\"14\":{},\"15\":{},\"16\":{},\"17\":{\"0\":2,\"1\":2,\"2\":2},\"18\":{},\"19\":{\"0\":2,\"1\":2,\"2\":2,\"3\":2,\"4\":2},\"20\":{},\"21\":{},\"22\":{},\"23\":{},\"24\":{\"0\":1,\"1\":1},\"25\":{}}";
					gameControl.diceA = Random.Range (1, 6);

					while (true) {
						gameControl.diceB = Random.Range (1, 6);
						if (gameControl.diceA != gameControl.diceB)
							break;
					}

					gameControl.turn = gameControl.diceA > gameControl.diceB ? 1 : 2;
					gameControl.remainderA = 167;
					gameControl.remainderB = 167;
					gameControl.userA = "White";
					gameControl.userB = "Black";
					gameControl.faceB = 13;
					gameControl.faceA = 13;
					gameControl.faceB = 13;

					offline = true;
					color = "white";
					myTurn = -1;
				}

				// Change Exit Name
				if (tv || offline)
					themeManager.exit.transform.Find ("ResignText").GetComponent<Text> ().text = "_EXIT";

				// Set Information
				turns = gameControl.turns;
				scoreA = gameControl.scoreA;
				scoreB = gameControl.scoreB;
				levelA = gameControl.levelA;
				levelB = gameControl.levelB;
				messageA = gameControl.messageA;
				messageB = gameControl.messageB;
				language = gameControl.language;

				UpdateInformation ();
				ReadTableJSON ();

				if (existTableInformation) {
					themeManager.SetTheme ();
					themeManager.SetUsersValue ();
					nutManager.InitNuts ();
					init = true;
				}
			} else if (!gameEnded) {
				Manager ();
			} else if(gameEnded && !showWinnerPage){
				StartCoroutine (ShowWinner ());
			}
		}
	}

	// Update
	void Update(){
		// Loading Aniamtion
		if (loading) {
			loadingObject.GetComponent<Text> ().color = new Color (1f, 1f, 1f, 1f);
			loadingObject.transform.Rotate (Vector3.forward * -90 * (delay * Time.deltaTime));
		} else {
			loadingObject.GetComponent<Text> ().color = new Color (1f, 1f, 1f, 0f);
		}

		// WinnerPage Aniamtion
		if (winnerPage)
			winnerPageObject.transform.Find("Background").transform.Rotate (Vector3.forward * -90 * (0.1f * Time.deltaTime));
	}

	// Update Information
	public void UpdateInformation() {
		// Comment
		code = gameControl.code;
		diceA = gameControl.diceA;
		diceB = gameControl.diceB;
		turn = gameControl.turn;
		lastMovement = gameControl.lastMovement;
		tableGame = gameControl.tableGame;
		usernameA.text = gameControl.userA;
		usernameB.text = gameControl.userB;
		remainderA.text = gameControl.remainderA.ToString ();
		remainderB.text = gameControl.remainderB.ToString ();
		faceA.sprite = animationManager.faceImages [gameControl.faceA - 1];
		faceB.sprite = animationManager.faceImages [gameControl.faceB - 1];
		maxDelay = gameControl.maxDelay;
		maxDelayOpportunity = gameControl.maxDelayOpportunity;

		// Play Timeout
		if (!offline) {
			float delayTimeOutAnimation = 0f;
			StartCoroutine (themeManager.PlayTimeOut (delayTimeOutAnimation));
		}
		// Comment
		gameControl.translateLanguage = true;
	}

	// Update Information Message
	public void UpdateInformationMessage(){
		// Show Message
		if(gameControl.messageA != messageA){
			messageA = gameControl.messageA;
			messageManager.ShowMessage("A" , gameControl.messageA);
		}

		if(gameControl.messageB != messageB){
			messageB = gameControl.messageB;
			messageManager.ShowMessage("B" , gameControl.messageB);
		}
	}

	// Show Popup
	public IEnumerator ShowPopup(string txt , float delay , float wait , bool endGame = false){
		yield return new WaitForSeconds(delay);
		popUpManager.message = txt;
		popUpManager.ShowPopUp ("Message");
		gameControl.translateLanguage = true;
		yield return new WaitForSeconds(wait);
		popUpManager.ClosePopUp ();
		if (endGame)
			gameControl.LoadScene ("BackgammonStaging");
	}

	// Show Posible Move
	public void ShowPosibleMove(string colorInput , List<int> moves){
		List<int> movesB = new List<int> ();
	
		if (moves.Count == 2) {
			if (moves [0] == moves [1]) {
				moves.Add (moves [0]);
				moves.Add (moves [0]);
			} else {
				movesB.Add (moves [1]);
				movesB.Add (moves [0]);
			}
		}

		List<MoveData> list = FindPosibleMove (tableGameArray, colorInput, moves);
		int maxMoveList = GetMaxMove (list);

		if (movesB.Count > 0) {
			List<MoveData> listB = FindPosibleMove (tableGameArray, colorInput, movesB);
			int maxMoveListB = GetMaxMove (listB);

			if (maxMoveList != 0 || maxMoveListB != 0) {
				if (maxMoveList == maxMoveListB) {
					foreach (var listItemB in listB) {
						list.Add (listItemB);
					}
				} else if (maxMoveListB > maxMoveList) {
					list = new List<MoveData> (listB);
					maxMoveList = maxMoveListB;
				}
			}
		}

		// Set Max Move
		maxMove = maxMoveList;
					
		if (list.Count > 0 && maxMove > 0) {
			// Remove Same 
			list = new List<MoveData> (RemoveSameMoves (list));
			posibleMoves = new List<MoveData> (list);

			/*
			foreach (var listItem in list) {
				string o = "";

				foreach (var move in listItem.moves) {
					o += move.start + " ---> " + move.end + " (" + move.move + ") |||||| ";
				}

				Debug.Log (o);
			}
			*/	

			// Auto Play
			if (true || list.Count > 1)
				nutManager.ShowDiceMove ();
			else {
				StartCoroutine (ShowAutoPlay(list[0].moves));
			}
		} else {
			posibleMoves = new List<MoveData> ();
			StartCoroutine (ShowNoMoreMove());
		}
	}

	// Find Posible Move
	private List<MoveData> FindPosibleMove(Dictionary<string , List<NutData>> tableInput , string colorInput , List<int> movesInput){
		List<MoveData> moveList = new List<MoveData> ();
		List<MoveData> preMoveList = new List<MoveData> ();
		Dictionary<string , List<NutData>> table = new Dictionary<string , List<NutData>> ();

		foreach (var tableItem in tableInput) {
			var tableKey = tableItem.Key;
			table.Add (tableKey, new List<NutData> (tableInput [tableKey]));
		}

		List<int> moves = new List<int> (movesInput);
		int nowMove = moves[0];
		string lastNutsLine = LastNutsLine (tableInput, offline ? (turn % 2 == 1 ? "white" : "black") : colorInput);
		bool nutsInHome = false;

		if (offline && ((turn % 2 == 1 && lastNutsLine != "userBKick" && int.Parse (lastNutsLine) < 7) ||
			(turn % 2 == 0 && lastNutsLine != "userAKick" && int.Parse (lastNutsLine) > 18))) {
			nutsInHome = true; 
		} else if ((colorInput == "white" && lastNutsLine != "userBKick" && int.Parse (lastNutsLine) < 7) ||
			(colorInput == "black" && lastNutsLine != "userAKick" && int.Parse (lastNutsLine) > 18))
			nutsInHome = true;

		// Pak Kardane Move
		moves.RemoveRange(0 , 1);

		foreach (var tableItem in table) {
			var tableKey = tableItem.Key;
			List<NutData> nutData = new List<NutData> (table [tableKey]);
			int nutDataCount = nutData.Count;

			// Agar Mohre harekati dashte bud
			if (nutDataCount > 0 && tableKey != "0" && tableKey != "25") {
				string lineColor = nutData[0].color;

				// Agar Color Line ba range vurudi Yeki Bud
				if(lineColor == colorInput){
					string findLine = "";

					// Mohre Zade Shode Ast
					if (tableKey == "userBKick" || tableKey == "userAKick") {
						if(colorInput == "white")
							findLine = (25 - nowMove).ToString();
						else
							findLine = nowMove.ToString();
					} else { // mohre dar vasate bazi asd
						if (colorInput == "white")
							findLine = (int.Parse (tableKey) - nowMove).ToString ();
						else
							findLine = (int.Parse (tableKey) + nowMove).ToString ();
					}


					// Check kardane inke khane Khali ast ya hadeaksar 1 dune mohre darad
					bool canMove = false;
					int minMove = 0 - (nutsInHome ? 1 : 0);
					int maxMove = 25 + (nutsInHome ? 1 : 0);
					bool nutInUserKick = NutsInUserKick (table , colorInput);

					if(!nutInUserKick || tableKey == "userBKick" || tableKey == "userAKick"){
						if (findLine != "") {
							if (int.Parse (findLine) > minMove && int.Parse (findLine) < maxMove) {
								List<NutData> nutDataFind = new List<NutData> (table [findLine]);
								int nutDataFindCount = nutDataFind.Count;

								if (nutDataFindCount == 0 || (nutDataFind [0].color == lineColor || nutDataFindCount == 1))
									canMove = true;	
							} else if (nutsInHome) {
								if (colorInput == "white" && lastNutsLine != "userBKick" && int.Parse (findLine) < 0 && int.Parse (tableKey) == int.Parse (lastNutsLine)) {
									findLine = "0";
									canMove = true;
								}

								if (colorInput == "black" && lastNutsLine != "userAKick" && int.Parse (findLine) > 25 && int.Parse (tableKey) == int.Parse (lastNutsLine)) {
									findLine = "25";
									canMove = true;
								}
							}
						}
					}

					// Can Move
					if(canMove){
						// Init Move
						MoveData move = new MoveData ();
						move.numberOfMove = 1;
						move.moves = new List<SingleMoveData> ();

						// Change Table
						Dictionary<string , List<NutData>> tableChildren = new Dictionary<string , List<NutData>> ();

						foreach (var tableItemChildren in table) {
							var tableKeyChildren = tableItemChildren.Key;
							tableChildren.Add (tableKeyChildren, new List<NutData> (table [tableKeyChildren]));
						}

						// Move Nut
						List<NutData> nutDataChildren = new List<NutData> (tableChildren [tableKey]);
						NutData nutItem = nutDataChildren [0];
						nutDataChildren.RemoveRange(0 , 1);
						tableChildren [tableKey] = nutDataChildren;
						nutDataChildren = tableChildren [findLine];

						// Agar Nuti dar an khane bud
						if (nutDataChildren.Count > 0) {
							string colorFind = nutDataChildren [0].color;

							// Hit
							if (colorFind != colorInput) {
								move.numberOfHit++;
								NutData nutItemKicked = nutDataChildren [0];

								if(colorFind == "white")
									tableChildren ["userBKick"].Add (nutItemKicked);
								else
									tableChildren ["userAKick"].Add (nutItemKicked);

								// Reset Nut Data
								tableChildren [findLine] = new List<NutData> ();
							}

							tableChildren [findLine].Add (nutItem);
						} else {
							tableChildren [findLine].Add (nutItem);
						}
							
						// Init Move
						move.numberOfMove = 1;
						move.numberOfSingle = FindSingleNuts(tableChildren , colorInput);
						move.numberOfSingleOpponent = FindSingleNuts(tableChildren , colorInput == "white" ? "black" : "white");
						move.remainderMove = FindRemaiderMove(tableChildren , colorInput);

						SingleMoveData singleMove = new SingleMoveData ();
						singleMove.start = tableKey;
						singleMove.end = findLine;
						singleMove.move = nowMove;
						move.moves.Add (singleMove);

						// Children Moves
						if (moves.Count > 0) {
							List<MoveData> childrenMoves = new List<MoveData> (FindPosibleMove (tableChildren, colorInput, moves));
		
							// Children harekat Darad
							if (childrenMoves.Count > 0) {
								foreach (var childrenItem in childrenMoves) {
									MoveData moveTemp = new MoveData ();
									moveTemp.numberOfMove = move.numberOfMove + childrenItem.numberOfMove;
									moveTemp.numberOfHit = move.numberOfHit + childrenItem.numberOfHit;
									moveTemp.numberOfSingle = childrenItem.numberOfSingle;
									moveTemp.numberOfSingleOpponent = childrenItem.numberOfSingleOpponent;
									moveTemp.remainderMove = childrenItem.remainderMove;
									moveTemp.moves = new List<SingleMoveData> (move.moves);

									foreach (var childrenMove in childrenItem.moves) {
										moveTemp.moves.Add (childrenMove);
									}

									moveList.Add (moveTemp);
								}
							} else {
								preMoveList.Add(move);
							}
						} else {
							// Ezafe Kardan Be list
							moveList.Add(move);
						}
					}
				}
			}
		}

		if (moveList.Count == 0 && preMoveList.Count > 0) {
			moveList = new List<MoveData> (preMoveList);
		}

		return moveList;
	}

	// Last Nuts Line
	private string LastNutsLine(Dictionary<string , List<NutData>> table , string colorInput){
		int lineNumber = colorInput == "white" ? -1 : 25;
		string lineName = colorInput == "white" ? "24" : "0";

		foreach (var tableItem in table) {
			var tableKey = tableItem.Key;
			List<NutData> nutData = new List<NutData> (table [tableKey]);
			int nutDataCount = nutData.Count;

			if (nutDataCount > 0 && nutData [0].color == colorInput) {
				if (colorInput == "white") {
					if (tableKey == "userBKick") {
						lineNumber = 25;
						lineName = "userBKick";
					} else if (int.Parse (tableKey) > lineNumber) {
						lineNumber = int.Parse (tableKey);
						lineName = tableKey;
					}
				} else {
					if (tableKey == "userAKick") {
						lineNumber = 0;
						lineName = "userAKick";
					} else if (int.Parse (tableKey) < lineNumber) {
						lineNumber = int.Parse (tableKey);
						lineName = tableKey;
					}
				}
			}
		}

		return lineName;
	}

	// All Nut in User Kick
	public bool NutsInUserKick(Dictionary<string , List<NutData>> table , string colorInput){
		foreach (var tableItem in table) {
			var tableKey = tableItem.Key;
			List<NutData> nutData = new List<NutData> (table [tableKey]);

			if (nutData.Count > 0) {
				if (nutData [0].color == colorInput && (tableKey == "userAKick" || tableKey == "userBKick")) {
					return true;
				}
			}
		}

		return false;
	}

	// All Nut in User Kick
	public int NutsInUserKickNumber(Dictionary<string , List<NutData>> table , string colorInput){
		foreach (var tableItem in table) {
			var tableKey = tableItem.Key;
			List<NutData> nutData = new List<NutData> (table [tableKey]);

			if (nutData.Count > 0) {
				if (nutData [0].color == colorInput && (tableKey == "userAKick" || tableKey == "userBKick")) {
					return nutData.Count;
				}
			}
		}

		return 0;
	}

	// Get Max Move
	private int GetMaxMove(List<MoveData> list){
		int maxMove = 0;

		if(list.Count > 0)
			foreach (var listItem in list) {
				if (listItem.moves.Count > maxMove)
					maxMove = listItem.moves.Count;
			}

		return maxMove;
	}

	// Get Next Line
	public string GetNextLine(string line , int move){
		if ((line == "UserAKick" && color == "white") ||
			(line == "UserBKick" && color == "black"))
			return move.ToString();
		else if ((line == "UserAKick" && color == "black") ||
			(line == "UserBKick" && color == "white"))
			return (24 - move).ToString();
		else {
			if (color == "white")
				return (int.Parse (line) - move).ToString ();
			else
				return (int.Parse (line) + move).ToString ();
		}
	}

	// Find Single Nuts
	public int FindSingleNuts(Dictionary<string , List<NutData>> tableInput , string colorInput){
		int numberSingle = 0;

		foreach (var tableItem in tableInput) {
			var tableKey = tableItem.Key;
			List<NutData> nutData = new List<NutData> (tableInput [tableKey]);
			int nutDataCount = nutData.Count;

			if (nutDataCount == 1 && tableKey != "0" && tableKey != "25" && tableKey != "userAKick" && tableKey != "userBKick") {
				if (nutData [0].color == colorInput) {
					numberSingle++;
				}
			}
		}

		return numberSingle;
	}

	// Find Remaider Move
	public int FindRemaiderMove(Dictionary<string , List<NutData>> tableInput , string colorInput){
		int reminder = 0;

		foreach (var tableItem in tableInput) {
			var tableKey = tableItem.Key;
			List<NutData> nutData = new List<NutData> (tableInput [tableKey]);
			int nutDataCount = nutData.Count;

			if (nutDataCount > 0 && tableKey != "0" && tableKey != "25") {
				if (nutData [0].color == colorInput) {
					if (tableKey == "userAKick" || tableKey == "userBKick")
						reminder += 24 * nutDataCount;
					else {
						if (colorInput == "white")
							reminder += int.Parse (tableKey) * nutDataCount;
						else
							reminder += (25 - int.Parse (tableKey)) * nutDataCount;
					}
				}
			}
		}

		return reminder;
	}

	// Remove Same Moves
	public List<MoveData> RemoveSameMoves(List<MoveData> listInput){
		List<MoveData> list = new List<MoveData> ();

		if (listInput.Count == 2) {
			var first = listInput [0];
			var second = listInput [1];

			if (first.numberOfMove == second.numberOfMove &&
			    first.numberOfHit == second.numberOfHit &&
			    first.numberOfSingle == second.numberOfSingle &&
			    first.numberOfSingleOpponent == second.numberOfSingleOpponent &&
			    first.remainderMove == second.remainderMove &&
			    first.moves.Count == second.moves.Count) {
				bool same = true;

				for (var i = 0; i < first.moves.Count; i++) {
					if (first.moves [i].end != second.moves [i].end) {
						same = false;
						break;
					}
				}

				list.Add (first);

				if (!same)
					list.Add (second);
			} else
				list = listInput;
		} else if (listInput.Count > 0) {
			foreach (var listInputItem in listInput) {
				bool find = false;

				if (list.Count > 0)
					foreach (var listItem in list) {
						if (listItem.numberOfMove == listInputItem.numberOfMove &&
						    listItem.numberOfHit == listInputItem.numberOfHit &&
						    listItem.numberOfSingle == listInputItem.numberOfSingle &&
						    listItem.numberOfSingleOpponent == listInputItem.numberOfSingleOpponent &&
						    listItem.remainderMove == listInputItem.remainderMove &&
						    listItem.moves.Count == listInputItem.moves.Count) {
							bool same = true;

							for (var i = 0; i < listItem.moves.Count; i++) {
								if (listItem.moves [i] != listInputItem.moves [i]) {
									same = false;
									break;
								}
							}

							if (same)
								find = true;
						}
					}

				if (!find) {
					list.Add (listInputItem);
				}
			}
		}

		return list;
	}

	// Show No More Move
	public IEnumerator ShowNoMoreMove(){
		nutManager.ShowDiceMove ();
		StartCoroutine (ShowPopup ("_NO_MORE_MOVE", 0f, 2f));
		yield return new WaitForSeconds (2f);
		nutManager.OnClickOk ();
	}

	// Show Auto Play
	public IEnumerator ShowAutoPlay(List<SingleMoveData> moves){
		yield return new WaitForSeconds (1f);
		autoplay = true;
		nutManager.AnimateNutsWithArray (moves);
		// baraye sakhte shodam move ha
		yield return new WaitForSeconds (2f);
		nutManager.OnClickOk ();
	}

	// Show Winner
	public IEnumerator ShowWinner(){
		yield return new WaitForSeconds (1f);
		themeManager.userLayout.SetActive (false);
		themeManager.gameLayout.SetActive (false);
		winnerPageObject.SetActive (true);
		showWinnerPage = true;
		winnerPage = true;

		// Set Options
		var userA = winnerPageObject.transform.Find ("UserA");
		var userB = winnerPageObject.transform.Find ("UserB");
		userA.Find ("Username").GetComponent<Text> ().text = gameControl.userA;
		userB.Find ("Username").GetComponent<Text> ().text = gameControl.userB;
		userA.Find ("ImageParent").Find ("Image").GetComponent<Image> ().sprite = animationManager.faceImages [gameControl.faceA - 1];
		userB.Find ("ImageParent").Find ("Image").GetComponent<Image> ().sprite = animationManager.faceImages [gameControl.faceB - 1];

		if (gameControl.winner == 1)
			userA.Find ("ImageParent").Find ("Medal").gameObject.SetActive (true);
		else if (gameControl.winner == 2)
			userB.Find ("ImageParent").Find ("Medal").gameObject.SetActive (true);

		// Create Paper
		StartCoroutine (winnerManager.CreateImage ());

		// Play Shound
		soundsManager.EnableSound ("Win");
		gameControl.translateLanguage = true;
	}

	// Message Popup
	public void MessagePopup() {
		var text = GameObject.Find ("PopUpWindow").transform.Find ("Message").Find ("Text");
		text.GetComponent<Text> ().text = popUpManager.message;
	}

	// Exit Popup
	public void ExitPopup(){}

	// Add Friend Popup
	public void AddFriendPopup(){}

	// Open Exit Popup
	public void OpenExitPopup(){
		popUpManager.ShowPopUp ("Exit");
	}

	// Open Add Friend Popup
	public void OpenAddFriendPopup(Button btn){
		friend = btn.transform.parent.parent.parent.Find ("Username").GetComponent<Text> ().text;
		popUpManager.ShowPopUp ("AddFriend");
	}

	// Exit
	public void Exit(){
		if (offline || tv)
			ExitToMain ();
		else {
			popUpManager.ClosePopUp ();
			requestManager.EnableRequest ("Exit Game");
		}
	}

	// Exit to Main
	public void ExitToMain(){
		requestManager.DisableRequest ("Send Data");
		requestManager.DisableRequest ("Get Movement");
		requestManager.DisableRequest ("Alive");

		gameControl.resetGameInformation ();
		gameControl.LoadScene ("Main");
	}

	// Add Friend Yes Button
	public void AddFriendYesButton(){
		requestManager.EnableRequest ("Add Friend");
		popUpManager.ClosePopUp ();
	}

	// Manager
	public void Manager() {
		// Show Winner Page
		if (gameControl.status == 2) {
			requestManager.DisableRequest ("Send Data");
			requestManager.DisableRequest ("Get Movement");
			requestManager.DisableRequest ("Alive");
	
			// Disable Timeout
			if (!offline)
				themeManager.DisablePlayTimeOut ();

			// For Show last Movement
			if (turns == gameControl.turns && !nutManager.setInitAfterAnimate) {
				// Create Last Nut in Home
				gameEnded = true;
			} else if (showAnimation) {
				if (!offline) {
					ReadLastMovementJSON ();
					nutManager.AnimateNutsMove ();
				}

				showAnimation = false;
			}
		} else if (resetForNewSet && !nutManager.setInitAfterAnimate) {
			resetForNewSet = false;
			ReadTableJSON ();
			nutManager.InitNuts ();
			themeManager.SetUsersValue ();
		} else {
			// Create New Nuts
			if (scoreA == gameControl.scoreA && scoreB == gameControl.scoreB) {
				if (Mathf.Abs (gameControl.turns - turns) > 1 && !offline) {
					ReadTableJSON ();
					nutManager.InitNuts ();
					turns = gameControl.turns;
				}
				// Show Dice Animate
				else if (!initMovement) {
					// Comment
					initMovement = true;

					if (!tv && !offline) {
						// Debug.Log ("ALive");
						requestManager.DisableRequest ("Get Movement");
						requestManager.EnableRequest ("Alive");
					}

					StartCoroutine (nutManager.AnimateDice ());
				}
			}

			// Set Kadane Score
			if (scoreA != gameControl.scoreA || scoreB != gameControl.scoreB) {
				scoreA = gameControl.scoreA;
				scoreB = gameControl.scoreB;
				showAnimation = true;
				resetForNewSet = true;
			}

			// Nobate Harekat Harif
			if (turn != myTurn && !requestManager.GetStatus ("Get Movement") && !offline) {
				// Debug.Log ("Get Movement");
				requestManager.EnableRequest ("Get Movement");
				requestManager.DisableRequest ("Alive");
			}

			// Show Dice Move
			if (turns != gameControl.turns && gameControl.turns == turns + 1 && showAnimation) {
				if (!offline) {
					ReadLastMovementJSON ();
					nutManager.AnimateNutsMove ();
				}

				showAnimation = false;
			}
		}
	}

	// Send Data
	public void SendData(){
		loading = true;
		// Disable Timeout
		themeManager.DisablePlayTimeOut ();

		WWWForm data = new WWWForm();
		data.AddField("code" , gameControl.code);
		data.AddField("data" , sendData);
		requestManager.SendData ("senddata", data , CheckSendData , "Send Data");
	}

	// Check Send Data
	public void CheckSendData(){
		Dictionary<string,string> result = requestManager.result;

		if (CheckInformation (result) && gameControl.turns != turns) {
			// Disable
			loading = false;
			requestManager.DisableRequest ("Send Data");

			// Update
			UpdateInformation ();
			turns = gameControl.turns;
			initMovement = false;

			// Init New Nuts
			nutManager.initNewNut = true;
			nutManager.CheckNutObjectPosition ();
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result["error"]);

			if (result ["error"] == "Turns was not accepted.") {
				requestManager.DisableRequest ("Send Data");
				nutManager.CheckNutObjectPosition ();
			}
		}
	}

	// Get MoveMent
	public void GetMovement(){
		loading = true;
		WWWForm data = new WWWForm();
		data.AddField("code" , gameControl.code);
		data.AddField("message" , message);
		if (tv)
			data.AddField("tv" , 1);
		requestManager.SendData ("getmovement", data, CheckGetMovement , "Get Movement");
	}

	// Check Get Movement
	public void CheckGetMovement(){
		Dictionary<string,string> result = requestManager.result;

		if (CheckInformation(result) && gameControl.turns != turns) {
			// Disable
			loading = false;
			requestManager.DisableRequest("Get Movement");
			// Update
			UpdateInformation ();
			UpdateInformationMessage ();
	
			// Show Animation
			if (!againRequest) {
				showAnimation = true;
			} else {
				turns = gameControl.turns;
				initMovement = false;
				againRequest = false;
			}

			// Init New Nuts
			nutManager.initNewNut = true;
			if (tv)
				nutManager.checkNutPosition = true;
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result["error"]);
		}
	}

	// Alive
	public void Alive(){
		WWWForm data = new WWWForm();
		data.AddField("code" , gameControl.code);
		data.AddField("message" , message);
		requestManager.SendData ("alive", data , CheckAlive , "Alive");
	}

	// Check Alive
	public void CheckAlive(){
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True" && result.ContainsKey("status") && 
			result.ContainsKey("turns") && result.ContainsKey("winner") && 
			result.ContainsKey("messageA") && result.ContainsKey("messageB")) {
			gameControl.status = int.Parse(result ["status"]);
			gameControl.turns = int.Parse(result ["turns"]);
			gameControl.winner = int.Parse(result ["winner"]);

			gameControl.messageA = int.Parse(result ["messageA"]);
			gameControl.messageB = int.Parse(result ["messageB"]);
	
			UpdateInformationMessage ();
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result["error"]);
		}
	}

	// Add Friend
	public void AddFriend(){
		WWWForm data = new WWWForm();
		data.AddField ("friend", friend);
		requestManager.SendData ("addfriend", data, CheckAddFriend, "Add Friend");
	}

	// Check Add Friend
	public void CheckAddFriend() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True" && result.ContainsKey ("statusFriend") && result.ContainsKey ("usernameFriend") && result.ContainsKey ("levelFriend")) {
			requestManager.DisableRequest ("Add Friend");
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result ["error"]);
			requestManager.DisableRequest ("Add Friend");
		}
	}

	// Exit Game
	public void ExitGame() {
		WWWForm data = new WWWForm();
		data.AddField("code" , gameControl.code);
		requestManager.SendData ("exitgame", data, CheckExitGame, "Exit Game");
	}

	// Check Exit Game
	public void CheckExitGame() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True") {
			requestManager.DisableRequest ("Exit Game");
			ExitToMain ();
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result ["error"]);
			requestManager.DisableRequest ("Exit Game");
		}
	}

	// Resign
	public void Resign(){
		WWWForm data = new WWWForm();
		data.AddField("code" , code);
		requestManager.SendData ("exitgame", data, CheckResign, "Resign");
	}

	// Check Resign
	public void CheckResign() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True") {
			requestManager.DisableRequest ("Resign");
			requestManager.EnableRequest ("Get Movement");
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result ["error"]);
			requestManager.DisableRequest ("Exit Game");
		}
	}

	// Read JSON
	public void ReadTableJSON(){
		if (tableGame == "") {
			existTableInformation = false;
			Debug.Log ("Again");
			requestManager.DisableRequest ("Get Movement");
			initMovement = true;
			againRequest = true;
			return;
		} else
			existTableInformation = true;

		if (existTableInformation) {
			// Table
			var json = (Dictionary<string , object>)Json.Deserialize (tableGame);
			tableGameArray = new Dictionary<string,List<NutData>> ();
			int whiteNumber = 1;
			int blackNumber = 1;

			if (json.Count > 0) {
				foreach (object jsonItem in json.Keys) {
					var key = jsonItem.ToString ();
					Dictionary<string , object> arr = null;
					List<NutData> nutdata = new List<NutData> ();

					if (json [key] != null)
						arr = (Dictionary<string , object>)json [key];

					//tablGame.Add (key , value);
					foreach (object arrItem in arr.Keys) {
						var arrKey = arrItem.ToString ();

						if (arr [arrKey] != null) {
							var value = arr [arrKey].ToString ();

							// Init Nut
							NutData nut = new NutData ();
							nut.color = value == "1" ? "white" : "black";
							nut.line = key;
							nut.position = new Vector2 ();
							nut.number = (value == "1" ? whiteNumber : blackNumber);
							nutdata.Add (nut);

							if (value == "1")
								whiteNumber++;
							else
								blackNumber++;
						}
					}

					// Add To Table
					tableGameArray.Add (key, nutdata);
				}
			}
		}
	}

	// Read Last Movement JSON
	private void ReadLastMovementJSON(){
		// Table
		var json = (Dictionary<string , object>)Json.Deserialize (lastMovement);
		lastMovementArray = new List<SingleMoveData> ();

		if(json.Count > 0)
			foreach (object jsonItem in json.Keys) {
				var key = jsonItem.ToString ();
				Dictionary<string , object> arr = null;
				SingleMoveData lastMove = new SingleMoveData ();

				if (json [key] != null)
					arr = (Dictionary<string , object>)json [key];

				foreach (object arrItem in arr.Keys) {
					var arrKey = arrItem.ToString ();

					if (arr [arrKey] != null) {
						var value = arr [arrKey].ToString ();

						if (arrKey == "start")
							lastMove.start = value;
						else if (arrKey == "end")
							lastMove.end = value;
						else if (arrKey == "move")
							lastMove.move = int.Parse (value);
					}
				}

				// Add To Table
				lastMovementArray.Add (lastMove);
			}
	}

	// Check Information
	private bool CheckInformation(Dictionary<string,string> result){
		if (result ["statusRequest"] == "True" && result.ContainsKey ("code") && result.ContainsKey ("gameTypeA") && result.ContainsKey ("gameTypeB") && 
			result.ContainsKey ("status") && result.ContainsKey ("userA") && result.ContainsKey ("userB") && result.ContainsKey ("acceptB") && 
			result.ContainsKey ("diceA") && result.ContainsKey ("diceB") && result.ContainsKey ("remainderA") && result.ContainsKey ("remainderB") && 
			result.ContainsKey ("turns") && result.ContainsKey ("turn") && result.ContainsKey ("lastTurn") && result.ContainsKey ("movements") && 
			result.ContainsKey ("lastMovement") && result.ContainsKey ("tableGame") && result.ContainsKey ("winner") && result.ContainsKey ("lastUpdate") && 
			result.ContainsKey ("rounds") && result.ContainsKey ("diceGame") && result.ContainsKey ("acceptDiceGameA") && result.ContainsKey ("acceptDiceGameB") && 
			result.ContainsKey ("scoreA") && result.ContainsKey ("scoreB") && result.ContainsKey ("aliasA") && result.ContainsKey ("aliasB") && 
			result.ContainsKey ("levelA") && result.ContainsKey ("levelB") && result.ContainsKey ("levelDegreeA") && result.ContainsKey ("levelDegreeB") && 
			result.ContainsKey ("faceA") && result.ContainsKey ("faceB") && result.ContainsKey ("messageA") && result.ContainsKey ("messageB") && 
			result.ContainsKey ("maxDelay") && result.ContainsKey ("maxDelayOpportunity") && result.ContainsKey ("calculate") && result.ContainsKey ("winnerCoin") && 
			result.ContainsKey ("loserCoin") && result.ContainsKey ("winnerGoldCoin") && result.ContainsKey ("loserGoldCoin")) {

			gameControl.code = result ["code"];
			gameControl.gameTypeA = result ["gameTypeA"];
			gameControl.gameTypeB = result ["gameTypeB"];
			gameControl.status = int.Parse (result ["status"]);
			gameControl.userA = result ["userA"];
			gameControl.userB = result ["userB"];
			gameControl.acceptB = result ["acceptB"] == "0" ? false : true;
			gameControl.diceA = int.Parse (result ["diceA"]);
			gameControl.diceB = int.Parse (result ["diceB"]);
			gameControl.remainderA = int.Parse (result ["remainderA"]);
			gameControl.remainderB = int.Parse (result ["remainderB"]);
			gameControl.turns = int.Parse (result ["turns"]);
			gameControl.turn = int.Parse (result ["turn"]);
			gameControl.lastTurn = int.Parse (result ["lastTurn"]);
			gameControl.movements = result ["movements"];
			gameControl.lastMovement = result ["lastMovement"];
			gameControl.tableGame = result ["tableGame"];
			gameControl.winner = int.Parse (result ["winner"]);
			gameControl.lastUpdate = result ["lastUpdate"];
			gameControl.rounds = int.Parse (result ["rounds"]);
			gameControl.diceGame = int.Parse (result ["diceGame"]);
			gameControl.acceptGameDiceA = result ["acceptDiceGameA"] == "0" ? false : true;
			gameControl.acceptGameDiceB = result ["acceptDiceGameB"] == "0" ? false : true;
			gameControl.scoreA = int.Parse (result ["scoreA"]);
			gameControl.scoreB = int.Parse (result ["scoreB"]);
			gameControl.aliasA = result ["aliasA"];
			gameControl.aliasB = result ["aliasB"];
			gameControl.levelA = int.Parse (result ["levelA"]);
			gameControl.levelB = int.Parse (result ["levelB"]);
			gameControl.levelDegreeA = float.Parse (result ["levelDegreeA"]);
			gameControl.levelDegreeB = float.Parse (result ["levelDegreeB"]);
			gameControl.faceA = int.Parse (result ["faceA"]);
			gameControl.faceB = int.Parse (result ["faceB"]);
			gameControl.messageA = int.Parse (result ["messageA"]);
			gameControl.messageB = int.Parse (result ["messageB"]);
			gameControl.maxDelay = int.Parse (result ["maxDelay"]);
			gameControl.maxDelayOpportunity = int.Parse (result ["maxDelayOpportunity"]);
			gameControl.calculate = result ["calculate"] == "0" ? false : true;
			gameControl.winnerCoin = int.Parse (result ["winnerCoin"]);
			gameControl.loserCoin = int.Parse (result ["loserCoin"]);
			gameControl.winnerGoldCoin = int.Parse (result ["winnerGoldCoin"]);
			gameControl.loserGoldCoin = int.Parse (result ["loserGoldCoin"]);

			return true;
		}

		return false;
	}
}