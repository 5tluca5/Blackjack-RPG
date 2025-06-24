using CardAttribute;
using System.Collections.Generic;
using UnityEngine;

public class CardMaterialProvider : MonoBehaviour
{
    [SerializeField] List<Material> spadesMat;
    [SerializeField] List<Material> heartsMat;
    [SerializeField] List<Material> clubsMat;
    [SerializeField] List<Material> diamondsMat;

    private static Dictionary<CardSuit, Dictionary<CardRank, Material>> suitMatDict;

    private void Awake()
    {
        Initialize();
        DontDestroyOnLoad(this);
    }

    public void Initialize()
    {
        suitMatDict = new();

        Dictionary<CardSuit, List<Material>> mats = new Dictionary<CardSuit, List<Material>> {
            { CardSuit.Spade, spadesMat },
            { CardSuit.Heart, heartsMat },
            { CardSuit.Club, clubsMat },
            { CardSuit.Diamond, diamondsMat }
        };

        for(CardSuit s = CardSuit.Spade; s <= CardSuit.Diamond; s++)
        {
            var dict = new Dictionary<CardRank, Material>();

            for (int i = 1; i <= mats[s].Count; i++)
            {
                dict[(CardRank)i] = mats[s][i-1];
            }

            suitMatDict[s] = dict;
        }
    }

    public static Material GetSuitMat(CardSuit suit, CardRank rank)
    {
        if (suitMatDict != null && suitMatDict.ContainsKey(suit) && suitMatDict[suit].ContainsKey(rank))
        {
            return suitMatDict[suit][rank];
        }

        return null;
    }
}
