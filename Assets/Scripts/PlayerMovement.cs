using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed = 10f;
    public float sprintSpeedMultiplier = 1.5f; // Sprint speed multiplier
    public float crouchSpeedMultiplier = 0.5f; // Crouch speed multiplier
    private float currentSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool readyToJump = true;

    [Header("KeyBinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    private bool grounded;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rb;

    private CapsuleCollider playerCollider;

    private bool isCrouching = false; // Tracks crouch state
    private bool isSprinting = false; // Tracks sprint state

    private Camera playerCamera;
    private float originalCamHeight;
    public float crouchCamHeight = 0.7f; // Camera height when crouching

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        playerCollider = GetComponent<CapsuleCollider>();
        currentSpeed = movementSpeed; // Start with base movement speed

        playerCamera = Camera.main;
        originalCamHeight = playerCamera.transform.localPosition.y; // Store original camera height
    }

    private void Update()
    {
        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        PlayerInput();
        SpeedControl();

        // Drag
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

        // Jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Crouch
        if (Input.GetKeyDown(crouchKey))
        {
            StartCrouch();
        }
        if (Input.GetKeyUp(crouchKey))
        {
            StopCrouch();
        }

        // Sprint
        isSprinting = Input.GetKey(sprintKey) && grounded && !isCrouching;

        // Adjust speed for sprinting or crouching
        if (isCrouching)
        {
            currentSpeed = movementSpeed * crouchSpeedMultiplier;
        }
        else if (isSprinting)
        {
            currentSpeed = movementSpeed * sprintSpeedMultiplier;
        }
        else
        {
            currentSpeed = movementSpeed;
        }
    }

    private void MovePlayer()
    {
        // Get movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Grounded movement
        if (grounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);

        // In-air movement
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Limit velocity if needed
        if (flatVelocity.magnitude > currentSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * currentSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void Jump()
    {
        // Reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void StartCrouch()
    {
        isCrouching = true;

        // Adjust collider height for crouch
        playerCollider.height = playerHeight * 0.5f;
        playerCollider.center = new Vector3(0, playerHeight * 0.25f, 0);

        // Adjust camera height for crouch
        playerCamera.transform.localPosition = new Vector3(0, crouchCamHeight, 0);
    }

    private void StopCrouch()
    {
        isCrouching = false;

        // Reset collider height and center
        playerCollider.height = playerHeight;
        playerCollider.center = new Vector3(0, playerHeight * 0.5f, 0);

        // Reset camera height
        playerCamera.transform.localPosition = new Vector3(0, originalCamHeight, 0);
    }
}
