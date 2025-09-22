using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Interactable : MonoBehaviour
{
    protected virtual void Awake()
    {
        // Ensure the collider is set up as a trigger
        var collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        throw new System.NotImplementedException();
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool CanInteract()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Interact()
    {
        throw new System.NotImplementedException();
    }
}
