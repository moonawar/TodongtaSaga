using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableMap : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform mapRectTransform;
    [SerializeField] private float dragSpeed = 1f;
    [SerializeField] private float minZoom = 0.5f;
    [SerializeField] private float maxZoom = 2f;
    [SerializeField] private float zoomSpeed = 0.01f;

    private Vector2 lastDragPosition;
    private Vector2 dragDelta;
    private bool isDragging = false;
    private float currentZoom = 1f;

    private void Start()
    {
        if (mapRectTransform == null)
        {
            mapRectTransform = GetComponent<RectTransform>();
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            Vector3 newPosition = mapRectTransform.position + (Vector3)dragDelta * dragSpeed;
            mapRectTransform.position = newPosition;
        }

        // Handle pinch to zoom
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Zoom(difference * zoomSpeed);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        lastDragPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            dragDelta = eventData.position - lastDragPosition;
            lastDragPosition = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        dragDelta = Vector2.zero;
    }

    private void Zoom(float increment)
    {
        currentZoom = Mathf.Clamp(currentZoom + increment, minZoom, maxZoom);
        mapRectTransform.localScale = Vector3.one * currentZoom;
    }
}