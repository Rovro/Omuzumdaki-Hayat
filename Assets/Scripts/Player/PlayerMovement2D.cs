using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Flip")]
    public Transform visual; // Sprite holder; yoksa karakterin kendisini atayın
    private Vector3 initialScale;

    [Header("Tow Setup (Optional)")]
    public Rigidbody2D stretcherRb;         // Sedye RB
    public DistanceJoint2D stretcherJoint;  // Sedye üzerindeki joint (reference için)
    public LineRenderer rope;               // Opsiyonel ip görseli

    [Header("Animation")]
    public Animator animator;               // Karakter Animator
    public string speedParam = "Speed";     // Animator float param
    public string isWalkingParam = "IsWalking"; // Animator bool param
    [Tooltip("Yürüyor saymak için minimum hız")]
    public float walkThreshold = 0.05f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    // Gerçek hız ölçümü için
    private Vector2 _lastPos;
    private float _measuredSpeed;

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        walkThreshold = Mathf.Max(0f, walkThreshold);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (visual == null) visual = transform;
        initialScale = visual.localScale;

        if (rope != null)
        {
            rope.positionCount = 2;
            rope.useWorldSpace = true;
        }

        _lastPos = rb.position;
    }

    // New Input System callback (Player Input → Behavior: Send Messages)
    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        // Physics-friendly hareket
        Vector2 targetPos = rb.position + moveInput * (moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(targetPos);

        // Gerçek hız ölçümü (MovePosition sonrası)
        Vector2 delta = rb.position - _lastPos;
        _measuredSpeed = delta.magnitude / Time.fixedDeltaTime;
        _lastPos = rb.position;
    }

    private void Update()
    {
        // Flip (yalnızca X)
        if (moveInput.x > 0.01f)
            visual.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        else if (moveInput.x < -0.01f)
            visual.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);

        // Animator (A + B birlikte)
        if (animator != null)
        {
            bool hasInput = Mathf.Abs(moveInput.x) > 0.01f || Mathf.Abs(moveInput.y) > 0.01f;
            // Yürüme bayrağı: inputa dayalı (anlık tepki)
            animator.SetBool(isWalkingParam, hasInput);
            // Hız: gerçek ölçüm (geçişleri doğal yapar)
            animator.SetFloat(speedParam, _measuredSpeed);

            // Dilersen sadece hızla kontrol edebilirsin:
            // animator.SetBool(isWalkingParam, _measuredSpeed > walkThreshold);
        }

        // Rope görseli (opsiyonel)
        if (rope != null && stretcherRb != null && stretcherJoint != null)
        {
            Vector2 soldierAnchorWorld = (Vector2)transform.TransformPoint(stretcherJoint.connectedAnchor);
            Vector2 stretcherAnchorWorld = (Vector2)stretcherRb.transform.TransformPoint(stretcherJoint.anchor);
            rope.SetPosition(0, soldierAnchorWorld);
            rope.SetPosition(1, stretcherAnchorWorld);
        }
    }
}
