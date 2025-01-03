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
    [SerializeField] int betValue;

    private void Start()
    {
        cardZone = GetComponentInChildren<CardZone>();
        chipZone = GetComponentInChildren<ChipZone>();

        chipZone.Setup(this);
    }

    public Players Owner => owner;
    public bool IsBothCardRevealed() => cardZone.IsBothCardRevealed();
    public bool IsBetPlaced() => betPlaced;
    public int GetBetValue() => betValue;
    public int GetTotalPoint() => GetCurrentCardSet().GetRevealedCardPoint();
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

    public void ConfirmBet()
    {
        betPlaced = true;
    }

    public void EndPlacingBet()
    {
        chipZone.BetPhaseEnded();
        betValue = betZone.BetPhaseEnded();

        playerProfile.AddChips(-betValue);
    }

    public void Reset()
    {
        betPlaced = false;
        cardZone.ResetCardZone();
        playerProfile.Refresh();
    }

    public bool AddChipToBetZone(ChipType chipType)
    {
        if (playerProfile.Chips < (int)chipType) return false;

        betZone.AddChip(chipType);
        return true;
    }

    public bool IsBusted()
    {
        int point = GetCurrentCardSet().GetRevealedCardPoint();

        return point > 21;
    }

    public bool IsBlackJack()
    {
        int point = GetCurrentCardSet().GetRevealedCardPoint();

        return point == 21 && GetCurrentCardSet().GetCardCount() == 2;
    }
}
