using UnityEngine;
using System.Collections;

public class NetworkBolt : MonoBehaviour
{
	public Vector3 originalDirection;

	//The spaceship that shoot that bullet, use to attribute point correctly
	public NetworkPlayer owner;

	/*void Start()
	{
		Destroy(gameObject, 3.0f);
		GetComponent<Rigidbody>().velocity = originalDirection * 200.0f;
		transform.forward = originalDirection;
	}*/


	void OnTriggerEnter(Collider other) 
	{
		if (other.tag == "Player" || other.tag == "Boundary" )
			return;
		
		Destroy(gameObject);
	}

}
