using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Variables
    [Header("Movement")]
    public float movementSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("KeyBinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
	}

	private void Update()
	{
        //Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        PlayerInput();
        SpeedControl();

        //Drag
        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
	}

	private void FixedUpdate()
	{
        MovePlayer();
	}

	private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //Jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        //Get movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //Grounded
        if(grounded)
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f, ForceMode.Force);

        //In air
        else if(!grounded)
			rb.AddForce(moveDirection.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force);
	}

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        //Limit velocity if needed
        if(flatVelocity.magnitude > movementSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * movementSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x , rb.linearVelocity.y , limitedVelocity.z);
        }
    }

    private void Jump()
    {
        //Reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up *jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
