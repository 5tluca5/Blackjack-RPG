using System.Collections.Generic;
using UnityEngine;
using UniRx;
using GamConstant;
using System.Collections;
using System.Linq;

public class DeckController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject cardPrefab;
    [SerializeField] DeckObject deckObject;
    [SerializeField] List<Transform> dealerCardPositions;
    [SerializeField] List<Transform> playerCardPositions;
    [SerializeField] CardZone dealerCardZone;
    [SerializeField] CardZone playerCardZone;

    [Header("Parameters")]
    [SerializeField, Range(1, 10)] int deckSize = 1;

    Deck deck = new();

    Dictionary<Players, List<CardDisplay>> cardDict = new();
    Dictionary<Players, List<Transform>> cardPosDict = new();
    Dictionary<Players, CardZone> cardZoneDict = new();

    public List<Card> GetCardList() => deck.GetCardList();

    public int GetRemainingCardSize() => GetCardList().Count;

    public int GetMaximumCardSize() => deckSize * 52;

    public bool IsShuffling() => deckObject.IsShuffling();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(deckObject == null)
        {
            deckObject = FindAnyObjectByType<DeckObject>();
        }

        GetCardList().ObserveEveryValueChanged(cards => cards.Count).Subscribe(x =>
        {
            deckObject.SetDeckSize((float)x / GetMaximumCardSize());
        }).AddTo(this);

        cardPosDict = new Dictionary<Players, List<Transform>>
        {
            {Players.Dealer, dealerCardPositions},
            {Players.Player1, playerCardPositions},
        };

        cardZoneDict = new Dictionary<Players, CardZone>
        {
            {Players.Dealer, dealerCardZone},
            {Players.Player1, playerCardZone},
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitDeck()
    {
        deck.InitDeck(deckSize);
        deckObject.SetDeckOriginalHeight(deckSize);

        cardDict = new Dictionary<Players, List<CardDisplay>> {
            { Players.Dealer, new List<CardDisplay>()},
            { Players.Player1, new List<CardDisplay>()},
            { Players.Player2, new List<CardDisplay>()},
            { Players.Player3, new List<CardDisplay>()},
        };
    }

    public void ShuffleDeck(float duration)
    {
        deck.ShuffleDeck();
        deckObject.PlayShuffleAnimation(duration * (1 / GameController.Instance.GameSpeed));
    }

    public void DealCard(Players target)
    {
        if (deckObject.IsShuffling()) return;

        var card = deck.DrawCard();
        var refPos = cardPosDict[target];
        var refTF = refPos[Mathf.Min(refPos.Count-1, cardDict[target].Count)];

        var cd = deckObject.DealCard(target, card, refTF);

        cd.OnCardRevealed().Subscribe(x =>
        {
            GameController.Instance.UpdatePlayerCard(target, x);
        }).AddTo(cd);

        cd.OnCardRevealCompleted().Subscribe(x =>
        {
            if (cardDict[target].Where(c => c.IsRevealed()).Count() >= 2)
            {
                cardZoneDict[target].AddCards(cardDict[target]);
            }
        }).AddTo(cd);

        cardDict[target].Add(cd);

        if (target == Players.Dealer && cardDict[target].Count >= 2)
        {
            cd.FlipCard();
        }
    }

    public IEnumerator InitialDealCard()
    {
        if (deckObject.IsShuffling() || !GameController.Instance.IsInitialDeal()) yield return null;

        for(int i=0; i<2; i++)
        {
            for (Players p = Players.Dealer; p <= Players.Player1; p++)
            {
                DealCard(p);
                yield return new WaitForSeconds(0.5f * (1 / GameController.Instance.GameSpeed));
            }
        }
    }
}
