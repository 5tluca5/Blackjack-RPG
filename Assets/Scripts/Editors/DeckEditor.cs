using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DeckController))]
public class DeckEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Get the target Deck instance
        DeckController deck = (DeckController)target;

        // Get the list of cards
        List<Card> cardList = deck.GetCardList();

        // Add a button to Init the deck
        if (GUILayout.Button("Initialize Deck"))
        {
            deck.InitDeck();
        }

        // Add a button to shuffle the deck
        if (GUILayout.Button("Shuffle Deck"))
        {
            deck.ShuffleDeck(2);
        }

        // Deal a card to player
        if (GUILayout.Button("Deal Card"))
        {
            deck.DealCard(GamConstant.Players.Player1);
        }

        // Display the cards in the editor
        if (cardList != null && cardList.Count > 0)
        {
            EditorGUILayout.LabelField("Card List:", EditorStyles.boldLabel);
            string[] suits = { "♠︎", "♥︎", "♣︎", "♦︎" };
            for (int i = 0; i < cardList.Count; i++)
            {
                Card card = cardList[i];
                if (card != null)
                {
                    EditorGUILayout.LabelField($"[{i}]  {suits[(int)card.suit]}{card.GetRank()}");
                }
                else
                {
                    EditorGUILayout.LabelField($"[{i}]  , Card: null");
                }
            }
        }
        else
        {
            EditorGUILayout.LabelField("No cards in the list.");
        }
    }
}
