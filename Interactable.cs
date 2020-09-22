using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool isInteractable = true;

    //Base class with method which is meant to be overwritten by specific type of interaction
    public virtual void Interact()
    {
        Debug.Log("Interacting with " + transform.name);
    }
}
