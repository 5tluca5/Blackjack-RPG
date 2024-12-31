using GamConstant;
using UnityEngine;
using System.Collections.Generic;
using UniRx;
using System.Linq;

public class PlayerZone : MonoBehaviour
{
    [SerializeField] Players owner;
    [SerializeField] PlayerProfile playerProfile;

    [Header("References")]
    [SerializeField] CardZone cardZone;
    [SerializeField] ChipZone chipZone;
    [SerializeField] BetZone betZone;
    [SerializeField] List<Transform> cardPosRefs;

    [Header("Parameters")]
    [SerializeField] bool betPlaced = false;
    [SerializeField] int curCardsetIndex;

    private void Start()
    {
        cardZone = GetComponentInChildren<CardZone>();
        chipZone = GetComponentInChildren<ChipZone>();

        chipZone.Setup(this);
    }

    public Players Owner => owner;
    public bool IsBothCardRevealed() => cardZone.IsBothCardRevealed();
    public bool IsBetPlaced() => betPlaced;

    public CardSet GetCardSet(int cardSetIndex) => cardZone.GetCardSet(cardSetIndex);

    public CardSet GetCurrentCardSet() => cardZone.GetCurrentCardSet();
    
    public List<Transform> GetCardPosRefs() => cardPosRefs;
    public PlayerProfile GetPlayerProfile() => playerProfile;

    public CardSet AddCard(CardDisplay card)
    {
        card.OnCardRevealed().Subscribe(x =>
        {
            GameController.Instance.UpdatePlayerCardSetPoint(owner, curCardsetIndex);
        }).AddTo(card);

        card.OnCardRevealCompleted().Subscribe(x =>
        {
            var cardSet = GetCurrentCardSet();

            if (cardSet.GetCardDisplays().Where(c => c.IsRevealed()).Count() >= 2)
            {
                cardSet.Rearrange();
            }
        }).AddTo(card);

        return cardZone.AddCard(card, curCardsetIndex);
    }
    
    public void StartPlacingBet()
    {
        chipZone.SetPlayerChipValue(playerProfile.Chips);
        chipZone.BetPhaseStarted();
    }

    public void PlacedBet()
    {
        betPlaced = true;
    }

    public void Reset()
    {
        cardZone.ResetCardZone();
    }

    public bool AddChipToBetZone(ChipType chipType)
    {
        if (playerProfile.Chips < (int)chipType) return false;

        betZone.AddChip(chipType);
        return true;
    }
}
