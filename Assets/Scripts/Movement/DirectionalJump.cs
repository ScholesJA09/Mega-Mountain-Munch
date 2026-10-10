using UnityEngine;

public class DirectionalJump : MonoBehaviour
{
    [Header("Jump Settings")]
    public float minJumpForceY = 5f;
    public float maxJumpForceY = 15f;
    public float minJumpForceX = 3f;
    public float maxJumpForceX = 10f;
    public float maxChargeTime = 1.2f;

    [Header("Audio")]
    public AudioClip jumpSound;
    [Range(0f, 1f)] public float jumpVolume = 1f; // Adjust volume slider in Inspector

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private int facingDirection = 1;
    private bool isGrounded;
    private bool wasGrounded;
    private float currentChargeTime = 0f;
    private bool isCharging = false;
    private bool hasJumped = false;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // Automatically add an AudioSource if you forgot to add one
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (isGrounded && (!wasGrounded || hasJumped) && !isCharging)
        {
            if (anim != null) anim.Play("Idle");
            hasJumped = false;
        }

        if (isGrounded)
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            if (inputX > 0) facingDirection = 1;
            else if (inputX < 0) facingDirection = -1;

            spriteRenderer.flipX = (facingDirection == -1);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isCharging = true;
            currentChargeTime = 0f;
            if (anim != null) anim.Play("Action1");
        }

        if (isCharging && !isGrounded)
        {
            isCharging = false;
            currentChargeTime = 0f;
        }

        if (Input.GetKey(KeyCode.Space) && isCharging)
        {
            currentChargeTime += Time.deltaTime;
            currentChargeTime = Mathf.Min(currentChargeTime, maxChargeTime);
        }

        if (Input.GetKeyUp(KeyCode.Space) && isCharging && isGrounded)
        {
            PerformJump();
        }

        wasGrounded = isGrounded;
    }

    void PerformJump()
    {
        float chargePercent = currentChargeTime / maxChargeTime;

        float jumpY = Mathf.Lerp(minJumpForceY, maxJumpForceY, chargePercent);
        float jumpX = Mathf.Lerp(minJumpForceX, maxJumpForceX, chargePercent);

        rb.velocity = new Vector2(facingDirection * jumpX, jumpY);

        if (jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound, jumpVolume);
        }

        isCharging = false;
        hasJumped = true;
        currentChargeTime = 0f;
    }
}