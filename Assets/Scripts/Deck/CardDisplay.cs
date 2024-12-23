using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;

    Card card;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public Card GetCard() => card;

    public void Setup(Card card)
    {
        this.card = card;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        meshRenderer.materials[1] = CardMaterialProvider.GetSuitMat(card.suit, card.rank);
    }
}
