using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;
using UniRx;

public class CardSet : MonoBehaviour
{
    List<CardDisplay> cards = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCard(CardDisplay card)
    {
        card.transform.parent.SetParent(transform);

        cards.Add(card);

        Rearrange().ToObservable().Subscribe();
    }

    public void AddCards(List<CardDisplay> cards)
    {
        foreach (var card in cards)
        {
            this.cards.Add(card);
            card.transform.parent.SetParent(transform);
        }

        Rearrange().ToObservable().Subscribe();
    }

    IEnumerator Rearrange()
    {
        var size = cards.Count;
        float s = size % 2 == 0 ? ((size - 1) / 2 + 0.5f) : ((size - 1) / 2);

        float halfSize = (size - 1) / 2f; // Center point
        float scale = 1f / (size / 2f);   // Scale factor
        scale = 0.5f;

        for (int i = 0; i < size; i++)
        {
            float value = (i - halfSize) * scale;
            //cards[i].transform.localPosition = new Vector3(value, i / 100, 0);
            cards[i].transform.parent.transform.DOLocalMove(new Vector3(value, i/100, 0), 0.1f * (1 / GameController.Instance.GameSpeed));
            //yield return new WaitForSeconds(0.1f * (1 / GameController.Instance.GameSpeed));
        }

        yield return null;
    }

}
