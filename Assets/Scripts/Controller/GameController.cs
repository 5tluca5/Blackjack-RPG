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
    [SerializeField] DealerObject dealer;

    [Header("Parameters")]
    [SerializeField] ReactiveProperty<GamePhase> currentPhase = new();
    [SerializeField] public float GameSpeed { get; private set; } = 1f;

    [Header("Game Data")]
    [SerializeField] Players currentPlayer = Players.Player1;
    [SerializeField] int currentCardSetIndex = 0;

    Dictionary<Players, CardZone> playersCardZone = new();

    //Raycast stuff
    float raycastCooldown = 0f;


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
        playersCardZone = new Dictionary<Players, CardZone>
        {
            {Players.Dealer, deckController.GetCardZone(Players.Dealer)},
            {Players.Player1, deckController.GetCardZone(Players.Player1)},
        };

        currentPhase.Subscribe(x =>
        {
            switch (x)
            {
                case GamePhase.InitialDeal:
                    DisableRaycast();
                    break;
                case GamePhase.PlayerTurn:
                    EnableRaycast();
                    break;
                case GamePhase.DealerTurn:
                    DisableRaycast();
                    break;
                case GamePhase.SettlementPhase:
                    DisableRaycast();
                    break;
                case GamePhase.NextRound:
                    break;
            }

            Debug.Log("Current Phase: " + x);

        }).AddTo(this);

        StartNewTurn().ToObservable().Subscribe();
    }

    // Update is called once per frame
    void Update()
    {
        if(raycastCooldown > 0)
        {
            raycastCooldown = Mathf.Max(0, raycastCooldown - Time.deltaTime);

            if (raycastCooldown <= 0)
                cameraRaycast.setRaycastEnable(true);
        }
    }

    public IEnumerator StartNewTurn()
    {
        currentPhase.Value = GamePhase.InitialDeal;

        ResetTurn();

        deckController.InitDeck();
        deckController.ShuffleDeck(2f);

        yield return new WaitUntil(() => !deckController.IsShuffling());

        yield return new WaitForSeconds(1 * (1 / GameSpeed));

        deckController.InitialDealCard().ToObservable().Subscribe(x =>
        {
            Debug.Log("InitialDealCard completed");
            currentPhase.Value = GamePhase.PlayerTurn;
        }).AddTo(this);

        yield return null;
    }

    public bool CanRaycast() => raycastCooldown <= 0f;
    public Players GetCurrentPlayer() => currentPlayer;
    public bool IsPlayerTurn() => currentPhase.Value == GamePhase.PlayerTurn;
    public bool IsInitialDeal() => currentPhase.Value == GamePhase.InitialDeal;
    public bool IsDealerInteractable() => IsPlayerTurn() && playersCardZone[currentPlayer].IsBothCardRevealed();
    public void ReceivedInput(KeyCode keyCode)
    {
        GameObject go = cameraRaycast.GetRaycastedObject();

        if (go != null && go.TryGetComponent(out Interactable interactable))
        {
            interactable.Interact(keyCode);
        }
    }

    public void DealCard(Players player, int cardsetIndex)
    {
        deckController.DealCard(player, cardsetIndex);
    }
    #region Player regular actions

    public void RevealCard(CardDisplay cardDisplay, RevealCardType type)
    {
        if (cardDisplay.GetOwner() != Players.Dealer && currentPhase.Value == GamePhase.PlayerTurn)
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

    public void PlayerHit()
    {
        deckController.DealCard(currentPlayer, 0);
        SetRaycastCooldown(1f);
    }

    public void PlayerStand()
    {
        // Next phase
        currentPhase.Value = GamePhase.DealerTurn;

        var onCompleted = dealer.ExecuteDealerActionPhase();

        if(onCompleted != null)
        {
            onCompleted.Subscribe(_ =>
            {
                currentPhase.Value = GamePhase.SettlementPhase;
            }).AddTo(this);
        }
    }

    public void PlayerSplit()
    {

    }

    public void PlayerDoubleDown()
    {

    }

    public void PlayerSurrender()
    {

    }

    #endregion
    public void UpdatePlayerCardSetPoint(Players player, int cardSetIndex)
    {
        if (playersCardZone.TryGetValue(player, out var cardZone))
        {
            cardZone.GetCardSet(cardSetIndex).CalculateRevealedCardPoint();
        }
    }

    public void SubscribeCardSetPoint(CardSet cardset)
    {
        if (cardset != null)
        {
            cardset.SubscribeRevealedCardPoint().Subscribe(x =>
            {
                hudController.UpdateScoreboardPoint(cardset.GetOwner(), x);
            }).AddTo(cardset);
        }
    }

    void ResetTurn()
    {
        foreach (var playerCardZone in playersCardZone)
        {
            if(playerCardZone.Value != null)
                playerCardZone.Value.ResetCardZone();
        }
    }

    public void SetRaycastCooldown(float f)
    {
        raycastCooldown = f * (1 / GameSpeed);

        if(f > 0)
            cameraRaycast.setRaycastEnable(false);
    }

    public void DisableRaycast()
    {
        raycastCooldown = float.MaxValue;
        cameraRaycast.setRaycastEnable(false);
    }
    public void EnableRaycast()
    {
        raycastCooldown = 0;
        cameraRaycast.setRaycastEnable(true);
    }
}