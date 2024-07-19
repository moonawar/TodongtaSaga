using UnityEngine;

#if DEBUG
// [RequireComponent(typeof(LineRenderer))]
#endif

public class PlayerMovement : MonoBehaviour
{
    public Vector2 CurrentVelocity { get { return currentVelocity; } }

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 100f;

    [Header("Constraint")]
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private Collider2D stayInside;

    [Header("Joystick")]
    [SerializeField] private Joystick joystick;
    [SerializeField] private bool joystickOnly = false;

    private Vector2 movement;
    private Vector2 currentVelocity;
    private Collider2D playerCollider;
    private Animator animator;
    private Vector2 lastSlide;

    private float scaler = 1f;

// #if DEBUG
//     private LineRenderer line;
// #endif

    private void Awake()
    {
        animator = GetComponent<Animator>();

// #if DEBUG
//         line = GetComponent<LineRenderer>();
// #endif
    }

    private void Start()
    {
        playerCollider = GetComponent<Collider2D>();
    }

    private void OnEnable() { GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged; }
    private void OnDisable()
    {
        if (GameStateManager.Instance == null) return;
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state != GameState.Gameplay)
        {
            currentVelocity = Vector2.zero;
            animator.SetFloat("Speed", 0f);
        }

        joystick.SetHorizontal(0f);
        joystick.SetVertical(0f);
    }

    private Vector2 ReadInput()
    {
        Vector2 move = new(joystick.Horizontal, joystick.Vertical);
        if (joystickOnly)
        {
            return move;
        }
        else
        {
            Vector2 keyboardMove = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            return move + keyboardMove;
        }
    }

    private void Update()
    {
#if DEBUG
        Vector3 prevPosition = transform.position;
#endif

        if (GameStateManager.Instance.CurrentState != GameState.Gameplay) return;
        // Get input
        movement = Vector2.ClampMagnitude(ReadInput(), 1f);

        // Calculate velocity
        if (movement != Vector2.zero)
        {
            currentVelocity = Vector2.MoveTowards(currentVelocity, maxSpeed * scaler * movement, acceleration * scaler * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.deltaTime);
        }

        // Apply movement with collision check
        Vector3 appliedMove = currentVelocity * Time.deltaTime;
        Vector3 newPosition = transform.position + appliedMove;

        if (IsMoveAllowed(newPosition))
        {
            transform.position = newPosition;
        }
        else
        {
            // Attempt to slide along obstacles
            RaycastHit2D hit = Physics2D.BoxCast(
                newPosition + (Vector3)playerCollider.offset, 
                playerCollider.bounds.size * 1.1f, 0f, appliedMove.normalized, 
                appliedMove.magnitude, collisionLayer
            );

            if (hit.collider != null)
            {
                // Calculate the slide vector
                Vector2 reflect = Vector2.Reflect(appliedMove, hit.normal);
                Vector2 perpendicular = new(-hit.normal.y, hit.normal.x);
                Vector2 slide = Vector2.Dot(perpendicular, reflect) * perpendicular;

                // Check if sliding is allowed
                if (IsMoveAllowed((Vector2)transform.position + slide))
                {
                    lastSlide = slide;
                    transform.position = (Vector2)transform.position + slide;
                } else {
                    // Try again with last succesful slide
                    if (IsMoveAllowed((Vector2)transform.position + lastSlide))
                    {
                        transform.position = (Vector2)transform.position + lastSlide;
                    }
                }
            }
        }

// #if DEBUG
//         Vector3 moveDirection = (transform.position - prevPosition).normalized;

//         line.SetPosition(0, prevPosition);
//         line.SetPosition(1, prevPosition + moveDirection * 5f);
// #endif

        // Update animator
        float speed = movement.magnitude;
        animator.SetFloat("Speed", speed);

        if (speed > 0.01f)
        {
            // Only update direction if the player is moving
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
        }
    }

    private bool IsMoveAllowed(Vector2 newPosition)
    {
        return !WillCollide(newPosition) && IsInsideAllowedArea(newPosition);
    }

    private bool IsInsideAllowedArea(Vector2 newPosition)
    {
        if (stayInside == null) return true;

        // Calculate the corners of the player's collider at the new position
        Vector2 halfSize = playerCollider.bounds.size / 2;
        Vector2 topLeft = newPosition + new Vector2(-halfSize.x, halfSize.y) + playerCollider.offset;
        Vector2 topRight = newPosition + new Vector2(halfSize.x, halfSize.y) + playerCollider.offset;
        Vector2 bottomLeft = newPosition + new Vector2(-halfSize.x, -halfSize.y) + playerCollider.offset;
        Vector2 bottomRight = newPosition + new Vector2(halfSize.x, -halfSize.y) + playerCollider.offset;

        // Check if all corners are inside the boundary
        return stayInside.OverlapPoint(topLeft) &&
               stayInside.OverlapPoint(topRight) &&
               stayInside.OverlapPoint(bottomLeft) &&
               stayInside.OverlapPoint(bottomRight);
    }

    private bool WillCollide(Vector2 newPosition)
    {
        newPosition += playerCollider.offset;
        return Physics2D.OverlapBox(newPosition, playerCollider.bounds.size, 0f, collisionLayer);
    }
}