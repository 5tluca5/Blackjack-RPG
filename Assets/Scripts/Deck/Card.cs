using System;
using UnityEngine;
using CardAttribute;

namespace CardAttribute
{
    [Serializable]
    public enum CardSuit
    {
        Spade,
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
    public enum CardRank
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

[Serializable, CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
public class Card : ScriptableObject
{
    public int index { get; private set; }

    [SerializeField] CardSuit suit;
    [SerializeField] CardType type;
    [SerializeField] CardRank rank;

    public int GetRank() => (int)rank;  

    public Card(CardSuit suit, CardRank rank)
    {
        this.suit = suit;
        this.rank = rank;
    }
    
}