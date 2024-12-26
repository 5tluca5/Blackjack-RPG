using System.Collections.Generic;
using UnityEngine;

public class CardZone : MonoBehaviour
{
    CardSet currentCardSet;

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
        if (currentCardSet == null)
        {
            var go = new GameObject("CardSet");
            currentCardSet = go.AddComponent<CardSet>();
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
        }

        currentCardSet.AddCard(card);
    }
    public void AddCards(List<CardDisplay> cards)
    {
        if (currentCardSet == null)
        {
            var go = new GameObject("CardSet");
            currentCardSet = go.AddComponent<CardSet>();
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
        }

        currentCardSet.AddCards(cards);
    }
}
