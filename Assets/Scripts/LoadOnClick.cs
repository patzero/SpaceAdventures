using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadOnClick : MonoBehaviour {

	public GameObject loadingImage;

	public void LoadScene(int level)
	{

		loadingImage.SetActive(true);
		foreach (GameObject o in Object.FindObjectsOfType<GameObject>()) {
			Destroy(o);
		}
		SceneManager.LoadScene(level);
	}
}