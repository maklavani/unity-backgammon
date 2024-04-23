using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paper : MonoBehaviour {
	public delegate void OnDestroy ();
	public event OnDestroy DestroyCallBack;

	private bool offScreen;
	private float offScreenY = -100f;
	private Rigidbody2D body2d;
	private Vector2 initialVelocity = new Vector2(75 , -75);

	void Awake(){
		body2d = GetComponent<Rigidbody2D> ();
	}

	void Start(){
		var startVelX = Random.Range(initialVelocity.x , initialVelocity.y) * transform.localScale.x;
		var startVelY = Random.Range(initialVelocity.x , initialVelocity.y) * transform.localScale.y;
		body2d.velocity = new Vector2 (startVelX , startVelY);
		body2d.rotation = Random.Range(0.0f , 1.0f);
	}

	void Update () {
		var posY = transform.position.y;
		var dirX = body2d.velocity.x;

		if (posY < offScreenY) {
			offScreen = true;
		} else {
			offScreen = false;
		}

		if (offScreen) {
			DestroyMe ();
		}
	}

	public void DestroyMe(){
		Destroy (gameObject);

		if (DestroyCallBack != null) {
			DestroyCallBack ();
		}
	}
}