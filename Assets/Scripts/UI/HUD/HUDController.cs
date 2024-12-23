using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("References")]

    [Header("Aim dot")]
    [SerializeField] Image aimDot;
    [SerializeField, ColorUsage(true)] Color aimColorNormal;
    [SerializeField, ColorUsage(true)] Color aimColorInteractable;

    [Header("Prompts")]
    [SerializeField] PromptGroup promptPlayerCard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAimTarget(GameObject go)
    {
        if(go != null)
        {
            if(go.CompareTag("PlayerCard"))
            {
                SwitchAimColor(true);
                promptPlayerCard.Show();
            }
            else
            {
                SwitchAimColor(false);
            }

            return;
        }

        SwitchAimColor(false);
    }

    void SwitchAimColor(bool isInteractable)
    {
        aimDot.color = isInteractable ? aimColorInteractable : aimColorNormal;
    }


}
