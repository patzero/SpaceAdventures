using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour {

	public GameObject explosion;
	public GameObject playerExplosion ;
	public int scoreValue;
    private GameController gameController;
    private bool isDead = false;

	void Start ()
	{
		// Get the game controller 
		GameObject gameObject = GameObject.FindWithTag ("GameController");
		if (gameObject != null)
		{
			gameController = gameObject.GetComponent <GameController>();
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' script");
		}
	}

	/**
	 * Tigger event to destroy the other object when enter inside this object
	 * */
	void OnTriggerEnter(Collider other) 
	{
		// Exclude if this is the boundary object 
		if (other.tag == "Boundary" || other.tag == "Asteroid" || other.tag == "Enemy" || other.tag == "Bolt enemy")
		{
			return;
		}

		// Instanciate the explosion for the asteroid 
		if (explosion != null)
		{
			Instantiate (explosion, transform.position, transform.rotation);
		}

		// Instanciate the explosion for the player 
		if (other.tag == "Player")
		{
			Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
			gameController.GameOver();
		}

		if (other.tag == "Bolt" && !this.isDead)
		{
            // variable used for increment score just to 10
            this.isDead = true;
			// Add score after destroy
			gameController.AddScore(scoreValue);
        }

        Destroy(other.gameObject);
		Destroy(gameObject);
	}
}
