using UnityEngine;

public class DestroyByBoundary : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        // Destroy everything that leaves the game boundary
        if (other.tag != "Player")
        {
            GameObject gameObject = GameObject.FindWithTag("GameController");
            GameController gameController = gameObject.GetComponent<GameController>();
            if (!gameController.IsGameOver())
            {
                if (other.tag == "Asteroid")
                    gameController.AddScore(-10);
                if (other.tag == "Enemy")
                    gameController.AddScore(-50);
            }

            Destroy(other.gameObject);
        }
    }
}