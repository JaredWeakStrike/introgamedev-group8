using System.Collections;
using UnityEngine;

// boosts the player's speed and jump for a few seconds when picked up
public class PowerUp : MonoBehaviour
{
    public float speedMultiplier = 1.5f;
    public float jumpMultiplier = 1.3f;
    public float duration = 5f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        player p = other.GetComponent<player>();
        if (p != null) StartCoroutine(Boost(p));
    }

    IEnumerator Boost(player p)
    {
        // hide the pickup while the boost is active
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        p.moveScalar *= speedMultiplier;
        p.jumpScalar *= jumpMultiplier;

        yield return new WaitForSeconds(duration);

        // player may be gone if the level restarted
        if (p != null)
        {
            p.moveScalar /= speedMultiplier;
            p.jumpScalar /= jumpMultiplier;
        }

        Destroy(gameObject);
    }
}
