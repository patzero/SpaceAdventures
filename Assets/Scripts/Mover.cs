using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	private Rigidbody rb;
	public float speed;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		// Set the movement of the bolt laser 
		rb.velocity = transform.forward * speed;
	}
	

}
