using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour {
	public static LoadingManager loadingManager;

	// Set Dont Destory Game Control Information
	void Awake(){
		// Create Dont Destroy On Load
		if (loadingManager == null) {
			DontDestroyOnLoad (gameObject);
			loadingManager = this;
		} else if (loadingManager != this) {
			Destroy (gameObject);
		}
	}
}