using UnityEngine;
using System;
using UnityEngine.Events;

public class InteractableObjectManager : MonoBehaviour
{
    public static InteractableObjectManager Instance;
    [SerializeField] private GameObject prefab;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void CreateInteractableObject(Vector3 position, Action onInteraction)
    {
        InGameDebug.Instance.Log("Creating interactable object at " + position);

        GameObject interactableObject = Instantiate(prefab, position, Quaternion.identity);
        InteractableObject interactableObjectComponent = interactableObject.GetComponent<InteractableObject>();
        interactableObjectComponent.ClearInteractionListeners();
        interactableObjectComponent.AddInteractionListener(onInteraction);
        interactableObject.SetActive(true);
    }

    public void CreateInteractableObject(Vector3 position, UnityEvent onInteraction)
    {
        InGameDebug.Instance.Log("Creating interactable object at " + position);

        GameObject interactableObject = Instantiate(prefab, position, Quaternion.identity);
        InteractableObject interactableObjectComponent = interactableObject.GetComponent<InteractableObject>();
        interactableObjectComponent.ClearInteractionListeners();
        interactableObjectComponent.AddInteractionListener(() => onInteraction?.Invoke());
        interactableObject.SetActive(true);
    }
}