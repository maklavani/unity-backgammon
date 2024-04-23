using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MiniJSON;

public class LayoutManager : MonoBehaviour {
	public GameControl gameControl;
	public GameObject WindowManager;

	private WindowManager manager;
	private PopUpManager popUpManager;
	private List<FriendData> friends = new List<FriendData> ();

	// Sise
	public GameObject topLayout;
	public GameObject topLayoutHidden;
	public GameObject bottomLayout;
	public GameObject bottomLayoutHidden;
	[HideInInspector]
	public RectTransform rtTop;
	[HideInInspector]
	public RectTransform rtTopHidden;
	[HideInInspector]
	public RectTransform rtBottom;
	[HideInInspector]
	public RectTransform rtBottomHidden;

	// Hide Animation
	public bool hide = true;
	public bool hideBottom = true;
	private bool init = false;
	private float speed;
	private float targetPosition = 110f;
	private float targetPositionBottom = -120f;
	private float topPosition = 90f;
	private float bottomPosition = -80f;

	void Awake() {
		manager = WindowManager.GetComponent<WindowManager> ();
		popUpManager = GetComponent<PopUpManager> ();

		// Sise
		rtTop = topLayout.GetComponent<RectTransform> ();
		rtTopHidden = topLayoutHidden.GetComponent<RectTransform> ();
		rtBottom = bottomLayout.GetComponent<RectTransform> ();
		rtBottomHidden = bottomLayoutHidden.GetComponent<RectTransform> ();

		// Hide Animation
		speed = 100 * Time.deltaTime;
		if (rtTopHidden.gameObject.activeSelf && rtTopHidden.sizeDelta.y > 0)
			rtTop.sizeDelta = new Vector2 (0, rtTopHidden.sizeDelta.y);
		if (rtBottomHidden.gameObject.activeSelf && rtBottomHidden.sizeDelta.y > 0)
			rtBottom.sizeDelta = new Vector2 (0, rtBottomHidden.sizeDelta.y);
	}

	void Update(){
		// Set Size
		if (rtTopHidden.gameObject.activeSelf && rtTopHidden.sizeDelta.y > 0)
			rtTop.sizeDelta = new Vector2 (0, rtTopHidden.sizeDelta.y);
		if (rtBottomHidden.gameObject.activeSelf && rtBottomHidden.sizeDelta.y > 0)
			rtBottom.sizeDelta = new Vector2 (0, rtBottomHidden.sizeDelta.y);
		targetPosition = rtTop.sizeDelta.y / 2 + 41f;
		targetPositionBottom = -1 * (rtBottom.sizeDelta.y / 2 + 41f);
		topPosition = -1 * (rtTop.sizeDelta.y / 2);
		bottomPosition = rtBottom.sizeDelta.y / 2;

		// Hide Animation
		if (!init) {
			if (!V2Equal (rtTop.anchoredPosition, new Vector2 (0, topPosition)))
				rtTop.anchoredPosition = Vector2.MoveTowards (rtTop.anchoredPosition, new Vector2 (0, topPosition), 1000000 * speed);

			if (!V2Equal (rtBottom.anchoredPosition, new Vector2 (0, bottomPosition)))
				rtBottom.anchoredPosition = Vector2.MoveTowards (rtBottom.anchoredPosition, new Vector2 (0, bottomPosition), 1000000 * speed);

			if (V2Equal (rtTop.anchoredPosition, new Vector2 (0, topPosition)) && V2Equal (rtBottom.anchoredPosition, new Vector2 (0, bottomPosition)))
				init = true;
		} else {
			if (hide && !V2Equal (rtTop.anchoredPosition, new Vector2 (0, targetPosition)))
				rtTop.anchoredPosition = Vector2.MoveTowards (rtTop.anchoredPosition, new Vector2 (0, targetPosition), speed);
			else if (!hide && !V2Equal (rtTop.anchoredPosition, new Vector2 (0, topPosition)))
				rtTop.anchoredPosition = Vector2.MoveTowards (rtTop.anchoredPosition, new Vector2 (0, topPosition), speed);

			if (hideBottom && !V2Equal (rtBottom.anchoredPosition, new Vector2 (0, targetPositionBottom)))
				rtBottom.anchoredPosition = Vector2.MoveTowards (rtBottom.anchoredPosition, new Vector2 (0, targetPositionBottom), speed);
			else if (!hideBottom && !V2Equal (rtBottom.anchoredPosition, new Vector2 (0, bottomPosition)))
				rtBottom.anchoredPosition = Vector2.MoveTowards (rtBottom.anchoredPosition, new Vector2 (0, bottomPosition), speed);
		}
	}

