using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameRule", menuName = "Scriptable Objects/GameRule")]

public class GameRule : ScriptableObject
{
    [Header("Basic rules")]
    [SerializeField, Min(0)] int minBet;
    [SerializeField, Min(0)] int maxBet;
    [SerializeField, Min(1)] int numberOfDecks = 1;
    [SerializeField] int maxPoint = 21;
    [SerializeField] int blackJackPoint = 21;
    [SerializeField] int dealerMinPoint = 17;
    [SerializeField, Min(1)] int maxSplits = 2;
    [SerializeField] bool allowSurrender = true;
    [SerializeField] bool allowSplitting = true;

    [Header("Payout")]
    [SerializeField, Min(0)] float blackjackPayout = 1.5f;
    [SerializeField, Min(1.0f)] float insurancePayout = 2.0f;

    [Header("Others")]
    [SerializeField, Min(5)] float betPhaseTimeLimit = 30.0f;
    [SerializeField, Min(5)] float decisionTimeLimit = 30.0f;


    

    public bool IsBusted(int playerPoint)
    {
        return playerPoint > maxPoint;
    }

}

namespace CustomRule
{
    [Serializable]
    public class CustomRules
    {

    }

    [Serializable]
    public class PointPlusOne : CustomRules
    {

    }
}