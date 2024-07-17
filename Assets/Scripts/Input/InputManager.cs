using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch[] touches = Input.touches;

            foreach (var touch in touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    Collider2D[] colliders = Physics2D.OverlapPointAll(touchPosition, interactableLayer);

                    foreach (var c in colliders)
                    {
                        if (!c.TryGetComponent<Interactable>(out var interactable))
                        {
                            interactable = c.GetComponentInParent<Interactable>();
                        }
                        
                        if (interactable != null)
                        {
                            interactable.Interact();
                        }
                    }
                }
            }
        }
    }
}
