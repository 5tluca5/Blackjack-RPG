using UnityEngine;

public class Deck : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject cardPrefab;
    [SerializeField, Range(1, 10)] int deckSize = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("InitDeck")]
    public void InitDeck()
    {
        for (int i = 0; i < 52 * deckSize; i++)
        {

            var cardGO = Instantiate(cardPrefab, transform);
            cardGO.transform.localPosition = Vector3.zero - new Vector3 (0, 0, 0.005f * i);
        }
    }
}
