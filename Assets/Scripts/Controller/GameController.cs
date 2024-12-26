using GamConstant;
using UnityEngine;
using UniRx;
using System.Collections;
using System.Collections.Generic;
using CardAttribute;
using System.Linq;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("References")]
    [SerializeField] HUDController hudController;
    [SerializeField] DeckController deckController;
    [SerializeField] CameraRaycast cameraRaycast;

    [Header("Parameters")]
    [SerializeField] GamePhase currentPhase;
    [SerializeField] public float GameSpeed { get; private set; } = 1f;

    [Header("Game Data")]
    [SerializeField] Players currentPlayer = Players.Player1;
    Dictionary<Players, List<Card>> playersCard = new();
    Dictionary<Players, ReactiveProperty<int>> playersPoint = new();

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playersCard = new Dictionary<Players, List<Card>>
        {
            {Players.Dealer, new List<Card>()},
            {Players.Player1, new List<Card>()},
        };

        playersPoint = new Dictionary<Players, ReactiveProperty<int>>
        {
            {Players.Dealer, new ReactiveProperty<int>(0)},
            {Players.Player1, new ReactiveProperty<int>(0)},
        };

        foreach (var playerPoint in playersPoint)
        {
            playerPoint.Value.Subscribe(x =>
            {
                hudController.UpdateScoreboardPoint(playerPoint.Key, x);
            }).AddTo(this);
        }

        StartNewTurn().ToObservable().Subscribe();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator StartNewTurn()
    {
        currentPhase = GamePhase.InitialDeal;

        ResetTurn();

        deckController.InitDeck();
        deckController.ShuffleDeck(2f);

        yield return new WaitUntil(() => !deckController.IsShuffling());

        yield return new WaitForSeconds(1 * (1 / GameSpeed));

        deckController.InitialDealCard().ToObservable().Subscribe(x =>
        {
            Debug.Log("InitialDealCard completed");
            currentPhase = GamePhase.PlayerTurn;
        }).AddTo(this);

        yield return null;
    }

    public Players GetCurrentPlayer() => currentPlayer; 
    public bool IsPlayerTurn() => currentPhase == GamePhase.PlayerTurn;
    public bool IsInitialDeal() => currentPhase == GamePhase.InitialDeal;

    public void ReceivedInput(KeyCode keyCode)
    {
        GameObject go = cameraRaycast.GetRaycastedObject();

        if (go != null && go.TryGetComponent(out Interactable interactable))
        {
            interactable.Interact(keyCode);
        }
    }

    public void DealCard()
    {
        deckController.DealCard(Players.Player1);
    }

    public void RevealCard(CardDisplay cardDisplay, RevealCardType type)
    {
        if (cardDisplay.GetOwner() != Players.Dealer && currentPhase == GamePhase.PlayerTurn)
        {
            if (type == RevealCardType.Flip)
            {
                cardDisplay.FlipCard();
            }
            else
            {
                cardDisplay.PeakCard();
            }
        }
    }

    public void UpdatePlayerCard(Players player, Card card)
    {
        playersCard[player].Add(card);

        int point = 0;

        foreach (var c in playersCard[player].OrderByDescending(x => x.rank).ToList())
        {
            var cPoint = c.rank >= CardRank.Ten ? 10 : (int)c.rank;

            if (c.rank == CardRank.Ace && point + 11 <= 21)
            {
                cPoint = 11;
            }

            point += cPoint;
        }

        playersPoint[player].Value = point;
    }

    void ResetTurn()
    {
        foreach (var playerPoint in playersPoint)
        {
            playerPoint.Value.Value = 0;
        }

        foreach (var playerCard in playersCard)
        {
            playerCard.Value.Clear();
        }
    }
}
