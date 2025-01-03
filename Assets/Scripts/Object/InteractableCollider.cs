using UnityEngine;

public class InteractableCollider : MonoBehaviour, Interactable
{
    [SerializeField] Interactable mainInteractable;

    public void Interact(KeyCode keyCode)
    {
        mainInteractable.Interact(keyCode);
    }

    public bool IsInteractable()
    {
        return mainInteractable.IsInteractable();
    }

    private void Awake()
    {
        if (mainInteractable == null)
        {
            mainInteractable = transform.parent.GetComponent<Interactable>();
        }
    }
}
