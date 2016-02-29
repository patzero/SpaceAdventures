using UnityEngine;
using System.Collections;

public class NetworkBolt : MonoBehaviour
{
	public Vector3 originalDirection;

	//The spaceship that shoot that bullet, use to attribute point correctly
	public NetworkPlayer owner;

	void OnTriggerEnter(Collider other) 
	{
		if (other.tag == "Player" || other.tag == "Boundary" )
			return;
		
		Destroy(gameObject);
	}

}
