using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;
using UniRx;
using CardAttribute;
using System.Linq;
using GamConstant;

public class CardSet : MonoBehaviour
{
    Players owner;
    [SerializeField] int index;
    List<CardDisplay> cards = new();

    ReactiveProperty<int> revealedCardPoint = new(0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIndex(int index) => this.index = index;

    public int GetIndex() => index;

    public ReactiveProperty<int> SubscribeRevealedCardPoint() => revealedCardPoint;
    public int GetRevealedCardPoint() => revealedCardPoint.Value;
    public int GetCardCount() => cards.Count;
    public void SetOwner(Players owner) => this.owner = owner;

    public Players GetOwner() => this.owner;

    public List<CardDisplay> GetCardDisplays() => cards;
    public void AddCard(CardDisplay card)
    {
        card.transform.parent.SetParent(transform);

        cards.Add(card);
    }

    public void Rearrange()
    {
        DoRearrange().ToObservable().Subscribe();
    }

    IEnumerator DoRearrange()
    {
        var size = cards.Count;
        float s = size % 2 == 0 ? ((size - 1) / 2 + 0.5f) : ((size - 1) / 2);

        float halfSize = (size - 1) / 2f; // Center point
        float scale = 1f / (size / 2f);   // Scale factor
        scale = 1.2f;

        for (int i = 0; i < size; i++)
        {
            float value = (i - halfSize) * scale;
            cards[i].transform.parent.DORotate(new Vector3(0, 0, 0), 0f);
            //cards[i].transform.localPosition = new Vector3(value, i / 100, 0);
            cards[i].transform.parent.transform.DOLocalMove(new Vector3(value, i/100f, 0), 0.5f * (1 / GameController.Instance.GameSpeed)).SetEase(Ease.OutExpo);
            //yield return new WaitForSeconds(0.1f * (1 / GameController.Instance.GameSpeed));
        }

        yield return null;
    }

    public void CalculateRevealedCardPoint()
    {
        int point = 0;

        foreach (var c in cards.Where(x => x.IsRevealed()).Select(x => x.GetCard()).OrderByDescending(x => x.rank).ToList())
        {
            var cPoint = c.rank >= CardRank.Ten ? 10 : (int)c.rank;

            if (c.rank == CardRank.Ace && point + 11 <= 21)
            {
                cPoint = 11;
            }

            point += cPoint;
        }

        revealedCardPoint.Value = point;
    }

    public CardDisplay FlipNextCard()
    {
        if (owner != Players.Dealer) return null;

        foreach(var card in cards)
        {
            if (card.IsRevealed()) continue;

            card.FlipCard();
            return card;
        }

        
        return null;
    }

    public void ResetCardset()
    {

        foreach(var c in cards)
        {
            Destroy(c.transform.parent.gameObject);
        }

        cards.Clear();
        revealedCardPoint.Value = 0;

        Destroy(gameObject);
    }
}
