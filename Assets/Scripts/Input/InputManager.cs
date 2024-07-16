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
                    RaycastHit2D[] hit = Physics2D.RaycastAll(touchPosition, Vector2.zero, Mathf.Infinity, interactableLayer);

                    foreach (var h in hit)
                    {
                        Interactable interactable = h.collider.GetComponent<Interactable>();
                        if (interactable == null)
                        {
                            interactable = h.collider.GetComponentInParent<Interactable>();
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
