using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TVWindow : GenericWindow {
	public RequestManager requestManager;
	private GameControl gameControl = null;
	private SoundsManager soundsManager;

	public GameObject loadingObject;
	private bool loading = false;

	// Open
	public override void Open () {
		base.Open ();

		// Find Game Control
		if (gameControl == null && GameObject.Find ("GameControl") != null) {
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();
			soundsManager = gameControl.GetComponent<SoundsManager> ();
		}
	
		soundsManager.DisableAllSounds ();
		soundsManager.EnableSound ("New Game", true);
	
		requestManager.EnableRequest ("TV");
	}

	// Close
	public override void Close () {
		requestManager.DisableRequest ("TV");
		base.Close ();
	}

	// Update
	void Update(){
		// Loading Aniamtion
		if (loading) {
			loadingObject.GetComponent<Text> ().color = new Color (1f, 1f, 1f, 1f);
			loadingObject.transform.Rotate (Vector3.forward * -90 * (3f * Time.deltaTime));
		} else {
			loadingObject.GetComponent<Text> ().color = new Color (1f, 1f, 1f, 0f);
		}
	}

	// TV
	public void TV(){
		loading = true;
		WWWForm data = new WWWForm();
		requestManager.SendData ("tv" , data , CheckTV , "TV");
	}

	// Check TV
	public void CheckTV (){
		Dictionary<string,string> result = requestManager.result;

		if (CheckPlayersExsits (result)) {
			// Disable
			loading = false;
			requestManager.DisableRequest ("TV");

			StartCoroutine (gotoGame ());
		} else if (result ["statusRequest"] == "True"){
			requestManager.EnableRequest ("New Game");
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result["error"]);

			if (result ["error"] == "Not Found Game.") {
				loading = false;
				requestManager.DisableRequest ("TV");
				manager.Open ((int)Windows.Home);
			}
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

		if (result.ContainsKey ("userA") && result.ContainsKey ("faceA") && result ["userA"] != "" && result ["faceA"] != "") {
			gameControl.userA = result ["userA"];
			gameControl.faceA = int.Parse (result ["faceA"]);
		}

		if (result.ContainsKey ("userB") && result.ContainsKey ("faceB") && result ["userB"] != "" && result ["faceB"] != "") {
			gameControl.userB = result ["userB"];
			gameControl.faceB = int.Parse (result ["faceB"]);
		}

		return false;
	}

	// GotoGame
	public IEnumerator gotoGame(){
		yield return new WaitForSeconds (1f);
		soundsManager.DisableAllSounds ();
		gameControl.LoadScene ("Backgammon");
	}
}