using UnityEngine;

public class RuleController : MonoBehaviour
{
    public static RuleController Instance { get; private set; }

    [SerializeField] GameRule inputGameRule;

    private GameRule gameRule;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if(inputGameRule != null)
        {
            gameRule = Instantiate(inputGameRule);
        }
    }

    public int MinBet => gameRule.minBet;
    public int MaxBet => gameRule.maxBet;
    public int NumberOfDecks => gameRule.numberOfDecks;
    public int BustPoint => gameRule.bustPoint;
    public int BlackJackPoint => gameRule.blackJackPoint;
    public int DealerMinPoint => gameRule.dealerMinPoint;
    public int MaxSplits => gameRule.maxSplits;
    public bool AllowSurrender => gameRule.allowSurrender;
    public bool AllowSplitting => gameRule.allowSplitting;
    public bool AllowDoubleDown => gameRule.allowDoubleDown;
    public float BlackjackPayout => gameRule.blackjackPayout;
    public float InsurancePayout => gameRule.insurancePayout;
    public float BetPhaseTimeLimit => gameRule.betPhaseTimeLimit;
    public float DecisionTimeLimit => gameRule.decisionTimeLimit;

}
