using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MiniJSON;

public class FriendsWindow : GenericWindow {
	public RequestManager requestManager;
	public PopUpManager popUpManager;
	private GameControl gameControl = null;

	public Dropdown dropdown;
	public InputField username;
	public GameObject itemObject;
	public GameObject itemsObject;

	private List<string> listDropdown = new List<string> ();
	private string dropdownText;
	private string deleteUsername;
	private string acceptUsername;
	private List<FriendData> friends = new List<FriendData> ();
	private int friendsNumber = 0;

	// Open
	public override void Open (){
		base.Open ();

		// Find Game Control
		if (gameControl == null && GameObject.Find ("GameControl") != null)
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();

		if (friends.Count == 0) {
			friendsNumber = 0;
			requestManager.EnableRequest ("Get Friends");
		}
	}

	// Change Dropdown
	public void ChangeDropdown(){
		if (requestManager.GetStatus ("Add Friend") == false && (dropdown.value != 0)) {
			dropdownText = listDropdown [dropdown.value];
			requestManager.EnableRequest ("Add Friend");
		}
	}

	// Add To Dropdown
	public void AddToDropdown(List<string> l){
		dropdown.ClearOptions ();
		dropdown.interactable = true;
		dropdown.AddOptions (l);
		dropdown.Show ();
	}

	// Search
	public void Search(){
		requestManager.EnableRequest ("Get Users");
	}

	// Show Friends
	public void ShowFriends(){
		if (friends.Count > 0) {
			foreach (var friend in friends) {
				friendsNumber++;
				AddFriendToItems (friend , friendsNumber);
			}

			gameControl.translateLanguage = true;
		}
	}

	// Add Friend To Items
	public void AddFriendToItems(FriendData friend , int number){
		GameObject item = Instantiate (itemObject , itemsObject.transform);
		item.name = friend.username;
		item.transform.SetSiblingIndex (0);

		item.transform.Find ("Rank").GetComponent<Image> ().color = new Color (0.96f , 0.89f , 0f , friend.status == 2 ? 1f : 0.3f);
		item.transform.Find ("Rank").Find ("Text").GetComponent<Text> ().text = number.ToString ();
		item.transform.Find ("Username").Find ("Text").GetComponent<Text> ().text = friend.username;
		item.transform.Find ("Level").Find ("Level").GetComponent<Text> ().text = friend.level.ToString ();
		Button btn = item.transform.Find ("Button").GetComponent<Button> ();
		btn.onClick.AddListener (delegate { DeleteCilck (btn); });
	}

	// Remove From Friends
	public void RemoveFromFriends(string username) {
		if(friendsNumber > 0)
			friendsNumber--;
		if (itemsObject.transform.Find (username))
			Destroy (itemsObject.transform.Find (username).gameObject);
	}

	// Delete Cilck
	public void DeleteCilck(Button btn){
		GameObject btnObject = btn.gameObject;
		deleteUsername = btnObject.transform.parent.Find ("Username").Find ("Text").GetComponent<Text> ().text;
		requestManager.EnableRequest ("Delete Friend");
	}
		
	// Accept Cilck
	public void AcceptCilck(Button btn){
		GameObject btnObject = btn.gameObject;
		acceptUsername = btnObject.transform.parent.Find ("Username").Find ("Text").GetComponent<Text> ().text;
		requestManager.EnableRequest ("Accept Friend");
	}

	// Get Users
	public void GetUsers(){
		WWWForm data = new WWWForm();
		data.AddField ("friend", username.text);
		requestManager.SendData ("getusers", data, CheckGetUsers, "Get Users");
	}

	// Check Get Users
	public void CheckGetUsers() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True") {
			if (result.ContainsKey ("list")) {
				listDropdown = new List<string> ();
				listDropdown.Add ("-");
				var json = (Dictionary<string , object>)Json.Deserialize (result ["list"]);

				if(json.Count > 0)
					foreach (object jsonItem in json.Keys) {
						var key = jsonItem.ToString ();
						Dictionary<string , object> arr = null;

						if (json [key] != null)
							arr = (Dictionary<string , object>)json [key];

						foreach (object arrItem in arr.Keys) {
							var arrKey = arrItem.ToString ();

							if (arr [arrKey] != null) {
								var value = arr [arrKey].ToString ();

								if (arrKey == "username")
									listDropdown.Add (value);
							}
						}
					}

				if (listDropdown.Count > 1)
					AddToDropdown (listDropdown);
			}

