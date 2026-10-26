using UnityEngine;

// sets the player's respawn point when touched
public class Checkpoint : MonoBehaviour
{
    // last checkpoint the player touched
    public static Checkpoint Active;

    public Color activeColor = Color.green;
    private Color normalColor;
    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        normalColor = sprite.color;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || Active == this) return;

        // reset color of the old checkpoint
        if (Active != null) Active.sprite.color = Active.normalColor;

        Active = this;
        sprite.color = activeColor;
    }
}
