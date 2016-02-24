using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour 
{

	public GameObject shot;
    private GameObject gameObj;
    private GameController gameController;
    public Transform shotSpawn;
    public Transform shotSpawn2;
    public float fireRate;
	public float delay;

	private AudioSource audioSource;

	void Start ()
	{
        gameObj = GameObject.FindWithTag("GameController");
        if (gameObj != null)
            gameController = gameObj.GetComponent<GameController>();
        audioSource = GetComponent<AudioSource> ();
		InvokeRepeating ("Fire", delay, fireRate);
	}

	void Fire ()
	{
        if (gameController != null && !gameController.IsGameOver())
        {
            Instantiate (shot, shotSpawn.position, shotSpawn.rotation);
            if (shotSpawn2 != null)
                Instantiate(shot, shotSpawn2.position, shotSpawn2.rotation);
            audioSource.Play();
        }
        
	}
}