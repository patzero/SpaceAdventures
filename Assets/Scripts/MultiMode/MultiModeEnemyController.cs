using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MultiModeEnemyController : NetworkBehaviour
{

	public GameObject shot;
	public Transform shotSpawn;
	public Transform shotSpawn2;

	public float fireRate;
	public float delay;

	private AudioSource audioSource;

	void Start ()
	{
		audioSource = GetComponent<AudioSource> ();
		InvokeRepeating ("Fire", delay, fireRate);
	}

	void Fire ()
	{
		if (!isServer)
			return;
		
		//Instantiate (shot, shotSpawn.position, shotSpawn.rotation);
		NetworkServer.Spawn ((GameObject)Instantiate (shot, shotSpawn.position, shotSpawn.rotation));

		GameObject shot1 = Instantiate (shot, shotSpawn.position, shotSpawn.rotation) as GameObject;
		NetworkServer.Spawn (shot1);
		if (shotSpawn2 != null) {
			GameObject shot2 = Instantiate(shot, shotSpawn2.position, shotSpawn2.rotation) as GameObject;
			NetworkServer.Spawn (shot2);
		}
		audioSource.Play ();
	}
}