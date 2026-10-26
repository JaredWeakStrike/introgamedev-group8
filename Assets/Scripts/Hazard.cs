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
        if (other.CompareTag("Player")) KillPlayer(other.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) KillPlayer(collision.gameObject);
    }

    public static void KillPlayer(GameObject player)
    {
        PlayerDied?.Invoke();

        // respawn at last checkpoint
        if (Checkpoint.Active != null)
        {
            player.transform.position = Checkpoint.Active.transform.position;
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            return;
        }

        // no checkpoint, restart level
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
