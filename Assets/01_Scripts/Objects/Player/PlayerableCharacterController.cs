using System;
using UnityEngine;

/// <summary>
///
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerableCharacterController : MonoBehaviourExtension
{
    // ===== Config =====
    [Header("Move")]
    [SerializeField] float moveSpeed = 6f;

    [Header("Jump")]
    [SerializeField] float jumpForce = 14f;
    [SerializeField] float coyoteTime = 0.1f;     
    [SerializeField] float jumpBuffer = 0.1f;     

    [Header("Dash")]
    [SerializeField] float dashForce = 20f;
    [SerializeField] float dashDuration = 0.15f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundRadius = 0.15f;
    [SerializeField] LayerMask groundMask;

    // ===== State =====
    Rigidbody2D rb;
    Animator anim;

    bool isGrounded;
    float coyoteTimer;
    float jumpBufferTimer;
    bool isDashing;
    float dashTimer;

    // ===== Input =====
    public interface IPlayerInput
    {
        float MoveX { get; }
        bool JumpDown { get; }     // ���� ������
        bool DashDown { get; }
        bool AttackDown { get; }
        bool InteractDown { get; }
    }

    // 
    class DefaultKeyboardInput : IPlayerInput
    {
        public float MoveX => Input.GetAxisRaw("Horizontal");
        public bool JumpDown => Input.GetButtonDown("Jump");
        public bool DashDown => Input.GetKeyDown(KeyCode.LeftShift);
        public bool AttackDown => Input.GetKeyDown(KeyCode.J);
        public bool InteractDown => Input.GetKeyDown(KeyCode.K);
    }

    IPlayerInput input = new DefaultKeyboardInput();

    // ===== Events (�ܺ� �ý����� ����) =====
    public event Action OnJump;
    public event Action OnDashStart;
    public event Action OnDashEnd;
    public event Action OnAttack;
    public event Action OnInteract;

    // ===== Lifecycle =====
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    // Character Service Manager Register Character
    protected override void OnEnable()
    {
        base.OnEnable();
        PlayerableCharacterManager.Instance.OnRegisterCharacter(this);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        PlayerableCharacterManager.Instance.OnUnRegisterCharacter(this);
    }

    protected override void Start() 
    {
    }

    void Update()
    {
        UpdateGrounded();
        UpdateTimers();
        ReadActionInputs();
        UpdateFacing();

        // TODO: Animator Update
        //UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (isDashing) return; 
        MoveHorizontally(input.MoveX);
        TryJump();
    }

    // ===== Public API =====
    public void SetInput(IPlayerInput newInput) => input = newInput ?? input;

    public void TeleportTo(Vector2 pos)
    {
        rb.position = pos;
        rb.linearVelocity = Vector2.zero;
    }

    public void ApplyKnockback(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    // ===== Internals =====
    void UpdateGrounded()
    {
        if (groundCheck)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
        else
            isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.01f; // �ӽ� ��ü
    }

    void UpdateTimers()
    {
        coyoteTimer = isGrounded ? coyoteTime : Mathf.Max(0, coyoteTimer - Time.deltaTime);
        jumpBufferTimer = Mathf.Max(0, jumpBufferTimer - Time.deltaTime);

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                OnDashEnd?.Invoke();
            }
        }
    }

    void ReadActionInputs()
    {
        if (input.JumpDown) jumpBufferTimer = jumpBuffer;

        if (input.DashDown) TryDash();

        if (input.AttackDown)
            OnAttack?.Invoke(); 

        if (input.InteractDown)
            OnInteract?.Invoke(); 
    }

    void MoveHorizontally(float x)
    {
        var v = rb.linearVelocity;
        v.x = x * moveSpeed;
        rb.linearVelocity = v;
    }

    void UpdateFacing()
    {
        float x = input.MoveX;
        if (Mathf.Abs(x) > 0.01f)
            transform.localScale = new Vector3(Mathf.Sign(x), 1f, 1f);
    }

    void TryJump()
    {
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferTimer = 0f;
            OnJump?.Invoke();
        }
    }

    void TryDash()
    {
        if (isDashing) return;
        isDashing = true;
        dashTimer = dashDuration;

        var dir = new Vector2(Mathf.Sign(transform.localScale.x), 0f);
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * dashForce, ForceMode2D.Impulse);

        OnDashStart?.Invoke();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
#endif
}
