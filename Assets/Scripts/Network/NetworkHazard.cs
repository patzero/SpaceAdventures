using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkTransform))]
public class NetworkHazard : NetworkBehaviour
{
	//current level of asteroid. When destroyed it spawn a number of
	//other asteroid of lower level except if level == 1, then destroyed
	public int level = 2;
	public float tumble;
	public GameObject explosion;
	public int scoreValue;

	protected bool _isDestroyed = false;
	protected NetworkTransform _netTransform;

	[SyncVar]
	protected Vector3 originalForce;
	[SyncVar]
	protected Vector3 originalTorque;

	[Server]
	public void SetupStartParameters(Vector3 force, Vector3 torque)
	{
		originalForce = force;
		originalTorque = torque;
	}

	void Start()
	{
		_netTransform = GetComponent<NetworkTransform>();

		Rigidbody asteroidRB = GetComponent<Rigidbody>();

		//asteroidRB.AddForce(originalForce);
		//asteroidRB.AddTorque(originalTorque);
		asteroidRB.angularVelocity = Random.insideUnitSphere * tumble; 

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


//	[ServerCallback]
//	void OnCollisionEnter(Collision collision)
//	{
//		//we collide so we dirty the NetworkTrasnform to sync it on clients.
//		//_netTransform.SetDirtyBit(1);
//
//		Debug.Log("Has Collision");
//
//		if (collision.gameObject.tag == "Bolt")
//		{
//			Debug.Log("Hit by Bolt");
//			/*NetworkSpaceshipBullet bullet = collision.gameObject.GetComponent<NetworkSpaceshipBullet>();
//
//			bullet.owner.score += level;
//
//			Explode();*/
//		}
//		else if(collision.gameObject.tag == "Player")
//		{//we collided with the player, destroy it.
//			Debug.Log("Hit by Player");
//			/*NetworkSpaceship ship = collision.gameObject.GetComponent<NetworkSpaceship>();
//			ship.Kill();
//			Explode();*/
//		}
//	}

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
			//player.Kill();
			player.TakeDamage(20);
			player.AddScore (scoreValue);
			//Explode ();
			CmdExplosion ();

			//var playerController = other.GetComponent <MultiModePlayerController>();
			//player.TakeDamage(20);
			//playerController.AddScore (scoreValue);

//			int playerHealth = playerController.GetHealth ();
//			if (playerHealth <= 0) {
//				GameObject explo = Instantiate(playerExplosion, transform.position, transform.rotation) as GameObject;
//				NetworkServer.Spawn (explo);
//				//Destroy(explo, 1.0f);
//				Destroy(other.gameObject);
//				gameController.RpcGameOver ();
//				//MasterServer.UnregisterHost ();
//			}
//			Destroy(gameObject);

			//Explode();
		}
	}
		

	//this explode the asteroid (so split it if level > 1 else just destroy it)
	[Server]
	public void Explode()
	{
		//if the 2 bullet touch the asteroid the same frame, they will both generate the callback before the asteroid get destroyed
		//but we want to destroy it only once.
		if (_isDestroyed)
			return;

		_isDestroyed = true;

		/*int targetLevel = level - 1;
		if (targetLevel > 0)
		{
			//pick a number between 3 & 5 small asteroids to spawn
			int numberToSpawn = Random.Range(3, 6);

			for (int i = 0; i < numberToSpawn; ++i)
			{
				Vector3 force = Quaternion.Euler(0, i * 360.0f / numberToSpawn, 0) * Vector3.forward * Random.Range(0.5f, 1.5f) * 300.0f; ;

				GameObject newGO = Instantiate(NetworkGameManager.sInstance.asteroidPrefabs[targetLevel - 1], transform.position + force.normalized * 5.0f, Quaternion.Euler(0, Random.value * 180.0f, 0)) as GameObject;
				NetworkAsteroid asteroid = newGO.GetComponent<NetworkAsteroid>();

				//we slice a 360 angle in numberToSpawn part & send an asteroid for each
				asteroid.originalForce = force;
				asteroid.originalTorque = Random.insideUnitSphere * Random.Range(500.0f, 1500.0f);

				NetworkServer.Spawn(newGO);
			}
		}

		//destroy that asteroid too
		NetworkServer.Destroy(gameObject);*/

		GameObject explo = Instantiate(explosion, transform.position, transform.rotation) as GameObject;
		NetworkServer.Spawn(explo);

		NetworkServer.Destroy(gameObject);
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
