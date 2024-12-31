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
    [SerializeField] BetPhaseManager betPhaseManager;
    [SerializeField] CameraRaycast cameraRaycast;
    [SerializeField] DealerObject dealer;

    [Header("Parameters")]
    [SerializeField] ReactiveProperty<GamePhase> currentPhase = new();
    [SerializeField] public float GameSpeed { get; private set; } = 1f;

    [Header("Game Data")]
    [SerializeField] Players currentPlayer = Players.Player1;
    Dictionary<Players, PlayerZone> playersZone = new();

    [Header("Debug")]
    [SerializeField] public PlayerProfile PlayerProfile;
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
        var zones = GameObject.FindGameObjectsWithTag("PlayerZone").Select(x => x.GetComponent<PlayerZone>()).ToDictionary(x => x.Owner);

        playersZone = zones;

        currentPhase.Subscribe(x =>
        {
            switch (x)
            {
                case GamePhase.ShuffleDeck:
                    DisableRaycast();
                    break;
                case GamePhase.PlayerBet:
                    EnableRaycast();
                    BetPhaseStarted();
                    break;
                case GamePhase.DealingCards:
                    InitialDealCard();
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
        currentPhase.Value = GamePhase.ShuffleDeck;

        ResetTurn();

        deckController.InitDeck();
        deckController.ShuffleDeck(2f);

        yield return new WaitUntil(() => !deckController.IsShuffling());

        yield return new WaitForSeconds(1 * (1 / GameSpeed));

        currentPhase.Value = GamePhase.PlayerBet;

        //InitialDealCard().ToObservable().Subscribe(x =>
        //{
        //    Debug.Log("InitialDealCard completed");
        //    currentPhase.Value = GamePhase.PlayerTurn;
        //}).AddTo(this);
        
        yield return null;
    }

    public IEnumerator InitialDealCard()
    {
        if (deckController.IsShuffling() || !IsInitialDeal()) yield return null;

        for (int i = 0; i < 2; i++)
        {
            for (Players p = Players.Dealer; p <= Players.Player1; p++)
            {
                DealCard(p);
                yield return new WaitForSeconds(0.5f * (1 / Instance.GameSpeed));
            }
        }
    }
    public bool CanRaycast() => raycastCooldown <= 0f;
    public Players GetCurrentPlayer() => currentPlayer;
    public bool IsPlayerTurn() => currentPhase.Value == GamePhase.PlayerTurn;
    public bool IsInitialDeal() => currentPhase.Value == GamePhase.DealingCards;
    public bool IsDealerInteractable() => IsPlayerTurn() && playersZone[currentPlayer].IsBothCardRevealed();
    public void ReceivedInput(KeyCode keyCode)
    {
        GameObject go = cameraRaycast.GetRaycastedObject();

        if (go != null && go.TryGetComponent(out Interactable interactable))
        {
            interactable.Interact(keyCode);
        }
    }

    public void DealCard(Players player)
    {
        if (!playersZone.ContainsKey(player)) return;

        deckController.DealCard(playersZone[player]);
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

    public void BetPhaseStarted()
    {
        if (betPhaseManager.IsBettingPhaseActive()) return;

        betPhaseManager.StartBettingPhase(playersZone.Values.ToList());
    }

    public void BetPhaseEnded()
    {

    }

    public void PlayerBetConfirmed()
    {

    }

    public void PlayerAddChipToBetZone(ChipType chipType)
    {
        if(!playersZone[currentPlayer].AddChipToBetZone(chipType))
        {
            // Show log
        }
    }

    public void PlayerHit()
    {
        deckController.DealCard(playersZone[currentPlayer]);
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
        if (playersZone.TryGetValue(player, out var playerZone))
        {
            playerZone.GetCardSet(cardSetIndex).CalculateRevealedCardPoint();
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
        foreach (var playerZone in playersZone)
        {
            if(playerZone.Value != null)
                playerZone.Value.Reset();
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