	// V2Equal
	public bool V2Equal(Vector2 a , Vector2 b){
		return Vector2.SqrMagnitude (a - b) < 0.0001;
	}

	// V3Equal
	public bool V3Equal(Vector3 a , Vector3 b){
		return Vector3.SqrMagnitude (a - b) < 0.0001;
	}

	// Open Home
	public void OpenHome(){
		manager.Open ((int)Windows.Home);
	}

	// Open New Game
	public void OpenNewGame() {
		if (gameControl.oxygen > 0)
			manager.Open ((int)Windows.NewGame);
		else
			popUpManager.ShowPopUp ("Oxygen");
	}

	// Open Store
	public void OpenStore(){
		manager.Open ((int)Windows.Store);
	}

	// Open Setting
	public void OpenSetting(){
		manager.Open ((int)Windows.Setting);
	}

	// Open Ranking
	public void OpenRanking(){
		manager.Open ((int)Windows.Ranking);
	}

	// Open Friends
	public void OpenFriends(){
		manager.Open ((int)Windows.Friends);
	}

	// Open Offline
	public void OpenOffline(){
		manager.Open ((int)Windows.Offline);
	}

	// Open Tournament
	public void OpenTournament(){
		manager.Open ((int)Windows.Tournament);
	}

	// Open Final Victor
	public void OpenFinalVictor(){
		manager.Open ((int)Windows.FinalVictor);
	}

	// Open Start
	public void OpenStart(){
		manager.Open ((int)Windows.Start);
	}

	// Open TV
	public void OpenTV(){
		manager.Open ((int)Windows.TV);
	}

	// Open Profile
	public void OpenProfile(){
		manager.Open ((int)Windows.Profile);
	}

	// Open Friend
	public void OpenFriend() {
		Debug.Log (gameControl.friends);
		friends = ReadJSONFriends (gameControl.friends);

		if (friends.Count > 0) {
			if (gameControl.oxygen > 0)
				manager.Open ((int)Windows.Friend);
			else
				popUpManager.ShowPopUp ("Oxygen");
		} else {
			popUpManager.message = "_HAVENT_FRIEND";
			popUpManager.ShowPopUp ("Message");
		}
	}

	// Open Tournaments
	public void OpenTournaments(){
		manager.Open ((int)Windows.Tournaments);
	}

	// OxygenPopup
	public void OxygenPopup(){
		var oxygen = GameObject.Find ("PopUpWindow").transform.Find ("Oxygen").Find ("Buttons").Find ("Coin");

		if (gameControl.coin >= 250)
			oxygen.GetComponent<Button> ().interactable = true;
		else
			oxygen.GetComponent<Button> ().interactable = false;
	}

	// Mobile Popup
	public void MobilePopup() {}

	// Charity Popup
	public void CharityPopup() {
		List<GameObject> objs = new List<GameObject> ();
		objs.Add (GameObject.Find ("Charity").transform.Find ("CharityWindow").Find ("Buttons").Find ("1").gameObject);
		objs.Add (GameObject.Find ("Charity").transform.Find ("CharityWindow").Find ("Buttons").Find ("2").gameObject);
		objs.Add (GameObject.Find ("Charity").transform.Find ("CharityWindow").Find ("Buttons").Find ("3").gameObject);

		if(objs.Count > 0)
			foreach(var obj in objs){
				var pm = obj.GetComponent<ProductData> ();

				if (pm.coin <= gameControl.coin)
					pm.GetComponent<Button> ().interactable = true;
				else
					pm.GetComponent<Button> ().interactable = false;
			}
	}

	// Friend Popup
	public void FriendPopup() {}

	// Message Popup
	public void MessagePopup() {
		var text = GameObject.Find ("PopUpWindow").transform.Find ("Message").Find ("Text");
		text.GetComponent<Text> ().text = popUpManager.message;
	}

	// Open Charity Popup
	public void OpenCharityPopup() {
		popUpManager.ShowPopUp ("Charity");
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
				if (arr.ContainsKey ("level"))
					friend.level = int.Parse (arr ["level"].ToString ());
				list.Add (friend);
			}

		return list;
	}
}