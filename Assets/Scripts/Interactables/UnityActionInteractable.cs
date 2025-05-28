using UnityEngine;
using UnityEngine.Events;

public class UnityActionInteractable : AbstractInteractable
{
    [SerializeField]
    private UnityEvent onInteract;

    protected override void Interact()
    {
        if (onInteract != null)
        {
            onInteract.Invoke();
        }
    }
}
