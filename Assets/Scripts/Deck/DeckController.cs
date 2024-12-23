using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class DeckController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject cardPrefab;
    [SerializeField] DeckObject deckObject;
    [SerializeField] List<Transform> dealerCardPositions;
    [SerializeField] List<Transform> playerCardPositions;

    [Header("Parameters")]
    [SerializeField, Range(1, 10)] int deckSize = 1;

    Deck deck = new();

    int playerCardCounter = 0;

    public List<Card> GetCardList() => deck.GetCardList();

    public int GetRemainingCardSize() => GetCardList().Count;

    public int GetMaximumCardSize() => deckSize * 52;

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

        
        InitDeck();
        ShuffleDeck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitDeck()
    {
        deck.InitDeck(deckSize);
        deckObject.SetDeckOriginalHeight(deckSize);

        playerCardCounter = 0;
    }

    public void ShuffleDeck()
    {
        deck.ShuffleDeck();
        deckObject.PlayShuffleAnimation(3f);
    }

    public void DealCard()
    {
        var card = deck.DrawCard();
        var refTF = playerCardPositions[Mathf.Min(playerCardPositions.Count-1, playerCardCounter)];

        deckObject.DealCard(card, refTF);

        playerCardCounter++;
    }
}
