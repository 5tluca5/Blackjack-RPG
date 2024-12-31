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

    private void Awake()
    {

    }

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

    }

    // Update is called once per frame
    void Update()
    {

    }

    public CardZone GetCardZone(Players player) => cardZoneDict[player];

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

    public void DealCard(PlayerZone target)
    {
        if (deckObject.IsShuffling()) return;

        var card = deck.DrawCard();
        var refPos = target.GetCardPosRefs();
        var cardSet = target.GetCurrentCardSet();
        var refTF = refPos[Mathf.Min(refPos.Count-1, cardSet.GetCardDisplays().Count)];

        var cd = deckObject.DealCard(target.Owner, card, refTF);
        var cs = target.AddCard(cd);

        if(cs != null)
        {
            GameController.Instance.SubscribeCardSetPoint(cs);
        }

        if (target.Owner == Players.Dealer && cardSet.GetCardDisplays().Count >= 2)
        {
            cd.FlipCard();
        }
    }

}
