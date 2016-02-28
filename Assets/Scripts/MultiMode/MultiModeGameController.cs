using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class MultiModeGameController : NetworkBehaviour {
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
	public GameObject loadingImage;

	public GameObject restartButton;
	public GameObject exitButton;
	private GameObject backgroundObject;
	private BGScroller bgScroller;

	private bool gameOver;
	private int score;

	void Start ()
	{
		/*gameOver = false;
		//restartText.text = "";
		gameOverText.text = ""; 
		restartButton.SetActive (false); 
		exitButton.SetActive (false);
		score = 0;
		UpdateScore ();*/

		// Get the game BackGround 
		/*backgroundObject = GameObject.FindWithTag ("BackGround");
		if (backgroundObject != null)
		{
			bgScroller = backgroundObject.GetComponent <BGScroller>();
			backgroundObject.transform.localScale += new Vector3(0, 1400f, 0);
			bgScroller.SetScrollSpeed (-10);
		}
			
		StartCoroutine (HyperSpaceWarp ());*/

	}

	void Update() 
	{
		//scoreText.text = "Score: " + score;
	}

	public override void OnStartServer() {
		gameOver = false;
		//restartText.text = "";
		//gameOverText.text = ""; 

		score = 0;
		UpdateScore ();

		StartCoroutine (SpawnWaves ());
	}

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
				//GameObject enemy = Instantiate (hazard, spawnPosition, spawnRotation) as GameObject;
				NetworkServer.Spawn ((GameObject)Instantiate (hazard, spawnPosition, spawnRotation));
				//CmdSpawn ();

				// Wait time for each ateroid spawn
				yield return new WaitForSeconds (spawnWait);
			}
			// Wait time for each wave  
			yield return new WaitForSeconds (waveWait);

			if (gameOver)
			{
				//restartButton.SetActive (true);
				//exitButton.SetActive (true);
				// restartText.text = "Press 'R' for Restart";
				break;
			}
		}
	}

	//[Command]
	void CmdSpawn () {
		// Create the position random between the edge of game screen
		GameObject hazard = hazards[Random.Range(0, hazards.Length)];
		Vector3 spawnPosition = new Vector3 (Random.Range (-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
		Quaternion spawnRotation = Quaternion.identity;
		//GameObject enemy = Instantiate (hazard, spawnPosition, spawnRotation) as GameObject;
		//NetworkServer.Spawn ((GameObject)Instantiate (hazard, spawnPosition, spawnRotation));
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

	[ClientRpc]
	public void RpcGameOver ()
	{
		//gameOverText.text = "Game Over!\nYou Lose";
		gameOver = true;

		// Load end scene
		int endLevel = 0;
		loadingImage.SetActive(true);
		NetworkServer.Shutdown();


		//SceneManager.LoadScene(endLevel);
	}

	public void RestartGame() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
