using System;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(Interactable))]
public class InteractableOutline : MonoBehaviour
{
    [SerializeField] private RenderingLayerMask interactableOutlineLayer;
    [SerializeField] private RenderingLayerMask OnHoverOutlineLayer;

    private Renderer[] renderers;
    private uint originalLayer;
    private bool isOutlineActive;
    private Interactable interactable;

    ReactiveProperty<bool> isInteractable = new(false);
    ReactiveProperty<bool> isOnHover = new(false);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactable = GetComponent<Interactable>();

        renderers = TryGetComponent<Renderer>(out var meshRenderer)
            ? new[] { meshRenderer }
            : GetComponentsInChildren<Renderer>();
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
            if(x != isOnHover.Value)
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

    private void SetOutline()
    {
        foreach (var rend in renderers)
        {
            if(isOnHover.Value)
            {
                rend.renderingLayerMask = originalLayer |  OnHoverOutlineLayer;
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
