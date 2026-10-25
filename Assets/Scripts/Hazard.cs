using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// kills the player on touch
public class Hazard : MonoBehaviour
{
    // called when the player dies
    public static event Action PlayerDied;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) KillPlayer();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) KillPlayer();
    }

    public static void KillPlayer()
    {
        PlayerDied?.Invoke();

        // restart level
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
