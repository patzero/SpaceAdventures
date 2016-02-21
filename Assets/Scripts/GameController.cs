using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
    public GameObject[] hazards;
    public Vector3 spawnValues;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;
	public float warpTime;

	public Text scoreText;
	//public Text restartText;
	public Text gameOverText;

	public GameObject restartButton;
	public GameObject backgroundObject;
	private BGScroller bgScroller;

	private bool gameOver;
	private bool restart;

	private int score;

	void Start ()
	{
		gameOver = false;
		restart = false;
		//restartText.text = "";
		gameOverText.text = ""; 
		restartButton.SetActive (false); 
		score = 0;
		UpdateScore ();

		// Get the game controller 
		backgroundObject = GameObject.FindWithTag ("BackGround");
		if (backgroundObject != null)
		{
			bgScroller = backgroundObject.GetComponent <BGScroller>();
			backgroundObject.transform.localScale += new Vector3(0, 1400f, 0);
			bgScroller.SetScrollSpeed (-10);
		}
			
		StartCoroutine (HyperSpaceWarp ());
		StartCoroutine (SpawnWaves ());
	}

//	void Update ()
//	{
//		if (restart)
//		{
//			// Restart the game when press 'R'
//			if (Input.GetKeyDown (KeyCode.R))
//			{
//				Application.LoadLevel (Application.loadedLevel);
//			}
//		}
//	}

	IEnumerator HyperSpaceWarp ()
	{
		yield return new WaitForSeconds (warpTime);

		backgroundObject.transform.localScale += new Vector3(0, -1400f, 0);
		bgScroller.SetScrollSpeed (-1);

	}
	/**
	 * Spawn a wave of asteroid 
	 * */
	IEnumerator SpawnWaves ()
	{
		// Wait time when the game started to spawn the wave of asteroid
		yield return new WaitForSeconds (startWait);
		while (true)
		{
			for (int i = 0; i < hazardCount; i++)
			{
                // Create the position random between the edge of game screen
                GameObject hazard = hazards[Random.Range(0, hazards.Length)];
                Vector3 spawnPosition = new Vector3 (Random.Range (-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate (hazard, spawnPosition, spawnRotation);
				// Wait time for each ateroid spawn
				yield return new WaitForSeconds (spawnWait);
			}
			// Wait time for each wave  
			yield return new WaitForSeconds (waveWait);

			if (gameOver)
			{
				restartButton.SetActive (true);
				// restartText.text = "Press 'R' for Restart";
				restart = true;
				break;
			}
		}
	}

	public void AddScore (int newScoreValue)
	{
		score += newScoreValue;
		UpdateScore ();
	}

	void UpdateScore ()
	{
		scoreText.text = "Score: " + score;
	} 

	public void GameOver ()
	{
		gameOverText.text = "Game Over!";
		gameOver = true;
	}

	public void RestartGame() {
		Application.LoadLevel (Application.loadedLevel);  
	}
}
