using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MultiModeDestroyByBoundary : NetworkBehaviour {

	void OnTriggerExit(Collider other) {
		// Destroy everything that leaves the game boundary
		if (other.tag != "Player") {
			Destroy(other.gameObject, 1.0f);
		}
	}
}
