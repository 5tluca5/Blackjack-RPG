using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameRule", menuName = "Scriptable Objects/GameRule")]

public class GameRule : ScriptableObject
{
    [Header("Basic rules")]
    [SerializeField, Min(0)] public int minBet;
    [SerializeField, Min(0)] public int maxBet;
    [SerializeField, Min(1)] public int numberOfDecks = 1;
    [SerializeField] public int bustPoint = 21;
    [SerializeField] public int blackJackPoint = 21;
    [SerializeField] public int dealerMinPoint = 17;
    [SerializeField, Min(1)] public int maxSplits = 2;
    [SerializeField] public bool allowSurrender = true;
    [SerializeField] public bool allowSplitting = true;
    [SerializeField] public bool allowDoubleDown = true;

    [Header("Payout")]
    [SerializeField, Min(0)] public float blackjackPayout = 1.5f;
    [SerializeField, Min(1.0f)] public float insurancePayout = 2.0f;

    [Header("Others")]
    [SerializeField, Min(5)] public float betPhaseTimeLimit = 30.0f;
    [SerializeField, Min(5)] public float decisionTimeLimit = 30.0f;


    public bool IsBusted(int playerPoint)
    {
        return playerPoint > bustPoint;
    }

    public bool IsBlackJack(int playerPoint)
    {
        return playerPoint == blackJackPoint;
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