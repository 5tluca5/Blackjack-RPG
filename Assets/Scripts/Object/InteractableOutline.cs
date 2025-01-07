using System;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(Interactable))]
public class InteractableOutline : MonoBehaviour
{
    [SerializeField] Renderer[] renderers;
    [SerializeField] private RenderingLayerMask interactableOutlineLayer;
    [SerializeField] private RenderingLayerMask OnHoverOutlineLayer;
    [SerializeField] bool hoverWhenInteractable = true;

    private uint originalLayer;
    private bool isOutlineActive;
    private Interactable interactable;

    ReactiveProperty<bool> isInteractable = new(false);
    ReactiveProperty<bool> isOnHover = new(false);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactable = GetComponent<Interactable>();

        if(renderers == null || renderers.Length == 0)
        {
            renderers = TryGetComponent<Renderer>(out var meshRenderer)
                ? new[] { meshRenderer }
                : GetComponentsInChildren<Renderer>();
        }

        originalLayer = renderers[0].renderingLayerMask;
        GameController.Instance.OnGamePhaseChanged().Subscribe(phase =>
        {
            isInteractable.Value = interactable.IsInteractable();
        }).AddTo(this);

        isInteractable.Subscribe(x =>
        {
            SetOutline();
        }).AddTo(this);

        isOnHover.Subscribe(x =>
        {
            SetOutline();
        }).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        //isOnHover.Value = false;
    }

    public void SetOnHover(bool set)
    {
        isOnHover.Value = set;
    }

    public void SetInteractable(bool set)
    {
        isInteractable.Value = set;
    }

    private void SetOutline()
    {
        foreach (var rend in renderers)
        {
            if((!hoverWhenInteractable && isOnHover.Value) || (hoverWhenInteractable && isOnHover.Value && isInteractable.Value))
            {
                rend.renderingLayerMask = originalLayer | OnHoverOutlineLayer;
            }
            else if (isInteractable.Value)
            {
                rend.renderingLayerMask = originalLayer | interactableOutlineLayer;
            }
            else
            {
                rend.renderingLayerMask = originalLayer;
            }
        }
    }
}
