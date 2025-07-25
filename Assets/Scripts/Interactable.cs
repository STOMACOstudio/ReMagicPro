using UnityEngine;

public class Interactable : MonoBehaviour
{
    // Base interaction method. Derived classes can override this to provide
    // specific behaviour when the player interacts with the object.
    public virtual void Interact()
    {
        Debug.Log($"Interacted with {gameObject.name}");
    }
}
