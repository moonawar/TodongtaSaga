using UnityEngine;

public class RadiusDrawer : MonoBehaviour
{
    [SerializeField] private float radius;

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
