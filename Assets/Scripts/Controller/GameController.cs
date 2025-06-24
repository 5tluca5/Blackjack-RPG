using GamConstant;
using UnityEngine;
using UniRx;
using System.Collections;
using System.Collections.Generic;
using CardAttribute;
using System.Linq;
using UnityEngine.Rendering.Universal;
using System;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("References")]
    [SerializeField] HUDController hudController;
    [SerializeField] DeckController deckController;
    [SerializeField] BetPhaseManager betPhaseManager;
    [SerializeField] CameraRaycast cameraRaycast;
    [SerializeField] DealerObject dealer;
    [SerializeField] ChipZone chipZone;

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
        hudController.SetupScoreboard(zones.ToDictionary(x => x.Key, x => x.Value.GetPlayerProfile()));

        currentPhase.Subscribe(x =>
        {
            switch (x)
            {
                case GamePhase.ShuffleDeck:
                    PhaseShuffleDeck().ToObservable().Subscribe();
                    break;
                case GamePhase.PlayerBet:
                    PhasePlayerBet();
                    break;
                case GamePhase.DealingCards:
                    PhaseDealingCards().ToObservable().Subscribe();
                    break;
                case GamePhase.PlayerTurn:
                    EnableRaycast();
                    break;
                case GamePhase.DealerTurn:
                    PhaseDealerTurn();
                    break;
                case GamePhase.SettlementPhase:
                    PhaseSettlementPhase().ToObservable().Subscribe();
                    break;
                case GamePhase.NextRound:
                    PhaseNextRound().ToObservable().Subscribe();
                    break;
            }

            GameLogger.Instance.Log("Current Phase: " + x);

        }).AddTo(this);

        SetGamePhase(GamePhase.ShuffleDeck);
    }

    // Update is called once per frame
    void Update()
    {
        if (raycastCooldown > 0)
        {
            raycastCooldown = Mathf.Max(0, raycastCooldown - Time.deltaTime);

            if (raycastCooldown <= 0)
                cameraRaycast.setRaycastEnable(true);
        }
    }

    #region Game Phases
    IEnumerator PhaseShuffleDeck()
    {
        DisableRaycast();

        ResetTurn();

        deckController.InitDeck();
        deckController.ShuffleDeck(2f);

        yield return new WaitUntil(() => !deckController.IsShuffling());

        yield return new WaitForSeconds(1 * (1 / GameSpeed));

        SetGamePhase(GamePhase.PlayerBet);

        //InitialDealCard().ToObservable().Subscribe(x =>
        //{
        //    Debug.Log("InitialDealCard completed");
        //    currentPhase.Value = GamePhase.PlayerTurn;
        //}).AddTo(this);

        yield return null;
    }

    void PhasePlayerBet()
    {
        if (betPhaseManager.IsBettingPhaseActive()) return;

        EnableRaycast();
        chipZone.SetPlayerChipValue(GetCurrentPlayerProfile().Chips);
        chipZone.BetPhaseStarted();

        betPhaseManager.StartBettingPhase(playersZone.Values.Where(x => x.Owner != Players.Dealer).ToList());
    }

    IEnumerator PhaseDealingCards()
    {
        if (deckController.IsShuffling() || !IsInitialDeal()) yield return null;

        List<Players> order = new List<Players>() { Players.Player3, Players.Player1, Players.Player2, Players.Dealer };

        for (int i = 0; i < 2; i++)
        {
            foreach (var p in order)
            {
                if (playersZone.ContainsKey(p))
                {
                    DealCard(p);
                    yield return new WaitForSeconds(0.5f * (1 / Instance.GameSpeed));
                }
            }
        }


        yield return new WaitForSeconds(1 * (1 / GameSpeed));

        SetGamePhase(GamePhase.PlayerTurn);
    }

    void PhaseDealerTurn()
    {
        DisableRaycast();

        var onCompleted = dealer.ExecuteDealerActionPhase();

        if (onCompleted != null)
        {
            onCompleted.Subscribe(_ =>
            {
                SetGamePhase(GamePhase.SettlementPhase);
            }).AddTo(this);
        }
    }

    IEnumerator PhaseSettlementPhase()
    {
        DisableRaycast();

        var winner = DetermineWinner(Players.Dealer, currentPlayer);
        var pp = GetCurrentPlayerProfile();
        var dp = playersZone[Players.Dealer].GetPlayerProfile();
        var betValue = playersZone[currentPlayer].GetBetValue();
        int winningValue = 0;

        if (winner == currentPlayer)
        {
            winningValue = playersZone[currentPlayer].IsBlackJack() ? (int)(betValue * 1.5f) : betValue;

            GameLogger.Instance.Log($"[{pp.LogPlayerName}] Won {betValue.LogWinningChips()}!");
            pp.AddChips(betValue + winningValue);
            dp.AddChips(-winningValue);
        }
        else if (winner == Players.Dealer)
        {
            GameLogger.Instance.Log($"[{pp.LogPlayerName}] Lost {betValue.LogLossingChips()}!");
            dp.AddChips(betValue);
        }
        else
        {
            GameLogger.Instance.Log($"[{pp.LogPlayerName}] Tied!");
            pp.AddChips(betValue);
        }

        yield return new WaitForSeconds(2 * (1 / GameSpeed));

        EnableRaycast();
        //SetGamePhase(GamePhase.NextRound);
    }

    IEnumerator PhaseNextRound()
    {
        DisableRaycast();
        ResetTurn();

        yield return new WaitForSeconds(1 * (1 / GameSpeed));

        SetGamePhase(GamePhase.ShuffleDeck);
    }
    #endregion

    public bool CanRaycast() => raycastCooldown <= 0f;
    public void SetGamePhase(GamePhase phase) => currentPhase.Value = phase;
    public Players GetCurrentPlayer() => currentPlayer;
    public PlayerProfile GetCurrentPlayerProfile() => playersZone[currentPlayer].GetPlayerProfile();
    public ReactiveProperty<GamePhase> OnGamePhaseChanged() => currentPhase;
    public GamePhase GetCurrentPhase() => currentPhase.Value;
    public ChipType GetSelectingChip() => chipZone.GetSelectingChipType();
    public ChipZone GetChipZone() => chipZone;
    public bool IsBetPhase() => currentPhase.Value == GamePhase.PlayerBet;

    public bool IsPlayerTurn() => currentPhase.Value == GamePhase.PlayerTurn;
    public bool IsInitialDeal() => currentPhase.Value == GamePhase.DealingCards;
    public bool IsSettlementPhase() => currentPhase.Value == GamePhase.SettlementPhase;
    public bool IsDealerInteractable()
    {
        return (IsPlayerTurn() && playersZone[currentPlayer].IsBothCardRevealed())
            || (IsBetPhase() && playersZone[currentPlayer].GetBetValue() > 0)
            || IsSettlementPhase();
    }
    public void ReceivedInput(KeyCode keyCode)
    {
        GameObject go = cameraRaycast.GetRaycastedObject();
        
        if (go != null && go.TryGetComponent(out Interactable interactable) && interactable.IsInteractable())
        {
            interactable.Interact(keyCode);
        }
    }

    InteractableOutline lastInteractableOutline;
    public void OnHovorGameObject(GameObject go)
    {
        if(go != null && go.TryGetComponent(out InteractableOutline interactableOutline))
        {
            if (lastInteractableOutline != null && lastInteractableOutline != interactableOutline)
            {
                lastInteractableOutline.SetOnHover(false);
            }

            interactableOutline.SetOnHover(true);
            lastInteractableOutline = interactableOutline;
        }
        else
        {
            if (lastInteractableOutline != null)
            {
                lastInteractableOutline.SetOnHover(false);
                lastInteractableOutline = null;
            }
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

    public void BetPhaseEnded()
    {
        playersZone[currentPlayer].EndPlacingBet();
        PlayerBetConfirmed();

        SetGamePhase(GamePhase.DealingCards);
    }

    public void PlayerBetConfirmed()
    {
        if (playersZone[currentPlayer].IsBetPlaced()) return;

        playersZone[currentPlayer].ConfirmBet();
        chipZone.BetPhaseEnded();

        GameLogger.Instance.Log($"[{GetCurrentPlayerProfile().LogPlayerName}] placed bet: {playersZone[currentPlayer].GetBetValue().LogWinningChips()}.");
    }

    public void PlayerAddChipToBetZone()
    {
        if(!playersZone[currentPlayer].AddChipToBetZone(GetSelectingChip()))
        {
            // Show log
        }
        else
        {
            dealer.SetInteractable(IsDealerInteractable());
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
        SetGamePhase(GamePhase.DealerTurn);

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

    public void PlayerGoToNextRound()
    {
        SetGamePhase(GamePhase.NextRound);
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
                var owner = cardset.GetOwner();
                var player = playersZone[owner];

                hudController.UpdateScoreboardPoint(owner, x);

                if (player.IsBusted())
                {
                    SetGamePhase(GamePhase.SettlementPhase);
                }
                else if (player.IsBothCardRevealed())
                {
                    dealer.SetInteractable(IsDealerInteractable());
                }

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

    public Players DetermineWinner(Players dealer,  Players player)
    {
        // Adjust logic here
        var dp = playersZone[dealer];
        var pp = playersZone[player];

        if (pp.IsBusted()) return dealer;
        if (dp.IsBusted()) return player;

        //If the dealer takes five cards without busting the cards, it is considered a victory for the dealer.
        if (dp.GetCurrentCardSet().GetCardCount() >= 5) return dealer;

        if (pp.GetTotalPoint() > dp.GetTotalPoint()) return player;
        if (pp.GetTotalPoint() < dp.GetTotalPoint()) return dealer;

        return Players.NullPlayer;  // Draw
    }
}