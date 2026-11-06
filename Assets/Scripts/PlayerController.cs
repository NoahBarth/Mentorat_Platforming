using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField] private Transform modelTransform;
    private Vector2 currentMovementVector;
    private float MovementSpeed = 10f;
    private float RotationSpeed = 5f;
    private float JumpBufferTime = 0.08f;
    private float CoyoteTime = 0.10f;
    private float JumpHoldTime = 0.07f;
    private float LowRiseGravity = 22f;
    private float HighRiseGravity = 34f;
    private float FallGravity = 20f;
    private float TerminalFallSpeed = 30f;
    private float rotationY;
    // private float rotationX;
    private float verticalVelocity;
    private float jumpBufferTimer;
    private float bufferedJumpForce;
    private float coyoteTimer;
    private float jumpHoldTimer;
    private Animator animator;
    private bool wasGrounded;
    private bool isGrounded;
    private Renderer[] childRenderers;
    private IMovingPlatform currentPlatform;
    private float pushStrengthMultiplier = 1.5f; // Adjust this value to control how much the player is pushed by the platform


    [Header("Dust Effects")]
    public ParticleSystem jumpDust;
    public ParticleSystem landDust;
    public static event Action OnPlayerJump;

    [Header("Platform Detection")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundCheckMask = ~0; // adjust to your ground/platform layer


    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        childRenderers = GetComponentsInChildren<Renderer>(true);

        if (modelTransform == null && transform.childCount > 0)
        {
            modelTransform = transform.GetChild(0);
        }
    }


    void Update()
    {
        isGrounded = characterController.isGrounded;

        animator.SetBool("IsGrounded", isGrounded);

        if (!wasGrounded && isGrounded)
        {
            TriggerLandDust();
            ApplyBufferedJumpColor(Color.white);
        }

        wasGrounded = isGrounded;
        animator.SetBool("IsGrounded", isGrounded);
    }


    void LateUpdate()
    {
        FaceModelTowardCamera();
    }


    public void Move(Vector2 movementVector)
    {
        DetectCurrentPlatform();
        currentMovementVector = movementVector;

        if (characterController.isGrounded)
        {
            coyoteTimer = CoyoteTime;
        }
        else if (coyoteTimer > 0f)
        {
            coyoteTimer -= Time.deltaTime;

            if (coyoteTimer < 0f)
            {
                coyoteTimer = 0f;
            }
        }

        if (jumpBufferTimer > 0f && characterController.isGrounded)
        {
            verticalVelocity = bufferedJumpForce;
            jumpBufferTimer = 0f;
            jumpHoldTimer = JumpHoldTime;
        }

        if (jumpBufferTimer > 0f)
        {
            jumpBufferTimer -= Time.deltaTime;

            if (jumpBufferTimer <= 0f)
            {
                jumpBufferTimer = 0f;
            }
        }

        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0f;

        Vector3 move = camForward.normalized * movementVector.y + camRight.normalized * movementVector.x;
        if (move.sqrMagnitude > 1f)
        {
            move = move.normalized;
        }

        move = move * MovementSpeed * Time.deltaTime;

        characterController.Move(move);

        animator.SetFloat("Speed", Mathf.Clamp01(movementVector.magnitude));

        UpdateVerticalVelocity();

        Vector3 platformDelta = Vector3.zero;

        if (currentPlatform != null)
        {
            Vector3 toPlayer = transform.position - currentPlatform.Pivot;
            Vector3 rotatedOffset = currentPlatform.DeltaRotation * toPlayer;
            Vector3 rotationInducedDelta = rotatedOffset - toPlayer;

            platformDelta = currentPlatform.DeltaPosition + rotationInducedDelta*pushStrengthMultiplier;

            // Spin the player with the platform (safe outside Move(), no collision involved)
            transform.rotation = currentPlatform.DeltaRotation * transform.rotation;
        }

        characterController.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime + platformDelta);
    }


    public void Rotate(Vector2 rotationVector)
    {
        rotationY += rotationVector.x * RotationSpeed * Time.deltaTime;
        // rotationX -= rotationVector.y * RotationSpeed * Time.deltaTime;

        transform.localRotation = Quaternion.Euler(0, rotationY, 0);
    }


    private void FaceModelTowardCamera()
    {
        if (modelTransform == null || Camera.main == null || currentMovementVector.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Vector3 lookDirection = Camera.main.transform.forward;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.0001f)
        {
            modelTransform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }


    public void Jump(float jumpForce)
    {
        animator.SetTrigger("Jump");
        OnPlayerJump?.Invoke();
        if (characterController.isGrounded || coyoteTimer > 0f)
        {
            verticalVelocity = jumpForce;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            jumpHoldTimer = JumpHoldTime;
        }
        else
        {
            if (jumpBufferTimer > 0f)
            {
                return;
            }
            bufferedJumpForce = jumpForce;
            jumpBufferTimer = JumpBufferTime;
        }
    }


    public void LaunchUpwards(float launchForce)
    {
        animator.SetTrigger("Jump");
        verticalVelocity = launchForce;
        jumpBufferTimer = 0f;
        coyoteTimer = 0f;
        jumpHoldTimer = JumpHoldTime;
    }


    public void CutJump(float jumpCutMultiplier)
    {
        if (verticalVelocity > 0f)
        {
            verticalVelocity *= Mathf.Clamp01(jumpCutMultiplier);
            jumpHoldTimer = 0f;
        }
    }


    private void UpdateVerticalVelocity()
    {
        if (characterController.isGrounded && verticalVelocity <= 0f)
        {
            verticalVelocity = -2f;
            jumpHoldTimer = 0f;
            return;
        }

        float gravity = GetActiveGravity();
        verticalVelocity -= gravity * Time.deltaTime;

        if (verticalVelocity < -TerminalFallSpeed)
        {
            verticalVelocity = -TerminalFallSpeed;
        }

        if (jumpHoldTimer > 0f)
        {
            jumpHoldTimer -= Time.deltaTime;

            if (jumpHoldTimer < 0f)
            {
                jumpHoldTimer = 0f;
            }
        }
    }


    private float GetActiveGravity()
    {
        if (verticalVelocity > 0f)
        {
            return jumpHoldTimer > 0f ? LowRiseGravity : HighRiseGravity;
        }

        return FallGravity;
    }

    void TriggerLandDust()
    {
        if (landDust != null)
        {
            landDust.Play();
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("FallingPlatform") && isGrounded)
        {
            hit.gameObject.GetComponent<DropPlatform>()?.Activate();
        }
    }

    private void ApplyBufferedJumpColor(Color color)
    {
        if (childRenderers == null)
        {
            return;
        }

        for (int i = 0; i < childRenderers.Length; i++)
        {
            Renderer childRenderer = childRenderers[i];

            if (childRenderer != null && childRenderer.transform != transform)
            {
                childRenderer.material.color = color;
            }
        }
    }

    private void DetectCurrentPlatform()
    {
        currentPlatform = null;

        // if (!characterController.isGrounded)
        // {
        //     return;
        // }

        Vector3 origin = transform.position + characterController.center;
        float castDistance = (characterController.height / 2f) + groundCheckDistance;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, castDistance, groundCheckMask, QueryTriggerInteraction.Ignore))
        {
            currentPlatform = hit.collider.GetComponent<IMovingPlatform>();
        }
    }
}