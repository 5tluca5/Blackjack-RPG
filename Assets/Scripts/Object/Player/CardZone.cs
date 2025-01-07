using GamConstant;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class CardZone : MonoBehaviour
{
    Players owner;
    CardSet currentCardSet;
    Dictionary<int, CardSet> cardSets = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(PlayerZone playerZone)
    {
        owner = playerZone.Owner;

        if (currentCardSet == null && cardSets.Count <= 0)
            currentCardSet = CreateCardSet();
    }

    public CardSet AddCard(CardDisplay card, int cardSetIndex)
    {
        if (!cardSets.ContainsKey(cardSetIndex))
        {
            var cardSet = CreateCardSet();

            if(currentCardSet == null)
            {
                currentCardSet = cardSet;
            }

            cardSet.AddCard(card);

            return cardSet;
        }

        cardSets[cardSetIndex].AddCard(card);

        return cardSets[cardSetIndex];
    }

    public void Rearrange()
    {
        if (currentCardSet != null)
        {
            currentCardSet.Rearrange();
        }
    }

    public void SetCurrentCardSet(int index)
    {
        if (cardSets.ContainsKey(index))
        {
            currentCardSet = cardSets[index];
        }
    }

    public void SwapCardSet()
    {
        if (currentCardSet != null)
        {
            var index = currentCardSet.GetIndex();
            index++;
            if (index >= cardSets.Count)
            {
                index = 0;
            }

            SetCurrentCardSet(index);
        }
    }

    public CardSet GetCurrentCardSet()
    {
        if (currentCardSet == null && cardSets.Count <= 0)
        {
            currentCardSet = CreateCardSet();
        }
        else if(cardSets.Count > 0)
        {
            currentCardSet = cardSets.Values.First();
        }

        return currentCardSet;
    }

        public CardSet GetCardSet(int index)
    {
        if (cardSets.ContainsKey(index))
        {
            return cardSets[index];
        }

        return null;
    }

    public void ResetCardZone()
    {
        foreach(var carset in cardSets.Values)
        {
            carset.ResetCardset();
        }
        cardSets.Clear();
    }

    public bool IsBothCardRevealed() => currentCardSet.GetCardDisplays().Count(x => x.IsRevealed()) >= 2;

    public void FlipNextCard()
    {
        if (owner != Players.Dealer) return;

        if (currentCardSet != null)
        {
            currentCardSet.FlipNextCard();
        }
    }

    CardSet CreateCardSet()
    {
        var go = new GameObject("CardSet");
        var cardSet = go.AddComponent<CardSet>();
        cardSet.SetIndex(0);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        cardSets[0] = cardSet;

        if (currentCardSet == null)
        {
            currentCardSet = cardSet;
        }

        cardSet.SetOwner(owner);

        return cardSet;
    }
}
