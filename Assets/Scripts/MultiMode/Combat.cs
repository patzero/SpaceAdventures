using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Combat : NetworkBehaviour {

	public int health = 100;
	public string playerName = "Player";
	public Texture box;

	public GameObject playerExplosion;

	public void TakeDamage(int amount) {
		health = health - amount;

		if (health <= 0) {
			//MakeExplosion ();
			Instantiate(playerExplosion, transform.position, transform.rotation);
		}
	}
		
	void OnGUI () {

		Vector3 pos = Camera.main.WorldToScreenPoint (transform.position); 

		// Draw player name
		GUI.color = Color.black;
		GUI.Label(new Rect(pos.x - 30, Screen.height - pos.y + 40, 100, 30), playerName);

		GUI.color = Color.white;
		GUI.Label(new Rect(pos.x - 31, Screen.height - pos.y + 41, 100, 30), playerName);

		// Draw heath bar bg
		GUI.color = Color.grey;
		GUI.DrawTexture(new Rect(pos.x - 35, Screen.height - pos.y + 35, 50, 7), box);
			
		// Draw hp
		GUI.color = Color.green;
		GUI.DrawTexture(new Rect(pos.x - 36, Screen.height - pos.y + 36, health/2, 5), box);
	}
}
