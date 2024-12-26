using GamConstant;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance { get; private set; }

    [Header("References")]

    [Header("Aim dot")]
    [SerializeField] Image aimDot;
    [SerializeField, ColorUsage(true)] Color aimColorNormal;
    [SerializeField, ColorUsage(true)] Color aimColorInteractable;

    [Header("Prompts")]
    [SerializeField] PromptGroup promptPlayerCard;

    [Header("Scoreboard")]
    [SerializeField] Scoreboard scoreboard;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        if(go != null && go.TryGetComponent(out Interactable interactable))
        {
            
            if(go.CompareTag("PlayerCard") && interactable.IsInteractable())
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

    public void UpdateScoreboardPoint(Players player, int point)
    {
        scoreboard.UpdatePoint(player, point);
    }
}
