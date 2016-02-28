using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(Rigidbody))]
public class NetworkPlayer : NetworkBehaviour
{
	public float rotationSpeed = 45.0f;
	public float speed = 2.0f;
	public float maxSpeed = 3.0f;

	public ParticleSystem killParticle;
	public GameObject trailGameobject;
	public GameObject bulletPrefab;

	//Network syncvar
	[SyncVar(hook = "OnScoreChanged")]
	public int score;
	[SyncVar]
	public Color color;
	[SyncVar]
	public string playerName;
	[SyncVar(hook = "OnLifeChanged")]
	public int lifeCount;

	[SyncVar(hook="OnDamage")]
	public int health = 100;


	protected Rigidbody _rigidbody;
	protected Collider _collider;
	protected Text _scoreText;

	protected float _rotation = 0;
	protected float _acceleration = 0;

	protected float _shootingTimer = 0;

	protected bool _canControl = true;

	//hard to control WHEN Init is called (networking make order between object spawning non deterministic)
	//so we call init from multiple location (depending on what between spaceship & manager is created first).
	protected bool _wasInit = false;

	//******//
	private float nextFire;
	private Vector3 velocity = Vector3.zero;
	private AudioSource audioPlayer;
	private bool isKill = false;

	public GameObject bolt;
	public Transform shotSpawn1;
	public Transform shotSpawn2;
	public float tilt;
	public Boundary boundary;
	public float smoothTime;
	public float fireRate;
	public Texture box;
	public GameObject playerExplosion;

	void Awake()
	{
		//register the spaceship in the gamemanager, that will allow to loop on it.
		NetworkGameController.sShips.Add(this);
	}

	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_collider = GetComponent<Collider>();
		audioPlayer = GetComponent<AudioSource> ();
	
		/*Renderer[] rends = GetComponentsInChildren<Renderer>();
		foreach (Renderer r in rends)
			r.material.color = color;*/

		//We don't want to handle collision on client, so disable collider there
		_collider.enabled = isServer;


