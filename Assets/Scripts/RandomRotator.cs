using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour {

	private Rigidbody rb;
	public float tumble;

	void Start ()
	{
		rb = GetComponent<Rigidbody> ();

		// Randomly rotate the asteroid body
		rb.angularVelocity = Random.insideUnitSphere * tumble; 
	}
}
