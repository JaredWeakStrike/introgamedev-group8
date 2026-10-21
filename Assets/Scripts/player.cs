using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    public float moveScalar = 30;
    public float jumpScalar = 300;
    public InputAction WASD;
    private Vector2 moveDirection;
    public InputAction jump;
    private Rigidbody2D rigidbody;
    private bool jumping = false;
    private bool jumped = false;
    private bool doubleJumped = false;
    private bool doubleJumping = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WASD.Enable();
        jump.Enable();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();

        if (!jumped) doubleJumped = false;
    }

    void FixedUpdate()
    {
        Move();

        //jumped = 
    }

    void ProcessInput()
    {
        moveDirection = WASD.ReadValue<Vector2>().normalized;
        if (!jumped)
            jumping = jump.WasPressedThisFrame() && rigidbody.linearVelocityY < 0.1;
        else if (!doubleJumped)
            doubleJumping = jump.WasPerformedThisFrame();
    }

    void Move()
    {
        rigidbody.AddForce(new Vector2(moveDirection.x * moveScalar, 0));
        if (jumping)
        {
            jumping = false;
            rigidbody.AddForce(jumpScalar * Vector2.up);
            jumped = true;
        }

        if (doubleJumping)
        {
            doubleJumping = false;
            rigidbody.AddForce(new Vector2(moveDirection.x, jumpScalar * 1.25f));
            doubleJumped = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.transform.tag == "Ground")
        {
            if (collision.transform.position.y < transform.position.y)
                jumped = false;
        }
    }
}