		if (NetworkGameController.sInstance != null)
		{//we MAY be awake late (see comment on _wasInit above), so if the instance is already there we init
			//Init();
		}
	}

	public void Init()
	{
		if (_wasInit)
			return;

		/*GameObject scoreGO = new GameObject(playerName + "score");
		scoreGO.transform.SetParent(NetworkGameController.sInstance.uiScoreZone.transform, false);
		_scoreText = scoreGO.AddComponent<Text>();
		_scoreText.alignment = TextAnchor.MiddleCenter;
		_scoreText.font = NetworkGameController.sInstance.uiScoreFont;
		_scoreText.resizeTextForBestFit = true;
		_scoreText.color = color;
		_wasInit = true;

		UpdateScoreLifeText();*/
	}

	void OnDestroy()
	{
		NetworkGameController.sShips.Remove(this);
	}

	[ClientCallback]
	void Update()
	{
		_rotation = 0;
		_acceleration = 0;

		if (!isLocalPlayer || !_canControl)
			return;

		/*_rotation = Input.GetAxis("Horizontal");
		_acceleration = Input.GetAxis("Vertical");


		if(Input.GetButton("Jump") && _shootingTimer <= 0)
		{
			_shootingTimer = 0.2f;
			//we call a Command, that will be executed only on server, to spawn a new bullet
			//we pass the position&forward to be sure to shoot from the right one (server can lag one frame behind)
			CmdFire(transform.position, transform.forward, _rigidbody.velocity);
		}

		if (_shootingTimer > 0)
			_shootingTimer -= Time.deltaTime;*/

		// Instancetiate the shot when the user press on fire button
		if (Input.GetButton("Fire1") && Time.time > nextFire) {
			CmdFire ();
		}
	}


	[ClientCallback]
	void FixedUpdate()
	{
		if (!hasAuthority)
			return;

		if (!_canControl)
		{//if we can't control, mean we're destroyed, so make sure the ship stay in spawn place
			_rigidbody.rotation = Quaternion.identity;
			_rigidbody.position = Vector3.zero;
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.angularVelocity = Vector3.zero;
		}
		else
		{
			/*Quaternion rotation = _rigidbody.rotation * Quaternion.Euler(0, _rotation * rotationSpeed * Time.fixedDeltaTime, 0);
			_rigidbody.MoveRotation(rotation);

			_rigidbody.AddForce((rotation * Vector3.forward) * _acceleration * 1000.0f * speed * Time.deltaTime);

			if (_rigidbody.velocity.magnitude > maxSpeed * 1000.0f)
			{
				_rigidbody.velocity = _rigidbody.velocity.normalized * maxSpeed * 1000.0f;
			}

			CheckExitScreen();*/

			// Player Movement
			float moveHorizontal =   Input.GetAxis ("Horizontal");
			float moveVertical = Input.GetAxis ("Vertical");

			//if (moveHorizontal != 0 || moveVertical != 0) {
			Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
			_rigidbody.velocity = movement * speed;
			//}

			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				Touch myTouch = Input.GetTouch(0);
				Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(myTouch.position.x, myTouch.position.y, 5.0f));
				//rb.position = new Vector3(pos.x, pos.y, pos.z);
				transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smoothTime);
			}

			// Rotate the ship body when in movement left or right
			_rigidbody.rotation = Quaternion.Euler (0.0f, 0.0f, _rigidbody.velocity.x * -tilt);

			// Check Exit Screen
			// Limite the ship position to not going outside of game field
			// by clamp the x and z values of the ship to boundary min and max values
			_rigidbody.position = new Vector3 (
				Mathf.Clamp (_rigidbody.position.x, boundary.xMin, boundary.xMax),
				0.0f,
				Mathf.Clamp (_rigidbody.position.z, boundary.zMin, boundary.zMax)
			);
				

		}
	}

	[ClientCallback]
	void OnCollisionEnter(Collision coll)
	{
		if (isServer)
			return; // hosting client, server path will handle collision

		//if not, we are on a client, so just disable the spaceship (& play destruction aprticle).
		//This way client won't see it's destruction delayed (time for it to happen on server & message to get back)
		/*NetworkAsteroid asteroid = coll.gameObject.GetComponent<NetworkAsteroid>();

		if(asteroid != null)
		{
			LocalDestroy();
		}*/
	}

	//[ClientCallback]
	void OnTriggerEnter(Collider other)
	{
		Debug.Log ("Got hit by enemy");
		//if (isServer)
			//return; // hosting client, server path will handle collision

		if (other.tag == "BoltEnemy")
		{
			//Debug.Log ("Got hit by bolt enemy");

			//Hit by bolt enemy
			TakeDamage (10);
			MakePlayerExplosion ();
		}
	}

	void CheckExitScreen()
	{
		if (Camera.main == null)
			return;

		if (Mathf.Abs(_rigidbody.position.x) > Camera.main.orthographicSize * Camera.main.aspect)
		{
			_rigidbody.position = new Vector3(-Mathf.Sign(_rigidbody.position.x) * Camera.main.orthographicSize * Camera.main.aspect, 0, _rigidbody.position.z);
			_rigidbody.position -= _rigidbody.position.normalized * 0.1f; // offset a little bit to avoid looping back & forth between the 2 edges 
		}

		if (Mathf.Abs(_rigidbody.position.z) > Camera.main.orthographicSize)
		{
			_rigidbody.position = new Vector3(_rigidbody.position.x , _rigidbody.position.y, -Mathf.Sign(_rigidbody.position.z) * Camera.main.orthographicSize);
			_rigidbody.position -= _rigidbody.position.normalized * 0.1f; // offset a little bit to avoid looping back & forth between the 2 edges 
		}
	}


	// --- Score & Life management & display
	void OnScoreChanged(int newValue)
	{
		score = newValue;
		UpdateScoreLifeText();
	}

	void OnLifeChanged(int newValue)
	{
		lifeCount = newValue;
		UpdateScoreLifeText();
	}

	void OnDamage(int newHealth) {
		if (health <= 0) {
			MakePlayerExplosion ();
			EnableSpaceShip (false);
			isKill = true;
		}
		if (newHealth < 100) {
			// Make Explosion
			MakePlayerExplosion();
		}

		health = newHealth;
	}
		

	public int GetHealth() {
		return health;
	}

	void UpdateScoreLifeText()
	{
		if (_scoreText != null)
		{
			_scoreText.text = playerName + "\nSCORE : " + score + "\nLIFE : ";
			for (int i = 1; i <= lifeCount; ++i)
				_scoreText.text += "X";
		}
	}

	//===================================

	//We can't disable the whole object, as it would impair synchronisation/communication
	//So disabling mean disabling collider & renderer only
	public void EnableSpaceShip(bool enable)
	{
		//GetComponent<Renderer>().enabled = enable;
		transform.Find("Model").GetComponent<Renderer>().enabled = enable;
		_collider.enabled = isServer && enable;
		trailGameobject.SetActive(enable);

		_canControl = enable;
	}

	[Client]
	public void LocalDestroy()
	{
		// Player Explosion
		GameObject explo = Instantiate(playerExplosion, transform.position, transform.rotation) as GameObject;
		NetworkServer.Spawn(explo);


		/*killParticle.transform.SetParent(null);
		killParticle.transform.position = transform.position;
		killParticle.gameObject.SetActive(true);
		killParticle.Stop();
		killParticle.Play();*/

		if (!_canControl)
			return;//already destroyed, happen if destroyed Locally, Rpc will call that later

		EnableSpaceShip(false);
	}

	//this tell the game this should ONLY be called on server, will ignore call on client & produce a warning
	[Server]
	public void Kill()
	{
		lifeCount -= 1;

		RpcDestroyed();
		EnableSpaceShip(false);
		isKill = true;
		if (lifeCount > 0)
		{
			//we start the coroutine on the manager, as disabling a gameobject stop ALL coroutine started by it
			NetworkGameController.sInstance.StartCoroutine(NetworkGameController.sInstance.WaitForRespawn(this));
		}
	}

	[Server]
	public void TakeDamage(int amount) {
		health = health - amount;
	}

	[Server]
	public void AddScore(int amount) {
		score = score + amount;
	}

	[Server]
	public void Respawn()
	{
		EnableSpaceShip(true);
		isKill = false;
		RpcRespawn();
	}

	public void CreateBullets()
	{
		/*Vector3[] vectorBase = { _rigidbody.rotation * Vector3.right, _rigidbody.rotation * Vector3.up, _rigidbody.rotation * Vector3.forward };
		Vector3[] offsets = { -1.5f * vectorBase[0] + -0.5f * vectorBase[2], 1.5f * vectorBase[0] + -0.5f * vectorBase[2] };

		for (int i = 0; i < 2; ++i)
		{
			GameObject bullet = Instantiate(bulletPrefab, _rigidbody.position + offsets[i], Quaternion.identity) as GameObject;
			NetworkSpaceshipBullet bulletScript = bullet.GetComponent<NetworkSpaceshipBullet>();

			bulletScript.originalDirection = vectorBase[2]; 
			bulletScript.owner = this;

			//NetworkServer.SpawnWithClientAuthority(bullet, connectionToClient);
		}*/

		if (Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			GameObject bolt1 = Instantiate(bolt, shotSpawn1.position, shotSpawn1.rotation) as GameObject;
			NetworkBolt bolt1Script = bolt1.GetComponent<NetworkBolt>();
			bolt1Script.owner = this;

			GameObject bolt2 = Instantiate(bolt, shotSpawn2.position, shotSpawn2.rotation) as GameObject;
			NetworkBolt bolt2Script = bolt2.GetComponent<NetworkBolt>();
			bolt2Script.owner = this;

		
			audioPlayer.Play ();

			//NetworkServer.Spawn (missile1);
			//NetworkServer.Spawn (missile2);
		}
	}

	public bool playerIsKill() {
		return isKill;
	}

	void OnGUI () {
		if (isKill)
			return;
		
		Vector3 pos = Camera.main.WorldToScreenPoint (transform.position); 

		// Draw heath bar bg
		GUI.color = Color.grey;
		GUI.DrawTexture(new Rect(pos.x - 35, Screen.height - pos.y + 35, 50, 7), box);

		// Draw hp
		GUI.color = Color.green;
		GUI.DrawTexture(new Rect(pos.x - 36, Screen.height - pos.y + 36, health/2, 5), box);

		// Draw player name
		GUI.color = Color.black;
		GUI.Label(new Rect(pos.x - 30, Screen.height - pos.y + 40, 100, 30), playerName);

		GUI.color = color;
		GUI.Label(new Rect(pos.x - 31, Screen.height - pos.y + 41, 100, 30), playerName);

		// Draw player score
		/*GUI.color = Color.black;
		GUI.Label(new Rect(pos.x - 30, Screen.height - pos.y + 55, 100, 30), "Life : " + lifeCount);

		GUI.color = Color.white;
		GUI.Label(new Rect(pos.x - 31, Screen.height - pos.y + 56, 100, 30), "Life : " + lifeCount);*/

		// Draw player score
		GUI.color = Color.black;
		GUI.Label(new Rect(pos.x - 30, Screen.height - pos.y + 55, 100, 30), "Score : " + score);

		GUI.color = Color.white;
		GUI.Label(new Rect(pos.x - 31, Screen.height - pos.y + 56, 100, 30), "Score : " + score);
	}

	// =========== NETWORK FUNCTIONS

	/*[Command]
	public void CmdFire(Vector3 position, Vector3 forward, Vector3 startingVelocity)
	{
		if (!isClient) //avoid to create bullet twice (here & in Rpc call) on hosting client
			CreateBullets();

		RpcFire();
	}*/

	[Command]
	public void CmdFire()
	{
		if (!isClient) //avoid to create bullet twice (here & in Rpc call) on hosting client
			CreateBullets();

		RpcFire();
	}

	//
	[Command]
	public void CmdCollideAsteroid()
	{
		Kill();
	}

	[ClientRpc]
	public void RpcFire()
	{
		CreateBullets();
	}


	//called on client when the player die, spawn the particle (this is only cosmetic, no need to do it on server)
	[ClientRpc]
	void RpcDestroyed()
	{
		LocalDestroy();
	}

	[ClientRpc]
	void RpcRespawn()
	{
		EnableSpaceShip(true);

		//killParticle.gameObject.SetActive(false);
		//killParticle.Stop();
	}

	public void MakePlayerExplosion() {
		GameObject explo = Instantiate(playerExplosion, transform.position, transform.rotation) as GameObject;
		//Destroy(gameObject);
		//NetworkServer.Spawn(explo);
		//NetworkServer.Destroy(gameObject);
	}

	[Command]
	public void CmdPlayerExplosion()
	{
		if (!isClient) //avoid to create bullet twice (here & in Rpc call) on hosting client
			MakePlayerExplosion();

		RpcPlayerExplosion();
	}


	[ClientRpc]
	public void RpcPlayerExplosion()
	{
		MakePlayerExplosion();
	}
}

