using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour {

	private Rigidbody rb;
	private AudioSource audio;
	private float nextFire;
	private Quaternion calibrationQuaternion;

	public float speed;
	public float tilt;
	public Boundary boundary;

	public GameObject shot;
	public Transform shotSpawn;
	public Transform   shotSpawn2;
	public Transform shotSpawn3;
	public Transform shotSpawn4;
	public Transform shotSpawn5;
	public Transform shotSpawn6;
	public float fireRate;
	public  SimpleTouchPad touchPad;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		audio = GetComponent<AudioSource> ();
	}

	//Used to calibrate the Input.acceleration input
	void CalibrateAccelerometer () {
		Vector3 accelerationSnapshot = Input.acceleration;
		Quaternion rotateQuaternion = Quaternion.FromToRotation (new Vector3 (0.0f, 0.0f, -1.0f), accelerationSnapshot);
		calibrationQuaternion = Quaternion.Inverse (rotateQuaternion);
	}

	//Get the 'calibrated' value from the Input
	Vector3 FixAcceleration (Vector3 acceleration) {
		Vector3 fixedAcceleration = calibrationQuaternion * acceleration;
		return fixedAcceleration;
	}

	void Update() {
		// Instancetiate the shot when the user press on fire button
		if (Input.GetButton("Fire1") && Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
			Instantiate(shot, shotSpawn2.position, shotSpawn2.rotation);
//			Instantiate(shot, shotSpawn3.position, shotSpawn3.rotation);
//			Instantiate(shot, shotSpawn4.position, shotSpawn4.rotation);
//			Instantiate(shot, shotSpawn5.position, shotSpawn5.rotation);
//			Instantiate(shot, shotSpawn6.position, shotSpawn6.rotation);
			audio.Play();
		}
	}

	void FixedUpdate() {
		//float moveHorizontal =   Input.GetAxis ("Horizontal");
		//float moveVertical = Input.GetAxis ("Vertical");

		// Set the ship movement and the speed only for horizontal and vertical movement
		//Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		//Vector3 accelerationRaw = Input.acceleration;
		//Vector3 acceleration = FixAcceleration (accelerationRaw);

		Vector2 direction = touchPad.GetDirection ();
		Vector3 movement = new Vector3 (direction.x, 0.0f, direction.y);


//		if (Input.touchCount > 0) {
//			// The screen has been touched so store the touch
//			Touch touch = Input.GetTouch(0);
//			if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) {
//				// If the finger is on the screen, move the object smoothly to the touch position
//				Plane plane = new Plane(Vector3.up, transform.position);
//				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
//				float dist;
//				if (plane.Raycast (ray, out dist)) {
//					transform.position = ray.GetPoint (dist);
//				}
//			}
//		}

		rb.velocity = movement * speed;

		// Limite the ship position to not going outside of game field
		// by clamp the x and z values of the ship to boundary min and max values
		rb.position = new Vector3 (
			Mathf.Clamp (rb.position.x, boundary.xMin, boundary.xMax),
			0.0f,
			Mathf.Clamp (rb.position.z, boundary.zMin, boundary.zMax)
		);

		// Rotate the ship body when in movement left or right
		rb.rotation = Quaternion.Euler (0.0f, 0.0f, rb.velocity.x * -tilt);
	}
}
 