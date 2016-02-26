using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SpaceMenager : NetworkManager {

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) 
	{
		// Spawn the player at the right position in line
		Vector3 spawnPos = new Vector3(1.5f, 0f, 0f)  * conn.connectionId;
		GameObject player = (GameObject)Instantiate (base.playerPrefab, spawnPos, Quaternion.identity);
		NetworkServer.AddPlayerForConnection (conn, player, playerControllerId);
	}

	public void StartGame()
	{
		NetworkManager.singleton.StartHost ();
	}

	public void JoinGame()
	{
		NetworkManager.singleton.StartClient ();
	}
		
}
