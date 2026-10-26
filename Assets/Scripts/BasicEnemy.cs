using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class BasicEnemy : MonoBehaviour
{
    public float walkScalar = 15f;
    public float jumpScalar = 100f;
    [Range(0f, 1f)]
    public float deathRadius = 0.9f;
    public bool isMovingRight = false;
    private bool justStarted;
    private Rigidbody2D rigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        justStarted = true;
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (rigidbody.linearVelocityX == 0)
        {
            if (!justStarted)
            {
                isMovingRight = !isMovingRight;  
            } 
        }

        rigidbody.AddForce(Vector2.right * (isMovingRight ? walkScalar : -walkScalar));
        justStarted = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (collision.transform.position.y >= transform.position.y + (transform.localScale.y / 2 * deathRadius))
            {
                Die();
                collision.rigidbody.AddForce(Vector2.up * jumpScalar);
            }
            else
                Hazard.KillPlayer(collision.gameObject);
        }
    }

    public void Die()
    {
        transform.position = Vector2.zero;
        rigidbody.linearVelocity = Vector2.zero;
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);            
    }
}
