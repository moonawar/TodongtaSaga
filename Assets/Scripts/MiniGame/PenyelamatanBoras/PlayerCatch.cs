using UnityEngine;

namespace TodongtoaSaga.Minigames.PenyelamatanBoras
{
    public class PlayerCatch : MonoBehaviour
    {
        [SerializeField] private Transform catchAreaPivot;
        [SerializeField] private float catchRadius = 2f;
        [SerializeField] private float catchAngle = 30f;
        [SerializeField] private LayerMask catchableLayers; // Set this in the inspector to include Mouse and "Dont Catch" layers
        [SerializeField] private StatusSpawner catchStatusSpawner;
        public float CatchRadius { get => catchRadius; }
        public float CatchAngle { get => catchAngle; }

        private Collider2D[] hitColliders; // for debugging
        private bool isCollidingWithAmangUda = false;
        private float collisionTimer = 0f;
        [SerializeField] private float collisionDelay = 5f;
        private PlayerMovement movement;
        private BoxCollider2D boxCollider;
        private Animator animator;

        private void Awake() {
            movement = GetComponent<PlayerMovement>();
            if (catchStatusSpawner == null) {
                GetComponentInChildren<StatusSpawner>();
            }

            boxCollider = GetComponent<BoxCollider2D>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            // Rotate the catch area pivot to the mouse position
            Vector2 direction = movement.CurrentVelocity.normalized;

            if (movement.CurrentVelocity.magnitude != 0)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                catchAreaPivot.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }

            CheckForAmangUdaCollision();
        }

        public void Catch() {
            AudioManager.Instance.PlaySFX("Woosh");
            animator.SetTrigger("Catch");
        }

        public void OnCatchAnimEnd() {
            CheckForCatchables();
        }

        private void CheckForCatchables()
        {
            hitColliders = Physics2D.OverlapCircleAll(catchAreaPivot.position, catchRadius, catchableLayers);

            bool mouseHit = false;
            Collider2D somethingHit = null;

            foreach (Collider2D collider in hitColliders)
            {
                if (collider.CompareTag("Mouse"))
                {
                    if (IsInCatchArea(collider)) 
                    {
                        if (mouseHit) continue;
                        bool success = HandleMouseCatch(collider);
                        mouseHit = success;
                        break;
                    }
                }
                else if (collider.gameObject.layer == LayerMask.NameToLayer("DontCatch"))
                {
                    if (IsInCatchArea(collider))
                    {
                        if (somethingHit != null) continue;
                        HandleDontCatchObject(collider);
                        somethingHit = collider;
                        break;
                    }
                }
            }

            if (somethingHit != null)
            {
                if (IsAmangUda(somethingHit)) {
                    catchStatusSpawner.SpawnStatusText("Awas kena Amang Uda!");
                    return;
                }

                catchStatusSpawner.SpawnStatusText("Awas kena alat musik!");
                return;
            }

            if (mouseHit) { // Something was caught
                catchStatusSpawner.SpawnStatusText("Kena!");
            } else {
                catchStatusSpawner.SpawnStatusText("Meleset");
                AudioManager.Instance.PlaySFX("Meleset");
            }
        }

        private void CheckForAmangUdaCollision()
        {
            float scale = transform.lossyScale.x;
            Vector2 size = boxCollider.size * scale;

            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, size, 0);
            Collider2D amangUdaCollider = null;

            foreach (Collider2D collider in colliders)
            {
                if (IsAmangUda(collider))
                {
                    amangUdaCollider = collider;
                    break;
                }
            }

