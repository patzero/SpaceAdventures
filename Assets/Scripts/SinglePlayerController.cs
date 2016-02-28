using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;
}

public class SinglePlayerController : MonoBehaviour {

	private Rigidbody rb;
	private AudioSource audioPlayer;
	private float nextFire;
	private Quaternion calibrationQuaternion;

	public float speed;
	public float tilt;
	public Boundary boundary;

	public GameObject shot;
	public Transform shotSpawn;
	public Transform shotSpawn2;
	public float fireRate;
	public SimpleTouchPad touchPad;
	public float smoothTime;
	private Vector3 velocity = Vector3.zero;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		audioPlayer = GetComponent<AudioSource> ();
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
			audioPlayer.Play();
		}
	}

	void FixedUpdate() {
		float moveHorizontal =   Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		//if (moveHorizontal != 0 || moveVertical != 0) {
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		rb.velocity = movement * speed;
		//}

		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
		{
			Touch myTouch = Input.GetTouch(0);
			Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(myTouch.position.x, myTouch.position.y, 5.0f));
			//rb.position = new Vector3(pos.x, pos.y, pos.z);
			transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smoothTime);
		}

		// Limite the ship position to not going outside of game field
		// by clamp the x and z values of the ship to boundary min and max values
		rb.position = new Vector3 (
			Mathf.Clamp (rb.position.x, boundary.xMin, boundary.xMax),
			0.0f,
			Mathf.Clamp (rb.position.z, boundary.zMin, boundary.zMax)
		);

		// Rotate the ship body when in movement left or right
		rb.rotation = Quaternion.Euler (0.0f, 0.0f, rb.velocity.x * -tilt);

        /*var data = touchPad.GetPosition();
        if (data.x > 0 || data.y > 0)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(data.x, data.y, 10.0f));
            //Debug.Log("worldPos.z: " + worldPos.z);
            //Debug.Log("worldPos.x: " + worldPos.x);
            //Debug.Log("rb.position.x: " + rb.position.x);
            //if (worldPos.x < rb.position.x + 1 && worldPos.x > rb.position.x - 1 && worldPos.z < rb.position.z + 1 && worldPos.z > rb.position.z - 1)
            rb.position = worldPos;
        }
        // Limite the ship position to not going outside of game field
        // by clamp the x and z values of the ship to boundary min and max values
        rb.position = new Vector3 (
			Mathf.Clamp (rb.position.x, boundary.xMin, boundary.xMax),
			0.0f,
			Mathf.Clamp (rb.position.z, boundary.zMin, boundary.zMax)
		);
        // Rotate the ship body when in movement left or right
        rb.rotation = Quaternion.Euler (0.0f, 0.0f, rb.velocity.x * -tilt);*/
	}
}
 