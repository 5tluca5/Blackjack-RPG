using GamConstant;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class BetZone : MonoBehaviour
{
    [SerializeField] GameObject chipPrefab;
    [SerializeField] Transform spawnPoint;

    List<ChipObject> chipObjects = new();

    ReactiveProperty<int> betValue = new(0);

    public ReactiveProperty<int> OnBetValueChanged() => betValue;

    public void AddChip(ChipType chipType)
    {
        var chip = Instantiate(chipPrefab, spawnPoint);
        var chipObject = chip.GetComponent<ChipObject>();
        chipObject.SetChipValue(chipType);
        chipObject.SetKinematic(false);
        chipObject.OnChipInteracted().Subscribe(x =>
        {
            chipObjects.Remove(chipObject);
            Destroy(chip);
            CalculateBetValue();

        }).AddTo(chip);

        chipObjects.Add(chipObject);

        CalculateBetValue();
    }

    public int BetPhaseEnded()
    {
        // TODO: Tidy up the chips

        CalculateBetValue();

        return betValue.Value;
    }

    void CalculateBetValue()
    {
        betValue.Value = chipObjects.Sum(x => x.ChipValue);
    }
}
