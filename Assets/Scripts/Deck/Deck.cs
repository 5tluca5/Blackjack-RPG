using CardAttribute;
using System.Collections.Generic;
using UnityEngine;

public class Deck 
{
    List<Card> cardList = new();
    public List<Card> GetCardList() => cardList;

    public Deck()
    {

    }

    public void InitDeck(int deckSize)
    {
        cardList.Clear();

        int index = 0;

        for (int i = 0; i <deckSize; i++)
        {
            for(CardSuit s = CardSuit.Spade; s <= CardSuit.Diamond; s++)
            {
                for (CardRank r = CardRank.Ace; r <= CardRank.King; r++)
                {
                    cardList.Add(new Card(index, s, r));
                }
            }
        }
            
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            int randomIndex = Random.Range(0, cardList.Count);

            // Swap the current card with a random card
            Card temp = cardList[i];
            cardList[i] = cardList[randomIndex];
            cardList[randomIndex] = temp;
        }

        // Update the index for each card based on the new order
        for (int i = 0; i < cardList.Count; i++)
        {
            cardList[i].UpdateIndex(i);
        }
    }

    public Card DrawCard()
    {
        var card = cardList[0];
        cardList.RemoveAt(0);

        return card;
    }
}
