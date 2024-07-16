using System.Collections;
using UnityEngine;

public class Tikus : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private float invulnerableDuration;
    public bool IsInvulnerable {get; private set;} = false;

    [Header("Detection")]
    [SerializeField] private float frontDist = 0.5f;
    [SerializeField] private float backDist = 0.5f;
    [SerializeField] private LayerMask boundaryLayer;
    [SerializeField] private Transform raycastOrigin;

    [Header("Random Direction Change")]
    [SerializeField] private Range directionChangeDelayRange = new Range(1f, 5f);
    private float nextDirectionChangeTime;
    private Vector2 currentDirection = Vector2.left;
    private ObjectColorBlink colorBlink;

    private void Awake() {
        if (raycastOrigin == null) {
            Debug.LogWarning("No raycast origin assigned to TikusMovement script.");
            raycastOrigin = transform;
        }

        colorBlink = GetComponent<ObjectColorBlink>();
        invulnerableDuration = colorBlink.TotalBlinkDuration;
    }

    private void Start()
    {
        currentDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        SetNextDirectionChangeTime();
    }

    private void FixedUpdate() {
        if (GameStateManager.Instance.CurrentState != GameState.MinigamePlaying) return;

        Move();
        CheckBoundary();
        CheckRandomDirectionChange();

        if (IsTikusOutsideOfArena()) {
            transform.position = Vector3.zero;
        }
    }

    private IEnumerator BecomeInvulnerable()
    {
        IsInvulnerable = true;
        colorBlink.StartBlinking();
        yield return new WaitForSeconds(invulnerableDuration);
        IsInvulnerable = false;
    }


    private void Move() { 
        transform.position += moveSpeed * Time.fixedDeltaTime * (Vector3)currentDirection;
        transform.rotation = Quaternion.Euler(0, currentDirection.x >= 0 ? 180 : 0, 0);
    }

    private void CheckBoundary()
    {
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin.position, currentDirection, frontDist, boundaryLayer);
        
        if (hit.collider == null)
        {
            // If we don't hit anything, we're about to exit the boundary
            // Cast in the opposite direction to find the boundary normal
            RaycastHit2D boundaryCast = Physics2D.Raycast(raycastOrigin.position, -currentDirection, backDist, boundaryLayer);
            
            if (boundaryCast.collider != null)
            {
                // Calculate the reflection vector
                Vector2 reflection = Vector2.Reflect(currentDirection, boundaryCast.normal);
                
                // Set the new rotation instantly
                currentDirection = reflection;
            }
        }
    }

    private void CheckRandomDirectionChange()
    {
        if (Time.time >= nextDirectionChangeTime)
        {
            ChangeDirectionRandomly();
            SetNextDirectionChangeTime();
        }
    }

    private bool IsTikusOutsideOfArena() {
        return !Physics2D.Raycast(raycastOrigin.position, currentDirection, frontDist, boundaryLayer) && 
        !Physics2D.Raycast(raycastOrigin.position, -currentDirection, backDist, boundaryLayer);

    }

    private void ChangeDirectionRandomly()
    {
        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        currentDirection = randomDirection;
    }

    private void SetNextDirectionChangeTime()
    {
        nextDirectionChangeTime = Time.time + directionChangeDelayRange.RandomValue();
    }

    public void Die() {
        Destroy(gameObject);
    }

    public void TryCatched() {
        AmangUdaGameManager.Instance.DecrementTikusHealth();
        StartCoroutine(BecomeInvulnerable());
    }

    // Optional: Visualize the raycast in the Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(raycastOrigin.position, currentDirection * frontDist);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(raycastOrigin.position, -currentDirection * backDist);
    }
}