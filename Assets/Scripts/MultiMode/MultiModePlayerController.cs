using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
	
[System.Serializable]
public class MultiBoundary
{
	public float xMin, xMax, zMin, zMax;
}

public class MultiModePlayerController : NetworkBehaviour {

	private Rigidbody rb;
	private AudioSource audioPlayer;
	private float nextFire;
	private Quaternion calibrationQuaternion;

	public float speed;
	public float tilt;
	public Boundary boundary;

	public GameObject shot;
	public Transform shotSpawn1;
	public Transform shotSpawn2;
	public float fireRate;
	public SimpleTouchPad touchPad;

	[SyncVar(hook="OnDamage")]
	public int health = 100;

	[SyncVar(hook="OnScore")]
	public int score = 0;

	[SyncVar]
	public string playerName;

	public Texture box;
	public GameObject playerExplosion;

	public float smoothTime;
	private Vector3 velocity = Vector3.zero;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		audioPlayer = GetComponent<AudioSource> ();
	}

	//Used to calibrate the Input.acceleration input
	void CalibrateAccelerometer () {
		Vector3 accelerationSnapshot = Input.acceleration;
		Quaternion rotateQuaternion = Quaternion.FromToRotation (new Vector3 (0.0f, 0.0f, -1.0f), accelerationSnapshot);
		calibrationQuaternion = Quaternion.Inverse (rotateQuaternion);
	}

	//Get the 'calibrated' value from the Input
	Vector3 FixAcceleration (Vector3 acceleration) {
		Vector3 fixedAcceleration = calibrationQuaternion * acceleration;
		return fixedAcceleration;
	}

	void Update() {
		if (!isLocalPlayer) {
			return;
		}

		// Instancetiate the shot when the user press on fire button
		if (Input.GetButton("Fire1") && Time.time > nextFire) {
			CmdFire ();
		}
	}

	[Command]
	void CmdFire() {
		if (Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			GameObject missile1 = Instantiate(shot, shotSpawn1.position, shotSpawn1.rotation) as GameObject;
			GameObject missile2 = Instantiate(shot, shotSpawn2.position, shotSpawn2.rotation) as GameObject;
			audioPlayer.Play ();

			NetworkServer.Spawn (missile1);
			NetworkServer.Spawn (missile2);
		}
	}

	void FixedUpdate() {
		
		// Check if the player is a local player (multi mode)
		if (!isLocalPlayer) {
			return;
		}

		float moveHorizontal =   Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		//if (moveHorizontal != 0 || moveVertical != 0) {
			Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
			rb.velocity = movement * speed;
		//}

		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
		{
			Touch myTouch = Input.GetTouch(0);
			Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(myTouch.position.x, myTouch.position.y, 5.0f));
			//rb.position = new Vector3(pos.x, pos.y, pos.z);
			transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smoothTime);
		}
			
		// Limite the ship position to not going outside of game field
		// by clamp the x and z values of the ship to boundary min and max values
		rb.position = new Vector3 (
			Mathf.Clamp (rb.position.x, boundary.xMin, boundary.xMax),
			0.0f,
			Mathf.Clamp (rb.position.z, boundary.zMin, boundary.zMax)
		);
			
		// Rotate the ship body when in movement left or right
		rb.rotation = Quaternion.Euler (0.0f, 0.0f, rb.velocity.x * -tilt);
	}

	/**
	 * Call whenever health value changed
	 * */
	void OnDamage(int newHealth) {
		if (newHealth < 100) {
			// Make Explosion
			Instantiate(playerExplosion, transform.position, transform.rotation);
		}
		health = newHealth;
	}

	public void TakeDamage(int amount) {
		if (!isServer) {
			return;
		}
		health = health - amount;
	}

	public int GetHealth() {
		return health;
	}

	void OnScore(int newScore) {
		score = newScore;
	}

	public void AddScore(int amount) {
		if (!isServer) {
			return;
		}
		score = score + amount;
	}

	public int GetScore() {
		return score;
	}

	void OnGUI () {
		Vector3 pos = Camera.main.WorldToScreenPoint (transform.position); 

		// Draw player name
		GUI.color = Color.black;
		GUI.Label(new Rect(pos.x - 30, Screen.height - pos.y + 40, 100, 30), playerName);

		GUI.color = Color.white;
		GUI.Label(new Rect(pos.x - 31, Screen.height - pos.y + 41, 100, 30), playerName);

		// Draw player score
		GUI.color = Color.black;
		GUI.Label(new Rect(pos.x - 30, Screen.height - pos.y + 55, 100, 30), "Score : " + score);

		GUI.color = Color.white;
		GUI.Label(new Rect(pos.x - 31, Screen.height - pos.y + 56, 100, 30), "Score : " + score);

		// Draw heath bar bg
		GUI.color = Color.grey;
		GUI.DrawTexture(new Rect(pos.x - 35, Screen.height - pos.y + 35, 50, 7), box);

		// Draw hp
		GUI.color = Color.green;
		GUI.DrawTexture(new Rect(pos.x - 36, Screen.height - pos.y + 36, health/2, 5), box);
	}
}
