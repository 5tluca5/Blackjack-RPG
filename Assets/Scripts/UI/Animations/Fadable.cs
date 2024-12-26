using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class Fadable : MonoBehaviour
{
    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void Show(float endValue = 1f, float fadeInTime = 0.25f)
    {
        if (gameObject.activeSelf) return;

        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;

        canvasGroup.DOFade(endValue, fadeInTime);
    }

    public virtual void Hide(float endValue = 0f, float fadeOutTime = 0.25f)
    {
        if (!gameObject.activeSelf) return;

        canvasGroup.DOFade(endValue, fadeOutTime).onComplete += () =>
        {
            gameObject.SetActive(false);
        };
    }
}
