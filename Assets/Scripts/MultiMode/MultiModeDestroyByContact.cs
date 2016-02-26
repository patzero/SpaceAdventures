using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MultiModeDestroyByContact : NetworkBehaviour {

	public GameObject explosion;
	public GameObject playerExplosion;
	public int scoreValue;

	private MultiModeGameController gameController;
	private MultiModePlayerController playerController;
	private bool isDead = false;

	void Start ()
	{
		// Get the game controller 
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent <MultiModeGameController>();
		}
		if (gameController == null){
			Debug.Log ("Cannot find multi mode 'MultiModeGameController' script");
		}

		GameObject player = GameObject.FindWithTag ("Player");
		if (player != null) {
			playerController = player.GetComponent <MultiModePlayerController>();
		}
		if (gameController == null) {
			Debug.Log ("Cannot find multi mode 'MultiModePlayerController' script");
		}
	}

	public override void OnNetworkDestroy ()
	{
		if (explosion != null) {
			//MakeExplosion (explosion, 1.5f);
		}
	}

	/**
	 * Tigger event to destroy the other object when enter inside this object
	 * */
	void OnTriggerEnter(Collider other) 
	{
		//if (!isServer)
		//	return;
		
		// Exclude if this is the boundary object 
		if (other.tag == "Boundary" || other.tag == "Enemy") {
			return;
		}

		// Instanciate the explosion for the asteroid 
		if (explosion != null) {
			//Instantiate (explosion, transform.position, transform.rotation);
			NetworkServer.Spawn ((GameObject)Instantiate (explosion, transform.position, transform.rotation));

		}

		// Instanciate the explosion for the player 
		if (other.tag == "Player") {
			//gameController.AddScore (scoreValue);

			// Check player health
			var playerController = other.GetComponent <MultiModePlayerController>();
			playerController.TakeDamage(20);
			playerController.AddScore (scoreValue);

			int playerHealth = playerController.GetHealth ();
			if (playerHealth <= 0) {
				GameObject explo = Instantiate(playerExplosion, transform.position, transform.rotation) as GameObject;
				NetworkServer.Spawn (explo);
				//gameController.GameOver ();
				//Destroy(explo, 1.0f);
				Destroy(other.gameObject);
			}
			Destroy(gameObject);
			return;
		}

		if (other.tag == "Bolt" /*&& !this.isDead*/)
		{
			// Variable used for increment score just to 10
			//this.isDead = true;

			// Add score 
			playerController.AddScore (scoreValue);
			Destroy(other.gameObject);
			Destroy(gameObject);
			return;
		}

		Destroy(other.gameObject);
		Destroy(gameObject);
	}

	void MakeExplosion(GameObject explosion, float t)
	{
		GameObject exp = Instantiate (explosion, transform.position, transform.rotation) as GameObject;
		Destroy(exp, t);
	}
}
