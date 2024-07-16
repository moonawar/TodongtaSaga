using UnityEngine;

public class PlayerInteract : MonoBehaviour {
    [SerializeField] private float interactRadius = 1f;
    private Interactable active;

    private void DetectInteractable() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRadius);

        Interactable closest = null;
        foreach (var collider in colliders) {
            if (collider.TryGetComponent<Interactable>(out var interactable)) {
                if (closest == null) {
                    closest = interactable;
                } else {
                    if (CompareDistance(closest, interactable)) {
                        closest = interactable;
                    }
                }
            }
        }

        if (active == closest) return;
        if (active != null) { active.OnInteractableExit(); }
        active = closest;
        if (active != null) { active.OnInteractableEnter(); }
    }

    /**
        * @return True if the incoming object is closer to the player. False otherwise.
    */
    private bool CompareDistance(Interactable current, Interactable incoming) {
        return Vector2.Distance(transform.position, incoming.transform.position) <
            Vector2.Distance(transform.position, current.transform.position);
    }

    private void Update() {
        if (GameStateManager.Instance.CurrentState != GameState.Explore) return;
        DetectInteractable();

        if (Input.GetKeyDown(KeyCode.E) && active != null) {
            active.Interact();
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}