using UnityEngine;
using UniRx;
using GamConstant;
using CardAttribute;

public class CardDisplay : MonoBehaviour, Interactable
{
    [SerializeField] Animator animator;
    [SerializeField] MeshRenderer meshRenderer;

    // Animation Keys
    protected const string ANI_KEY_Flip = "Flip";
    protected const string ANI_KEY_gameSpeed = "GameSpeed";

    Card card;
    Players owner;

    bool isRevealing = false;
    bool isRevealed = false;
    Subject<Card> onRevealed = new Subject<Card>();
    Subject<Card> onRevealCompleted = new Subject<Card>();

    private void Awake()
    {
        
    }

    private void Update()
    {
        animator.SetFloat(ANI_KEY_gameSpeed, GameController.Instance.GameSpeed);
    }

    public Card GetCard() => card;

    public Players GetOwner() => owner;
    public bool IsRevealed() => isRevealed;

    public CardRank GetRank() => card.rank;

    public void Setup(Players owner, Card card)
    {
        onRevealed?.Dispose();
        onRevealed = new Subject<Card>();

        onRevealCompleted?.Dispose();
        onRevealCompleted = new Subject<Card>();

        this.owner = owner;
        this.card = card;

        isRevealing = false;
        isRevealed = false;

        UpdateDisplay();
    }

    public void SetTag(string tag)
    {
        gameObject.tag = tag;
        meshRenderer.gameObject.tag = tag;
    }

    public Subject<Card> OnCardRevealed() => onRevealed;
    public Subject<Card> OnCardRevealCompleted() => onRevealCompleted;

    public void UpdateDisplay()
    {
        Material[] mats = meshRenderer.materials;

        // Assign a new material to index 1
        Material newMat = CardMaterialProvider.GetSuitMat(card.suit, card.rank);

        if (newMat != null)
        {
            mats[1] = newMat; // Update the material
            meshRenderer.materials = mats; // Reassign the modified array back to the renderer
        }
        else
        {
            Debug.LogError($"Material not found for suit: {card.suit}, rank: {card.rank}");
        }
    }

    public void CardRevealed()
    {
        isRevealed = true;
        onRevealed.OnNext(card);
    }

    public void CardRevealCompleted()
    {
        isRevealed = true;
        onRevealCompleted.OnNext(card);
    }

    public void FlipCard()
    {
        isRevealing = true;
        animator.SetTrigger(ANI_KEY_Flip);
    }

    public void PeakCard()
    {

    }

    public bool IsInteractable()
    {
        return GameController.Instance.IsPlayerTurn() && !isRevealed && !isRevealing;
    }

    public void Interact(KeyCode keyCode)
    {
        if (keyCode == KeyCode.Mouse0)
        {
            GameController.Instance.RevealCard(this, RevealCardType.Flip);
        }
        else if (keyCode == KeyCode.Mouse1)
        {
            GameController.Instance.RevealCard(this, RevealCardType.Peak);
        }
    }
}
