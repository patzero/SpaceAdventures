using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	public Rigidbody rb;
	public float speed;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		// Set the movement speed
		rb.velocity = transform.forward * speed;
	}
	
    /*public static void moveTest(Vector2 currentTouchPosition)
    {
        rb.position = currentTouchPosition;
    }*/

}
