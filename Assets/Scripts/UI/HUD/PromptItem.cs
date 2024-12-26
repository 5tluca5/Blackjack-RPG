using UnityEngine;
using UnityEngine.UI;

public class PromptItem : MonoBehaviour
{
    [SerializeField] KeyCode keyCode;
    [SerializeField] Image promptIcon;
    [SerializeField] TMPro.TextMeshProUGUI promptText;
    [SerializeField] Color onPressColor;
    [SerializeField] Color onNormalColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(keyCode))
        {
            promptIcon.color = onPressColor;
            promptText.color = onPressColor;
        }
        else
        {
            promptIcon.color = onNormalColor;
            promptText.color = onNormalColor;
        }
    }
}
