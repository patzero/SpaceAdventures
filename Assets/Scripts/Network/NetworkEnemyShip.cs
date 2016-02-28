using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkTransform))]
public class NetworkEnemyShip : NetworkBehaviour
{
	private Rigidbody rb;
	private float currentSpeed;
	private float targetManeuver;

	public float dodge;
	public float smoothing;
	public float tilt;
	public Vector2 startWait;
	public Vector2 maneuverTime;
	public Vector2 maneuverWait;
	public Boundary boundary;

	public GameObject explosion;
	public int scoreValue;
	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	public float delay;

	private AudioSource audioSource;

	void Start()
	{
		audioSource = GetComponent<AudioSource> ();
		rb = GetComponent<Rigidbody>();
		currentSpeed = rb.velocity.z;
		StartCoroutine (Evade ());

		InvokeRepeating ("Fire", delay, fireRate);
	}

	IEnumerator Evade()
	{
		yield return new WaitForSeconds (Random.Range (startWait.x, startWait.y));

		while (true)
		{
			targetManeuver = Random.Range (1, dodge) * -Mathf.Sign (transform.position.x);
			yield return new WaitForSeconds (Random.Range (maneuverTime.x, maneuverTime.y));
			targetManeuver = 0;
			yield return new WaitForSeconds (Random.Range (maneuverWait.x, maneuverWait.y));
		}
	}

	void FixedUpdate ()
	{
		float newManeuver = Mathf.MoveTowards (rb.velocity.x, targetManeuver, Time.deltaTime * smoothing);
		rb.velocity = new Vector3 (newManeuver, 0.0f, currentSpeed);
		rb.position = new Vector3 
			(
				Mathf.Clamp (rb.position.x, boundary.xMin, boundary.xMax),
				0.0f,
				Mathf.Clamp (rb.position.z, boundary.zMin, boundary.zMax)
			);

		rb.rotation = Quaternion.Euler (0.0f, 0.0f, rb.velocity.x * -tilt);
	}

	void Fire ()
	{
		CmdFire ();

		/*//Instantiate (shot, shotSpawn.position, shotSpawn.rotation);
		NetworkServer.Spawn ((GameObject)Instantiate (shot, shotSpawn.position, shotSpawn.rotation));

		GameObject shot = Instantiate (shot, shotSpawn.position, shotSpawn.rotation) as GameObject;
		NetworkServer.Spawn (shot);
		audioSource.Play ();*/
	}

	/*[ServerCallback]
	public void Update()
	{
		if (Mathf.Abs(transform.position.x) > Camera.main.orthographicSize * Camera.main.aspect ||
			Mathf.Abs(transform.position.z) > Camera.main.orthographicSize)
		{//we are out of the screen, DESTROY
			NetworkServer.Destroy(gameObject);
		}
	}*/


	[ServerCallback]
	void OnTriggerEnter(Collider other) 
	{
		if (other.tag == "Bolt")
		{
			//Debug.Log("Hit by Bolt");
			/*NetworkSpaceshipBullet bullet = collision.gameObject.GetComponent<NetworkSpaceshipBullet>();
			bullet.owner.score += level;

			Explode();*/

			NetworkBolt bolt = other.GetComponent<NetworkBolt>();
			//bolt.owner.score += scoreValue;
			NetworkPlayer player = bolt.owner.GetComponent<NetworkPlayer>();
			player.AddScore (scoreValue);
			CmdExplosion ();
		}
		else if(other.tag == "Player")
		{
			//we collided with the player,
			//Debug.Log("Hit by Player");
			NetworkPlayer player = other.GetComponent<NetworkPlayer>();
			player.TakeDamage(20);
			player.AddScore (scoreValue);
			CmdExplosion ();
		}
	}

	public void CreateBullets()
	{

		GameObject enemyShot = Instantiate (shot, shotSpawn.position, shotSpawn.rotation) as GameObject;
		audioSource.Play ();
	}

	[Command]
	public void CmdFire()
	{
		if (!isClient) //avoid to create bullet twice (here & in Rpc call) on hosting client
			CreateBullets();

		RpcFire();
	}

	[ClientRpc]
	public void RpcFire()
	{
		CreateBullets();
	}


	public void MakeExplosion() {
		GameObject explo = Instantiate(explosion, transform.position, transform.rotation) as GameObject;
		Destroy(gameObject);
		//NetworkServer.Spawn(explo);
		//NetworkServer.Destroy(gameObject);
	}

	[Command]
	public void CmdExplosion()
	{
		if (!isClient) //avoid to create bullet twice (here & in Rpc call) on hosting client
			MakeExplosion();

		RpcExplosion();
	}


	[ClientRpc]
	public void RpcExplosion()
	{
		MakeExplosion();
	}

}

