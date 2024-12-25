using System;
using UnityEngine;

namespace GamConstant
{
    public enum GameMode
    {
        SinglePlayer,
        MultiPlayer
    }

    public enum Players
    {
        Dealer,
        Player1,
        Player2,
        Player3
    }

    [Serializable]
    public enum GamePhase
    {
        InitialDeal,
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
}