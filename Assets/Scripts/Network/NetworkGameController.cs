
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Network;
using System.Collections;
using System.Collections.Generic;

public class NetworkGameController : NetworkBehaviour
{
	static public List<NetworkPlayer> sShips = new List<NetworkPlayer>();
	static public NetworkGameController sInstance = null;

	public GameObject uiScoreZone;
	public Font uiScoreFont;

	[Header("Gameplay")]
	//Those are sorte dby level 0 == lowest etc...
	//public GameObject[] asteroidPrefabs;
	public GameObject[] hazards;
	public Vector3 spawnValues;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;
	public float warpTime;

	[Space]

	protected bool _spawningAsteroid = true;
	protected bool _running = true;

	void Awake()
	{
		sInstance = this;
	}

	void Start()
	{
		if (isServer)
		{
			//StartCoroutine(AsteroidCoroutine());
			StartCoroutine(SpawnWaves());
		}

		for(int i = 0; i < sShips.Count; ++i)
		{
			sShips[i].Init();
		}
	}

	[ServerCallback]
	void Update()
	{
		if (!_running || sShips.Count == 0)
			return;

		bool allDestroyed = true;
		for (int i = 0; i < sShips.Count; ++i)
		{
			//allDestroyed &= (sShips[i].lifeCount == 0);
			if (!sShips[i].playerIsKill()){
				allDestroyed = false;
			}
		}

		if(allDestroyed)
		{
			Debug.Log ("All kill");
			StartCoroutine(ReturnToLoby());
		}
	}

	public override void OnStartClient()
	{
		base.OnStartClient();

		/*foreach (GameObject obj in asteroidPrefabs)
		{
			ClientScene.RegisterPrefab(obj);
		}*/

		foreach (GameObject obj in hazards) {
			ClientScene.RegisterPrefab (obj);
		}

	}

	IEnumerator ReturnToLoby()
	{
		_running = false;
		yield return new WaitForSeconds(3.0f);
		LobbyManager.s_Singleton.ServerReturnToLobby();
	}

	/*IEnumerator AsteroidCoroutine()
	{
		const float MIN_TIME = 5.0f;
		const float MAX_TIME = 10.0f;

		while(_spawningAsteroid)
		{
			yield return new WaitForSeconds(Random.Range(MIN_TIME, MAX_TIME));

			Vector2 dir = Random.insideUnitCircle;
			Vector3 position = Vector3.zero;

			if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
			{//make it appear on the side
				position = new Vector3( Mathf.Sign(dir.x)* Camera.main.orthographicSize * Camera.main.aspect, 
					0, 
					dir.y * Camera.main.orthographicSize);
			}
			else
			{//make it appear on the top/bottom
				position = new Vector3(dir.x * Camera.main.orthographicSize * Camera.main.aspect, 
					0,
					Mathf.Sign(dir.y) * Camera.main.orthographicSize);
			}

			//offset slightly so we are not out of screen at creation time (as it would destroy the asteroid right away)
			position -= position.normalized * 0.1f;


			GameObject ast = Instantiate(asteroidPrefabs[asteroidPrefabs.Length - 1], position, Quaternion.Euler(Random.value * 360.0f, Random.value * 360.0f, Random.value * 360.0f)) as GameObject;

			NetworkAsteroid asteroid = ast.GetComponent<NetworkAsteroid>();
			asteroid.SetupStartParameters(-position.normalized * 1000.0f, Random.insideUnitSphere * Random.Range(500.0f, 1500.0f));

			NetworkServer.Spawn(ast);
		}
	}*/
	
	
	public IEnumerator WaitForRespawn(NetworkPlayer player)
	{
		yield return new WaitForSeconds(4.0f);

		player.Respawn();
	}

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
				GameObject hazardObject = Instantiate (hazard, spawnPosition, spawnRotation) as GameObject;
				NetworkServer.Spawn (hazardObject);

				// Wait time for each ateroid spawn
				yield return new WaitForSeconds (spawnWait);
			}
			// Wait time for each wave  
			yield return new WaitForSeconds (waveWait);

		}
	}
}

