using UnityEngine;

public interface Interactable 
{
    abstract bool IsInteractable();
    abstract void Interact(KeyCode keyCode);
}
