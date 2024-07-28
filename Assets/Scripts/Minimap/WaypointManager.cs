using UnityEngine;
using UnityEngine.UI;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance { get; private set; }
    [SerializeField] private Image arrow;
    [SerializeField] private float inset = 0.9f;
    [SerializeField] private Transform player;
    [SerializeField] private Camera uiCamera; // Reference to the UI camera

    private Vector2 destination;
    private bool waypointOn;
    private float radarRadius;
    private float worldRadarRadius;
    private RectTransform radarRectTransform;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        radarRectTransform = GetComponent<RectTransform>();
        radarRadius = radarRectTransform.rect.width / 2 * inset;
        arrow.gameObject.SetActive(false);

        CalculateWorldRadarRadius();
    }

    private void CalculateWorldRadarRadius()
    {
        Vector3 radarCenter = radarRectTransform.position;
        Vector3 radarEdge = radarCenter + new Vector3(radarRadius, 0, 0);

        Vector3 worldCenter = uiCamera.ScreenToWorldPoint(radarCenter);
        Vector3 worldEdge = uiCamera.ScreenToWorldPoint(radarEdge);

        worldRadarRadius = Vector3.Distance(worldCenter, worldEdge);
        
    }

    public void SetDestination(Vector2 dest)
    {
        if (arrow == null) return;
        arrow.gameObject.SetActive(true);
        destination = dest;
        waypointOn = true;
    }

    public void FinishDestination()
    {
        if (arrow == null) return;
        arrow.gameObject.SetActive(false);
        waypointOn = false;
    }

    private void Update()
    {
        if (!waypointOn) return;
        UpdateArrowPosition();
        UpdateArrowRotation();
    }

    private void UpdateArrowPosition()
    {
        Vector2 directionToTarget = destination - (Vector2)player.position;
        float distanceToTarget = directionToTarget.magnitude;
        Vector2 waypointPosition;

        if (distanceToTarget <= worldRadarRadius)
        {
            // Target is within radar range
            waypointPosition = directionToTarget / worldRadarRadius * radarRadius;
        }
        else
        {
            // Target is outside radar range, clamp to edge of radar
            waypointPosition = directionToTarget.normalized * radarRadius;
        }

        arrow.rectTransform.anchoredPosition = waypointPosition * inset;
    }

    private void UpdateArrowRotation()
    {
        Vector2 directionToTarget = destination - (Vector2)player.position;
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90f;
        arrow.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, worldRadarRadius);
        }
    }
}