			requestManager.DisableRequest ("Get Users");
		} else if (result.ContainsKey ("error")) {
			requestManager.DisableRequest ("Get Users");
			popUpManager.message = "_NOT_FOUND";
			popUpManager.ShowPopUp("Message");
		}
	}

	// Add Friend
	public void AddFriend(){
		WWWForm data = new WWWForm();
		data.AddField ("friend", dropdownText);
		requestManager.SendData ("addfriend", data, CheckAddFriend, "Add Friend");
	}

	// Check Add Friend
	public void CheckAddFriend() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True" && result.ContainsKey ("statusFriend") && result.ContainsKey ("usernameFriend") && result.ContainsKey ("levelFriend")) {
			requestManager.DisableRequest ("Add Friend");

			// Add to Friends
			FriendData friend = new FriendData ();
			friend.status = int.Parse (result ["statusFriend"].ToString ());
			friend.username = result ["usernameFriend"].ToString ();
			friend.level = int.Parse (result ["levelFriend"].ToString ());
			friends.Add (friend);
			friendsNumber++;
			AddFriendToItems (friend, friendsNumber);

			gameControl.translateLanguage = true;
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result ["error"]);
			requestManager.DisableRequest ("Add Friend");
		}
	}

	// Delete Friend
	public void DeleteFriend(){
		WWWForm data = new WWWForm();
		data.AddField ("friend", deleteUsername);
		requestManager.SendData ("deletefriend", data, CheckDeleteFriend, "Delete Friend");
	}

	// Check Delete Friend
	public void CheckDeleteFriend() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True") {
			requestManager.DisableRequest ("Delete Friend");
			RemoveFromFriends (deleteUsername);
			popUpManager.ClosePopUp ();
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result ["error"]);
			requestManager.DisableRequest ("Delete Friend");
		}
	}

	// Accept Friend
	public void AcceptFriend(){
		WWWForm data = new WWWForm();
		data.AddField ("friend", acceptUsername);
		requestManager.SendData ("acceptfriend", data, CheckAcceptFriend, "Accept Friend");
	}

	// Check Accept Friend
	public void CheckAcceptFriend() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True") {
			requestManager.DisableRequest ("Accept Friend");
			popUpManager.ClosePopUp ();
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result ["error"]);
			requestManager.DisableRequest ("Accept Friend");
		}
	}

	// Get Friends
	public void GetFriends(){
		WWWForm data = new WWWForm();
		requestManager.SendData ("getfriends", data, CheckGetFriends, "Get Friends");
	}

	// Check Get Friends
	public void CheckGetFriends() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True" && result.ContainsKey ("friends") && result.ContainsKey ("friendsNumber")) {
			requestManager.DisableRequest ("Get Friends");

			if (int.Parse (result ["friendsNumber"]) > 0) {
				friends = ReadJSONFriends (result ["friends"]);
				ShowFriends ();
			}
		} else if (result.ContainsKey ("error")) {
			requestManager.DisableRequest ("Get Friends");
		}
	}

	// Read JSON Friends
	public List<FriendData> ReadJSONFriends(string input) {
		var json = (Dictionary<string, object>)Json.Deserialize (input);
		List<FriendData> list = new List<FriendData> ();

		if (json.Count > 0)
			foreach (object jsonItem in json.Keys) {
				var key = jsonItem.ToString ();
				Dictionary<string, object> arr = null;
				if (json [key] != null)
					arr = (Dictionary<string, object>)json [key];

				// Add To List
				FriendData friend = new FriendData ();
				friend.status = int.Parse (arr ["status"].ToString ());
				friend.username = arr ["username"].ToString ();
				friend.level = int.Parse (arr ["level"].ToString ());
				list.Add (friend);
			}

		return list;
	}
}