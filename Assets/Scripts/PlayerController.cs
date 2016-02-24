using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour {

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
//			Instantiate(shot, shotSpawn3.position, shotSpawn3.rotation);
//			Instantiate(shot, shotSpawn4.position, shotSpawn4.rotation);
//			Instantiate(shot, shotSpawn5.position, shotSpawn5.rotation);
//			Instantiate(shot, shotSpawn6.position, shotSpawn6.rotation);
			audioPlayer.Play();
		}
	}

	void FixedUpdate() {
		//float moveHorizontal =   Input.GetAxis ("Horizontal");
		//float moveVertical = Input.GetAxis ("Vertical");

		// Set the ship movement and the speed only for horizontal and vertical movement
		//Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		//Vector3 accelerationRaw = Input.acceleration;
		//Vector3 acceleration = FixAcceleration (accelerationRaw);

		//Vector2 direction = touchPad.GetDirection();
		//Vector3 movement = new Vector3 (direction.x, 0.0f, direction.y);

		//rb.velocity = movement * speed;
        var data = touchPad.GetPosition();
        //Debug.Log("touch_x: " + data.x);
        //Debug.Log("touch_y: " + data.y);
        //Debug.Log("faucon_x: " + rb.position.x);
        //Debug.Log("faucon_z: " + rb.position.z);
        //Debug.Log("boundary.xMin: " + boundary.xMin);
        //Debug.Log("boundary.xMax: " + boundary.xMax);
        //Debug.Log("boundary.zMin: " + boundary.zMin);
        //Debug.Log("boundary.zMax: " + boundary.zMax);

        //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        //{

        if (data.x > 0 || data.y > 0)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(data.x, data.y, 10.0f));
            //Debug.Log("worldPos.z: " + worldPos.z);
            //Debug.Log("worldPos.x: " + worldPos.x);
            //Debug.Log("rb.position.x: " + rb.position.x);
            //if (worldPos.x < rb.position.x + 1 && worldPos.x > rb.position.x - 1 && worldPos.z < rb.position.z + 1 && worldPos.z > rb.position.z - 1)
            rb.position = worldPos;
        }
            
        //}
            



        // Limite the ship position to not going outside of game field
        // by clamp the x and z values of the ship to boundary min and max values
        rb.position = new Vector3 (
			Mathf.Clamp (rb.position.x, boundary.xMin, boundary.xMax),
			0.0f,
			Mathf.Clamp (rb.position.z, boundary.zMin, boundary.zMax)
		);
        /*var cam = new Camera();
        if (cam != null)
        {
            rb.position = cam.ScreenToWorldPoint(new Vector3(
            Mathf.Clamp(data.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(data.y, boundary.zMin, boundary.zMax)
        ));
        }*/

        /*if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            // Get movement of the finger since last frame
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            Vector3 touchPosition = new Vector3(Mathf.Clamp(touchDeltaPosition.x, boundary.xMin, boundary.xMax), transform.position.y, Mathf.Clamp(touchDeltaPosition.y, boundary.zMin, boundary.zMax));
            //touchPosition.Set();
            // Move object across XY plane
            transform.position = Vector3.Lerp(transform.position, touchPosition, Time.deltaTime * speed);
                                        rb.position = transform.position;
        }*/



        // Rotate the ship body when in movement left or right
        rb.rotation = Quaternion.Euler (0.0f, 0.0f, rb.velocity.x * -tilt);
	}
}
 