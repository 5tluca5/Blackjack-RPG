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
    public int BetValue => betValue.Value;

    private void Start()
    {
            
    }

    public void AddChip(ChipType chipType = ChipType.Chip_5)
    {
        // Define the range for random offset
        float randomX = Random.Range(-0.1f, 0.1f); // Random X offset between -1 and 1
        float randomY = 0;
        float randomZ = Random.Range(-0.1f, 0.1f); // Random Z offset between -1 and 1

        // Combine into a Vector3 offset
        Vector3 randomOffset = new Vector3(randomX, randomY, randomZ);

        // Apply the random offset to the spawnPoint position
        Vector3 spawnPosition = spawnPoint.position + randomOffset;

        var chip = Instantiate(chipPrefab, transform);
        chip.transform.position = spawnPosition;

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
