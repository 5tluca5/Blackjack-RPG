using UnityEngine;

public class PromptGroup : Fadable
{
    bool isShowing = false;
    public override void Show(float endValue = 1, float fadeInTime = 0.1F)
    {
        isShowing = true;

        base.Show(endValue, fadeInTime);
    }

    private void Update()
    {
        if(!isShowing)
            Hide();

        isShowing = false;
    }
}
