using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject[] hazards;
    public Vector3 spawnValues;
    public Text scoreText;
    public Text gameOverText;
    public GameObject restartButton;
    public GameObject exitButton;
    public AudioClip gameOverAudio;
    public int hazardCount;
    public float spawnWait;
    public float startWait;
    public float waveWait;
    public float warpTime;

    private GameObject backgroundObject;
    private GameObject engine1Object;
    private GameObject engine2Object;
    private BGScroller bgScroller;
    private AudioSource audioSound;
    private bool gameOver;
    //private bool restart;
    private int score;

    void Start()
    {
        gameOver = false;
        //restart = false;
        //restartText.text = "";
        gameOverText.text = "";
        restartButton.SetActive(false);
        exitButton.SetActive(false);
        score = 0;
        UpdateScore();

        // Load audio sound
        audioSound = GetComponent<AudioSource>();

        // Get the game controller 
        backgroundObject = GameObject.FindWithTag("BackGround");
        if (backgroundObject != null)
        {
            bgScroller = backgroundObject.GetComponent<BGScroller>();
            backgroundObject.transform.localScale += new Vector3(0, 1400f, 0);
            bgScroller.SetScrollSpeed(-70);
        }

        StartCoroutine(HyperSpaceWarp());
        StartCoroutine(SpawnWaves());
    }

    IEnumerator HyperSpaceWarp()
    {
        //yield return new WaitForSeconds(warpTime);
        //bgScroller.SetScrollSpeed(-1);
        
        yield return new WaitForSeconds(warpTime);
        backgroundObject.transform.localScale += new Vector3(0, -1400f, 0);
        bgScroller.SetScrollSpeed(-2);

        engine1Object = GameObject.FindWithTag("Engine 1");
        engine2Object = GameObject.FindWithTag("Engine 2");

        if (engine1Object != null)
            engine1Object.SetActive(false);

        if (engine2Object != null)
            engine2Object.SetActive(false);
        //yield return new WaitForSeconds(1);
        //bgScroller.SetScrollSpeed(-1);
        //yield return new WaitForSeconds(1);
        //bgScroller.SetScrollSpeed(-1);
    }
    /**
	 * Spawn a wave of asteroid 
	 * */
    IEnumerator SpawnWaves()
    {
        // Wait time when the game started to spawn the wave of asteroid
        yield return new WaitForSeconds(startWait);
        while (true)
        {
            for (int i = 0; i < hazardCount; i++)
            {
                // Create the position random between the edge of game screen
                GameObject hazard = hazards[Random.Range(0, hazards.Length)];
                Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(hazard, spawnPosition, spawnRotation);
                // Wait time for each ateroid spawn
                yield return new WaitForSeconds(spawnWait);
            }
            // Wait time for each wave  
            yield return new WaitForSeconds(waveWait);

            if (gameOver)
            {
                restartButton.SetActive(true);
                exitButton.SetActive(true);

                // restartText.text = "Press 'R' for Restart";
                //restart = true;
                break;
            }
        }
    }

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
		if (score < 0) 
			score = 0;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        audioSound.clip = gameOverAudio;
        audioSound.Play();
        gameOverText.text = "Game over";
        gameOver = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool IsGameOver()
    {
        return gameOver;
    }
}
