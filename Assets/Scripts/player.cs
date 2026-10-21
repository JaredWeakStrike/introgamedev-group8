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
    public int dashDownWindow = 10;
    private int dashDownTimer = 0;
    private bool dashedDown = false;
    private bool canDashDown = true;
    private bool groundPound = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WASD.Enable();
        jump.Enable();
        rigidbody = GetComponent<Rigidbody2D>();
        dashDownTimer = 0;
        dashDownWindow *= 60;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    void FixedUpdate()
    {
        if (dashDownTimer > 0 && !dashedDown) {
            --dashDownTimer;
            dashedDown = dashDownTimer > 0;
        }
        Move();
    }

    // processing inputs every frame
    void ProcessInput()
    {
        // horizontal movement
        moveDirection = WASD.ReadValue<Vector2>().normalized;

        // jump action direction
        if (!jumped)
            jumping = jump.WasPressedThisFrame() && rigidbody.linearVelocityY < 0.1;
        else if (!doubleJumped)
            doubleJumping = jump.WasPerformedThisFrame();

        // dashing down (ground pound) detection
        if (canDashDown)
            dashedDown = moveDirection.y < 0 && canDashDown;

        if (moveDirection.y < 0 && !dashedDown)
        {
            dashDownTimer = dashDownWindow;
            canDashDown = true;
        }
    }

    // handles inputs from ProcessInput to movement
    // note that ProcessInput is in Update(), and that Move() is in FixedUpdate()
    void Move()
    {
        // processing horizontal movement
        rigidbody.AddForce(new Vector2(moveDirection.x * moveScalar, 0));

        // processing jump action
        if (jumping)
        {
            jumping = false;
            rigidbody.AddForce(jumpScalar * Vector2.up);
            jumped = true;
        }

        // processing jump action while not grounded (double jump)
        if (doubleJumping)
        {
            doubleJumping = false;

            // if double jumping in opposite direction, immediately stop movement before adding force in new direction
            if (Mathf.Sign(moveDirection.x) != Mathf.Sign(rigidbody.linearVelocityX)) rigidbody.linearVelocityX = 0;

            // if falling, make sure vertical velocity is 0 so force upwards is full
            if (rigidbody.linearVelocityY < 0) rigidbody.linearVelocityY = 0;

            // adding jump force
            rigidbody.AddForce(new Vector2(moveDirection.x, jumpScalar * 1.25f));
            doubleJumped = true;
        }

        // grounded friction affects how fast you come to a stop
        if (!jumped && moveDirection.x == 0) rigidbody.linearVelocityX *= 0.65f;
        // grounded friction affects how fast you can change direction
        //      being in the air (jumped == true) means no ground friction, so more acceleration & less resistance
        if (!jumped && moveDirection.x != 0 && Mathf.Sign(moveDirection.x) != Mathf.Sign(rigidbody.linearVelocityX))
            rigidbody.linearVelocityX *= 0.5f;

        if (dashedDown)
        {
            if (rigidbody.linearVelocityY > 0) rigidbody.linearVelocityY = 0;
            rigidbody.AddForce(Vector2.down * jumpScalar);
            dashDownTimer = 0;
            groundPound = true;
        }
    }

    // collisions
    // only implemented for ground collision
    void OnCollisionEnter2D(Collision2D collision)
    {
        // ground collision means you are grounded (jumped == false)
        if (collision.transform.tag == "Ground")
        {
            if (collision.transform.position.y - collision.transform.lossyScale.y < transform.position.y) // allow for wall jumping
            {
                jumped = false;
                dashDownTimer = 0;
                canDashDown = false;
                dashedDown = false;
                doubleJumped = false;
                groundPound = false;
}
        }

        // ground that you can't wall jumping
        if (collision.transform.tag == "SlipGround")
        {
            if (collision.transform.position.y + collision.transform.lossyScale.y <= transform.position.y)
            {
                jumped = false;
                dashDownTimer = 0;
                canDashDown = false;
                dashedDown = false;
                doubleJumped = false;
                groundPound = false;
            }
        }
    }
}
