using UnityEngine;

public class LocationTrigger : MonoBehaviour
{
    public float Radius;
    public Mission Mission;

    private void Update() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Radius);
        foreach (Collider2D collider in colliders) {
            if (collider.CompareTag("Player") && !MissionManager.Instance.IsMissionInProgress) {
                WaypointManager.Instance.FinishDestination();
                MissionManager.Instance.StartMission(Mission);
                Destroy(gameObject);
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
