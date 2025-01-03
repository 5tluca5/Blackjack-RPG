using System;
using UnityEngine;

namespace GamConstant
{
    public enum GameMode
    {
        SinglePlayer,
        MultiPlayer
    }

    public enum Players : int
    {
        Dealer = 0,
        Player1,
        Player2,
        Player3,
        NullPlayer,
    }

    [Serializable]
    public enum GamePhase
    {
        ShuffleDeck,
        PlayerBet,
        DealingCards,
        PlayerTurn,
        DealerTurn,
        SettlementPhase,
        NextRound,
    }

    public enum RevealCardType
    {
        Flip,
        Peak
    }

    public enum ChipType : int
    {
        Chip_5 = 5,
        Chip_25 = 25,
        Chip_100 = 100,
        Chip_500 = 500,
        Chip_1000 = 1000,
        Chip_5000 = 5000
    }

}