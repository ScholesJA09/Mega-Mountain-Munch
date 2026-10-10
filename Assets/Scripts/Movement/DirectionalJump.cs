using UnityEngine;

public class DirectionalJump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float minJumpForceY = 5f;
    public float maxJumpForceY = 15f;
    public float minJumpForceX = 3f;
    public float maxJumpForceX = 10f;
    public float maxChargeTime = 1.2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private int facingDirection = 1; // 1 = Right, -1 = Left
    private bool isGrounded;
    private bool wasGrounded;
    private float currentChargeTime = 0f;
    private bool isCharging = false;
    private bool hasJumped = false; // Tracks if player is airborne from a jump
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Check current ground state
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // 2. Play Idle animation on LANDING after a jump or fall
        if (isGrounded && (!wasGrounded || hasJumped) && !isCharging)
        {
            if (anim != null)
            {
                anim.Play("Idle");
            }
            hasJumped = false; // Reset jump flag on landing
        }

        // 3. Direction and sprite flip (ONLY when grounded)
        if (isGrounded)
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            if (inputX > 0) facingDirection = 1;
            else if (inputX < 0) facingDirection = -1;

            spriteRenderer.flipX = (facingDirection == -1);
        }

        // 4. Start charging on Space down -> Play Action1 (Charge/Jump animation)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isCharging = true;
            currentChargeTime = 0f;

            if (anim != null)
            {
                anim.Play("Action1");
            }
        }

        // 5. Cancel charge if player falls off a ledge
        if (isCharging && !isGrounded)
        {
            isCharging = false;
            currentChargeTime = 0f;
        }

        // 6. Accumulate charge while holding Space
        if (Input.GetKey(KeyCode.Space) && isCharging)
        {
            currentChargeTime += Time.deltaTime;
            currentChargeTime = Mathf.Min(currentChargeTime, maxChargeTime);
        }

        // 7. Launch player on Space release
        if (Input.GetKeyUp(KeyCode.Space) && isCharging && isGrounded)
        {
            PerformJump();
        }

        // 8. Cache current grounded state
        wasGrounded = isGrounded;
    }

    void PerformJump()
    {
        float chargePercent = currentChargeTime / maxChargeTime;

        float jumpY = Mathf.Lerp(minJumpForceY, maxJumpForceY, chargePercent);
        float jumpX = Mathf.Lerp(minJumpForceX, maxJumpForceX, chargePercent);

        rb.velocity = new Vector2(facingDirection * jumpX, jumpY);

        isCharging = false;
        hasJumped = true; // Mark player as jumped so landing triggers Idle
        currentChargeTime = 0f;
    }
}