using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentsWindow : GenericWindow {
	public RequestManager requestManager;
	public PopUpManager popUpManager;
	private GameControl gameControl = null;

	[HideInInspector]
	public List<GameObject> tournamentsObject;

	public override void Open (){
		base.Open ();

		// Find Game Control
		if (gameControl == null && GameObject.Find ("GameControl") != null)
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();

		UpdateInformation ();
	}

	// Update Information
	public void UpdateInformation(){
		if(tournamentsObject.Count > 0)
			foreach(var obj in tournamentsObject){
				var td = obj.GetComponent<TournamentData> ();

				if (td.enterLevel <= gameControl.level) {
					td.GetComponent<Button> ().interactable = true;
					obj.transform.Find ("Lock").gameObject.SetActive (false);
				} else {
					td.GetComponent<Button> ().interactable = false;
					obj.transform.Find ("Lock").gameObject.SetActive (true);
				}
			}
	}

	// Tournament Click
	public void TournamentClick(Button btn){
		GameObject go = btn.gameObject;
		var td = go.GetComponent<TournamentData> ();
		manager.Open ((int)Windows.Tournament);
	}
}