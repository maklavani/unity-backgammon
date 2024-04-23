using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendWindow : GenericWindow {
	public RequestManager requestManager;
	private GameControl gameControl = null;
	private SoundsManager soundsManager;
	private AnimationManager animationManager;

	[HideInInspector]
	public List<GameObject> users;
	public GameObject loadingObject;

	private string usernameA = "";
	private string usernameB = "";
	private int rounds = 1;
	private bool loading = false;

	// Animation
	private float delay = 0.0f;
	private List<int> frames = new List<int> (){ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
	private List<int> userFrame = new List<int> ();
	private List<bool> statusFrame = new List<bool> ();

	// Open
	public override void Open (){
		base.Open ();

		// Find Game Control
		if (gameControl == null && GameObject.Find ("GameControl") != null) {
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();
			soundsManager = gameControl.GetComponent<SoundsManager> ();
			animationManager = gameControl.GetComponent<AnimationManager> ();
		}

		SetRandomFrame ();
		soundsManager.DisableAllSounds ();
		soundsManager.EnableSound ("New Game", true);

		requestManager.EnableRequest ("New Game Friend");
	}

	// Update
	void Update(){
		CheckUsernameExist ();

		// Loading Aniamtion
		if (loading) {
			loadingObject.GetComponent<Text> ().color = new Color (1f, 1f, 1f, 1f);
			loadingObject.transform.Rotate (Vector3.forward * -90 * (3f * Time.deltaTime));
		} else {
			loadingObject.GetComponent<Text> ().color = new Color (1f, 1f, 1f, 0f);
		}

		// Animation
		if (statusFrame.Count > 0){
			delay += Time.deltaTime;

			if (delay > 0.075f) {
				delay = 0f;

				for (var i = 0; i < statusFrame.Count; i++)
					if (statusFrame [i] == true) {
						userFrame [i] = (userFrame [i] + 1) % frames.Count;
						SetImage (i, userFrame [i]);
					}
			}
		}

		if (Input.GetKeyDown (KeyCode.Escape))
			ExitGameButton ();
	}

	// Close
	public override void Close (){
		requestManager.DisableRequest ("New Game Friend");
		base.Close ();
	}

	// Set Random Frame
	public void SetRandomFrame () {
		if (users.Count > 0)
			foreach (var user in users) {
				userFrame.Add (Random.Range(0 , frames.Count - 1));
				statusFrame.Add (true);
			}
	}

	// Set Image
	public void SetImage(int user , int frame){
		users [user].transform.Find ("ImageParent").Find ("Image").Find ("Face").GetComponent<Image> ().sprite = animationManager.faceImages [frames[frame] - 1];
	}

	// Check Username Exist
	public void CheckUsernameExist () {
		if (gameControl.userA != "" && usernameA == "") {
			usernameA = gameControl.userA;
			statusFrame [0] = false;
			users [0].transform.Find ("ImageParent").Find ("Image").Find ("Face").GetComponent<Image> ().sprite = animationManager.faceImages [gameControl.faceA - 1];
			var username = users [0].transform.Find ("TextParent").Find ("Username").gameObject;
			username.SetActive (true);
			username.GetComponent<Text> ().text = usernameA;
		} else if (gameControl.userB != "" && usernameB == "") {
			usernameB = gameControl.userB;
			statusFrame [1] = false;
			users [1].transform.Find ("ImageParent").Find ("Image").Find ("Face").GetComponent<Image> ().sprite = animationManager.faceImages [gameControl.faceB - 1];
			var username = users [1].transform.Find ("TextParent").Find ("Username").gameObject;
			username.SetActive (true);
			username.GetComponent<Text> ().text = usernameB;
		}
	}

	// Exit Game Button
	public void ExitGameButton(){
		requestManager.DisableRequest ("New Game Friend");
		requestManager.EnableRequest ("Exit Game Friend");
	}

	// New Game Friend
	public void NewGameFriend(){
		loading = true;

		WWWForm data = new WWWForm();
		data.AddField ("gameType" , "friendGame");
		data.AddField ("rounds" , rounds);
		requestManager.SendData ("newgame" , data , CheckNewGameFriend , "New Game Friend");
	}

	// Check New Game Friend
	public void CheckNewGameFriend (){
		Dictionary<string,string> result = requestManager.result;

		if (CheckPlayersExsits (result)) {
			// Disable
			loading = false;
			requestManager.DisableRequest ("New Game Friend");

			StartCoroutine (gotoGame ());
		} else if (result ["statusRequest"] == "True"){
			requestManager.EnableRequest ("New Game Friend");
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result["error"]);
		}
	}

	// Check Players Exsits
	private bool CheckPlayersExsits(Dictionary<string,string> result){
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
			result.ContainsKey ("loserCoin") && result.ContainsKey ("winnerGoldCoin") && result.ContainsKey ("loserGoldCoin") && 
			result ["status"] == "1" && result ["userB"] != "" && result ["acceptB"] == "1") {

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

		if (result.ContainsKey ("code"))
			gameControl.code = result ["code"];
		if (result.ContainsKey ("gameTypeA") && result ["gameTypeA"] != "")
			gameControl.gameTypeA = result ["gameTypeA"];
		if (result.ContainsKey ("gameTypeB") && result ["gameTypeB"] != "")
			gameControl.gameTypeB = result ["gameTypeB"];
		if (result.ContainsKey ("userA") && result ["userA"] != "")
			gameControl.userA = result ["userA"];
		if (result.ContainsKey ("userB") && result ["userB"] != "")
			gameControl.userB = result ["userB"];
		if (result.ContainsKey ("faceA") && result ["faceA"] != "")
			gameControl.faceA = int.Parse (result ["faceA"]);
		if (result.ContainsKey ("faceB") && result ["faceB"] != "")
			gameControl.faceB = int.Parse (result ["faceB"]);

		return false;
	}

	// Exit Game Friend
	public void ExitGameFriend(){
		WWWForm data = new WWWForm();
		data.AddField("code" , gameControl.code);
		requestManager.SendData ("exitgame", data, CheckExitGameFriend, "Exit Game Friend");
	}

	// Check Exit Game Friend
	public void CheckExitGameFriend() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True") {
			requestManager.DisableRequest ("Exit Game Friend");
			gameControl.resetGameInformation ();
			manager.Open ((int)Windows.Home);
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result ["error"]);
			requestManager.DisableRequest ("Exit Game Friend");
			requestManager.EnableRequest ("New Game Friend");
		}
	}

	// GotoGame
	public IEnumerator gotoGame(){
		yield return new WaitForSeconds (1f);
		soundsManager.DisableAllSounds ();
		gameControl.LoadScene ("Backgammon");
	}
}