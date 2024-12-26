using UnityEngine;

public class PromptGroup : Fadable
{
    bool isShowing = false;

    public override void Show(float endValue = 1, float fadeInTime = 0.25f)
    {
        isShowing = true;

        base.Show(endValue, 0.25f);
    }

    private void Update()
    {
        if(!isShowing)
            Hide();

        isShowing = false;
    }
}
