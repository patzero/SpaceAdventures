using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	private Rigidbody rb;
	public float speed;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		// Set the movement speed
		rb.velocity = transform.forward * speed;
	}
	

}
