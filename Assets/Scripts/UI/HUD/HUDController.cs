using GamConstant;
using System.Collections.Generic;
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
    [SerializeField] PromptGroup promptPlayerBetAction;
    [SerializeField] PromptGroup promptPlayerRegularAction;
    [SerializeField] PromptGroup promptPlayerNextRoundAction;

    [Header("Scoreboard")]
    [SerializeField] Scoreboard scoreboard;

    [Header("Bet Timer")]
    [SerializeField] BetTimer betTimer;

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
            bool isInteractable = interactable.IsInteractable();

            if (go.CompareTag("PlayerCard") && isInteractable)
            {
                promptPlayerCard.Show();
            }
            else if(go.CompareTag("Dealer") && isInteractable)
            {
                if (GameController.Instance.IsBetPhase())
                    promptPlayerBetAction.Show();
                else if (GameController.Instance.IsPlayerTurn())
                    promptPlayerRegularAction.Show();
                else if (GameController.Instance.IsSettlementPhase())
                    promptPlayerNextRoundAction.Show();
            }

            SwitchAimColor(isInteractable);

            return;
        }

        SwitchAimColor(false);
    }

    void SwitchAimColor(bool isInteractable)
    {
        aimDot.color = isInteractable ? aimColorInteractable : aimColorNormal;
    }

    public void SetupScoreboard(Dictionary<Players, PlayerProfile> playerProfiles)
    {
        scoreboard.Setup(playerProfiles);
    }
    public void UpdateScoreboardPoint(Players player, int point)
    {
        scoreboard.UpdatePoint(player, point);
    }

    public void ShowBetTimer(float initTime, float showAnimationDuration = 0.5f)
    {
        betTimer.Show(initTime, showAnimationDuration);
    }

    public void HideBetTimer(float hideAnimationDuration = 0.5f)
    {
        betTimer.Hide(hideAnimationDuration);
    }

    public void UpdateBetTimer(float time)
    {
        betTimer.UpdateTimer(time);
    }
}