            if (amangUdaCollider != null)
            {
                if (!isCollidingWithAmangUda)
                {
                    isCollidingWithAmangUda = true;
                    collisionTimer = 0f;
                    GameManager.Instance.IncrementAngerMeter();
                    catchStatusSpawner.SpawnStatusText("Amang Uda ketabrak!");
                    amangUdaCollider.GetComponent<ObjectColorBlink>().StartBlinking();
                }
                else
                {
                    collisionTimer += Time.deltaTime;
                    if (collisionTimer >= collisionDelay)
                    {
                        isCollidingWithAmangUda = false;
                    }
                }
            }
            else
            {
                isCollidingWithAmangUda = false;
            }
        }

        private bool IsAmangUda(Collider2D collider)
        {
            AmangUda amangUda = collider.GetComponent<AmangUda>();
            return amangUda != null;
        }

        private bool IsInCatchArea(Collider2D collider)
        {
            if (collider is CapsuleCollider2D capsuleCollider)
            {
                return IsCapsuleInCatchArea(capsuleCollider);
            }
            else if (collider is BoxCollider2D boxCollider)
            {
                return IsBoxInCatchArea(boxCollider);
            }
            // Fallback to checking the collider's position if it's neither a CapsuleCollider2D nor a BoxCollider2D
            return IsPointInCatchArea(collider.transform.position);
        }

        private bool IsCapsuleInCatchArea(CapsuleCollider2D capsule)
        {
            Vector2[] capsulePoints = GetCapsulePoints(capsule);
            foreach (Vector2 point in capsulePoints)
            {
                if (IsPointInCatchArea(point))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsBoxInCatchArea(BoxCollider2D box)
        {
            Vector2[] boxPoints = GetBoxPoints(box);
            foreach (Vector2 point in boxPoints)
            {
                if (IsPointInCatchArea(point))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsPointInCatchArea(Vector2 point)
        {
            Vector2 directionToPoint = point - (Vector2)catchAreaPivot.position;
            float distanceToPoint = directionToPoint.magnitude;
            float angleToPoint = Vector2.Angle(catchAreaPivot.up, directionToPoint);

            return distanceToPoint <= catchRadius && angleToPoint <= catchAngle / 2;
        }

        private Vector2[] GetCapsulePoints(CapsuleCollider2D capsule)
        {
            float scale = capsule.transform.lossyScale.x;
            float radius = capsule.size.x / 2 * scale;
            float height = capsule.size.y * scale;


            Vector2 center = capsule.transform.TransformPoint(capsule.offset);
            Vector2 up = capsule.transform.up * height / 2;
            Vector2 v_rad = capsule.transform.up * radius;
            Vector2 right = capsule.transform.right * radius;

            Vector2[] points = new Vector2[7];
            points[0] = center + up + v_rad; // Top center
            points[1] = center - up - v_rad; // Bottom center
            points[2] = center; // Center
            points[3] = center + up - right; // Top left
            points[4] = center + up + right; // Top right
            points[5] = center - up - right; // Bottom left
            points[6] = center - up + right; // Bottom right

            return points;
        }

        private Vector2[] GetBoxPoints(BoxCollider2D box)
        {
            Vector2 center = box.transform.TransformPoint(box.offset);
            Vector2 size = box.size / 2;
            Vector2[] points = new Vector2[5];

            float scale = box.transform.lossyScale.x;
            size.x *= scale;
            size.y *= scale;

            points[0] = center; // Center
            points[1] = center + new Vector2(size.x, size.y); // Top-right
            points[2] = center + new Vector2(-size.x, size.y); // Top-left
            points[3] = center + new Vector2(size.x, -size.y); // Bottom-right
            points[4] = center + new Vector2(-size.x, -size.y); // Bottom-left
            return points;
        }

        private bool HandleMouseCatch(Collider2D collider)
        {
            Tikus tikus = collider.GetComponent<Tikus>();
            if (tikus != null && !tikus.IsInvulnerable)
            {
                tikus.TryCatched();
                return true;
            }

            return false;
        }

        private void HandleDontCatchObject(Collider2D collider)
        {
            GameManager.Instance.IncrementAngerMeter();
            collider.GetComponent<ObjectColorBlink>().StartBlinking();
        }

        // Visualize the catch area in the editor
        private void OnDrawGizmos()
        {
            if (catchAreaPivot == null) return;
            Gizmos.color = Color.red;
            Vector3 leftDirection = Quaternion.Euler(0, 0, catchAngle / 2) * catchAreaPivot.up;
            Vector3 rightDirection = Quaternion.Euler(0, 0, -catchAngle / 2) * catchAreaPivot.up;
            Gizmos.DrawLine(catchAreaPivot.position, catchAreaPivot.position + leftDirection * catchRadius);
            Gizmos.DrawLine(catchAreaPivot.position, catchAreaPivot.position + rightDirection * catchRadius);
            Gizmos.DrawWireSphere(catchAreaPivot.position, catchRadius);
        }
    }
}