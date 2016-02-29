using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkTransform))]
public class NetworkHazard : NetworkBehaviour
{
	public float tumble;
	public GameObject explosion;
	public int scoreValue;
	public int damageValue;

	protected bool _isDestroyed = false;
	protected NetworkTransform _netTransform;

	void Start()
	{
		_netTransform = GetComponent<NetworkTransform>();

		Rigidbody asteroidRB = GetComponent<Rigidbody>();
		asteroidRB.angularVelocity = Random.insideUnitSphere * tumble; 
	}

	[ServerCallback]
	void OnTriggerEnter(Collider other) 
	{
		if (other.tag == "Bolt")
		{
			// When get hit by asteroid
			NetworkBolt bolt = other.GetComponent<NetworkBolt>();
			NetworkPlayer player = bolt.owner.GetComponent<NetworkPlayer>();
			player.AddScore (scoreValue);
			CmdExplosion ();
		}
		else if(other.tag == "Player")
		{
			//we collided with the player,
			//Debug.Log("Hit by Player");
			NetworkPlayer player = other.GetComponent<NetworkPlayer>();
			player.TakeDamage(damageValue);
			player.AddScore (scoreValue);
			CmdExplosion ();
		}
	}

	public void MakeExplosion() {
		GameObject explo = Instantiate(explosion, transform.position, transform.rotation) as GameObject;
		Destroy(gameObject);
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
