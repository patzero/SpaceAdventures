﻿using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour {

	public GameObject explosion;
	public GameObject playerExplosion ;
	public int scoreValue;
	private GameController gameController;

	void Start ()
	{
		// Get the game controller 
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <GameController>();
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
		if (other.tag == "Boundary")
		{
			return;
		}

		// Instanciate the explosion for the asteroid 
		Instantiate(explosion, transform.position, transform.rotation);

		// Instanciate the explosion for the player 
		if (other.tag == "Player")
		{
			Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
			gameController.GameOver ();
		}

		// Add score after destroy
		gameController.AddScore (scoreValue);

		Destroy(other.gameObject);
		Destroy(gameObject);

	}
}
