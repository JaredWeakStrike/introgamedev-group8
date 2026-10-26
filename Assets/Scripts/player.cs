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
    private bool grounded = false;
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
        if (rigidbody.attachedColliderCount <= 0)
        {
            grounded = false;
        }

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
            jumping = (jump.WasPressedThisFrame() || moveDirection.y > 0) && rigidbody.linearVelocityY < 0.1;
        else if (!doubleJumped)
            doubleJumping = jump.WasPressedThisFrame();

        // dashing down (ground pound) detection
        dashedDown = canDashDown && moveDirection.y < 0 && !grounded;
        if (grounded) groundPound = false;

        if (moveDirection.y < 0 && !dashedDown)
        {
            dashDownTimer = dashDownWindow;
            canDashDown = true;
        } else if (dashedDown)
        {
            canDashDown = false;
        }
    }

    // handles inputs from ProcessInput to movement
    // note that ProcessInput is in Update(), and that Move() is in FixedUpdate()
    void Move()
    {
        // processing horizontal movement
        if (grounded) 
            rigidbody.AddForce(new Vector2(moveDirection.x * moveScalar, 0));
        else
            rigidbody.AddForce(new Vector2(moveDirection.x * moveScalar * 0.5f, 0));


        // processing jump action
        if (jumping)
        {
            jumping = false;
            rigidbody.AddForce(jumpScalar * Vector2.up);
            jumped = true;
            grounded = false;
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
        if (grounded && moveDirection.x == 0) rigidbody.linearVelocityX *= 0.65f;
        // grounded friction affects how fast you can change direction
        //      being in the air (jumped == true) means no ground friction, so more acceleration & less resistance
        if (grounded && moveDirection.x != 0 && Mathf.Sign(moveDirection.x) != Mathf.Sign(rigidbody.linearVelocityX))
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
    // Uses contact normals instead of the ground object's position/scale, so it works on a
    // Tilemap (the whole level is one collider) as well as on separate blocks:
    //   normal.y > 0.5    -> standing on top of the surface (floor)
    //   |normal.x| > 0.5  -> touching the side of the surface (wall)
    //   normal.y < -0.5   -> hitting it from below (ceiling), resets nothing
    void OnCollisionEnter2D(Collision2D collision)
    {
        grounded = false;
        CheckGround(collision, true);
    }

    // landing while still touching the same collider (e.g. sliding down a tilemap wall onto its floor)
    // doesn't fire OnCollisionEnter2D again, so floor contacts are also checked while touching
    void OnCollisionStay2D(Collision2D collision)
    {
        if (!grounded && rigidbody.linearVelocityY <= 0.01f)
            CheckGround(collision, false);
    }

    void CheckGround(Collision2D collision, bool allowWall)
    {
        bool isGround = collision.collider.CompareTag("Ground");
        bool isSlipGround = collision.collider.CompareTag("SlipGround");
        if (!isGround && !isSlipGround) return;

        bool onFloor = false;
        bool onWall = false;
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            if (normal.y > 0.5f) onFloor = true;
            else if (Mathf.Abs(normal.x) > 0.5f) onWall = true;
        }

        // ground collision means you are grounded (jumped == false)
        // "Ground" also resets the jump from its sides (allows wall jumping), "SlipGround" doesn't
        if (onFloor || (isGround && onWall && allowWall))
        {
            jumped = false;
            dashDownTimer = 0;
            canDashDown = false;
            dashedDown = false;
            groundPound = false;
        }

        if (onFloor)
        {
            grounded = true;
            doubleJumped = false;
            print("Grounded");
        }
    }

    public bool isMoving()
    {
        return rigidbody.linearVelocity.magnitude != 0;
    }

    public Vector2 getMoveDirection ()
    {
        return moveDirection;
    }

    public bool isGroundPounding()
    {
        return groundPound && rigidbody.linearVelocityY < -0.1;
    }

    public bool isGrounded()
    {
        return grounded;
    }
}
