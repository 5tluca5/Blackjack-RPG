using System;
using UnityEngine;
using CardAttribute;

namespace CardAttribute
{
    [Serializable]
    public enum CardSuit : int
    {
        Spade = 0,
        Heart,
        Club,
        Diamond
    }

    public enum CardType
    {
        Number,
        Face,
        Ace
    }

    [Serializable]
    public enum CardRank : int
    {
        Ace = 1,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
    }
}

[Serializable]
public class Card
{
    public int index { get; private set; }
    public CardSuit suit { get; private set; }
    public CardRank rank { get; private set; }

    public int GetRank() => (int)rank;  

    public Card(int index, CardSuit suit, CardRank rank)
    {
        this.index = index;
        this.suit = suit;
        this.rank = rank;
    }

    public void UpdateIndex(int i) => this.index = i;
}