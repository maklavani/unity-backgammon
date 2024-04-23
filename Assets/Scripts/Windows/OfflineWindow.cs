using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class OfflineWindow : GenericWindow {
	public SoundsManager soundsManager;
	public ToggleGroup rounds;
	private GameControl gameControl = null;

	// Open
	public override void Open (){
		base.Open ();

		// Find Game Control
		if (gameControl == null && GameObject.Find ("GameControl") != null)
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();
	}

	// Start Game
	public void StartGame(){
		var round = rounds.ActiveToggles ().FirstOrDefault ();
		gameControl.rounds = int.Parse (round.name.ToString ());
		StartCoroutine (gotoGame ());
	}

	// GotoGame
	public IEnumerator gotoGame(){
		yield return new WaitForSeconds (1f);
		soundsManager.DisableAllSounds ();
		gameControl.LoadScene ("Backgammon");
	}
}