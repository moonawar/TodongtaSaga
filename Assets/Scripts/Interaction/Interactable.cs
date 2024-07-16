using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract void Interact();
    public abstract void OnInteractableEnter();
    public abstract void OnInteractableExit();
}
