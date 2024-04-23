using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TapsellSDK;

public class AdvertisementManager : MonoBehaviour {
	private string tapsellCode = "gqtbfjaddlphooimckpobmjsbqtnqmrsrateshhcndearsetkpgqiasfggsplhjogngqmh";
	[HideInInspector]
	public string oxygenZoneID = "59a404034684651c44b97a9e";

	public string zoneID = "";
	public bool cached = false;
	public bool send = false;
	private bool available = false;
	private TapsellAd advertisement = null;
	private string advertisementID = null;

	public Text txt;

	void Start() {
		// Tapsell
		Tapsell.initialize (tapsellCode);
		Tapsell.setPermissionHandlerConfig (Tapsell.PERMISSION_HANDLER_AUTO);
		Tapsell.setDebugMode (true);

		Tapsell.setRewardListener((TapsellAdFinishedResult result) => {
			txt.text = "End";
			if (result.completed)
				txt.text = "End Complete";

			if(result.completed && result.rewarded) {
				txt.text = "End Show";
			}

			available = false;
		});
	}

	// Update
	void Update() {
		if (send && advertisementID == null && !available)
			SendRequest ();
	}

	// On GUI
	void OnGUI() {
		if (available) {
			txt.text = "Show " + advertisementID;
			available = false;
			TapsellShowOptions options = new TapsellShowOptions ();
			options.backDisabled = false;
			options.immersiveMode = true;
			options.rotationMode = TapsellShowOptions.ROTATION_LOCKED_LANDSCAPE;
			options.showDialog = true;
			Tapsell.showAd (advertisement, options);
		}
	}

	// Show Oxygen Advertisement
	public void ShowOxygenAdvertisement() {
		zoneID = oxygenZoneID;
		send = true;
		advertisementID = null;
		available = false;
		txt.text = "Start";
	}

	// Send Request
	public void SendRequest() {
		send = false;
		txt.text = "Send";

		Tapsell.requestAd(zoneID , cached , 
			(TapsellAd result) => {
				// onAdAvailable
				txt.text = "Action: onAdAvailable";
	
				if(result.adId != null) {
					available = true;
					advertisement = result;
					advertisementID = result.adId;
					txt.text = "ID:" + advertisementID;
				} else {
					send = true;
				}
			},

			(string zoneId) => {
				// onNoAdAvailable
				txt.text = "No Ad Available";
				send = false;
			},

			(TapsellError error) => {
				// onError
				txt.text = "EEEE:" + error.error;
				send = false;
			},

			(string zoneId) => {
				// onNoNetwork
				txt.text = "No Network";
				send = false;
			},

			(TapsellAd result) => {
				// onExpiring
				txt.text = "Expiring";
				advertisementID = null;
				send = true;
			}
		);
	}
}