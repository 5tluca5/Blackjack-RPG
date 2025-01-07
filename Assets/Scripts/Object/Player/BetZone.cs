using GamConstant;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using System;
using Unity.VisualScripting;

public class BetZone : MonoBehaviour, Interactable
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

    //public void AddChip(ChipType chipType = ChipType.Chip_5)
    //{
    //    // Define the range for random offset
    //    float randomX = Random.Range(-0.1f, 0.1f); // Random X offset between -1 and 1
    //    float randomY = 0;
    //    float randomZ = Random.Range(-0.1f, 0.1f); // Random Z offset between -1 and 1

    //    // Combine into a Vector3 offset
    //    Vector3 randomOffset = new Vector3(randomX, randomY, randomZ);

    //    // Apply the random offset to the spawnPoint position
    //    Vector3 spawnPosition = spawnPoint.position + randomOffset;

    //    var chip = Instantiate(chipPrefab, transform);
    //    chip.transform.position = spawnPosition;

    //    var chipObject = chip.GetComponent<ChipObject>();
    //    chipObject.SetChipValue(chipType);
    //    chipObject.SetKinematic(false);
    //    chipObject.OnChipInteracted().Subscribe(x =>
    //    {
    //        chipObjects.Remove(chipObject);
    //        Destroy(chip);
    //        CalculateBetValue();

    //    }).AddTo(chip);

    //    chipObjects.Add(chipObject);

    //    CalculateBetValue();
    //}

    public void AddChip(ChipType chipType = ChipType.Chip_5)
    {
        
        var chip = Instantiate(chipPrefab, spawnPoint);
        var spawnPosition = Vector3.zero;
        spawnPosition.y = chipObjects.Count * 0.05f;

        chip.transform.localPosition = spawnPosition;

        var chipObject = chip.GetComponent<ChipObject>();
        chipObject.SetChipValue(chipType);
        chipObject.SetKinematic(true);
        chipObject.OnChipInteracted().Subscribe(x =>
        {
            chipObjects.Remove(chipObject);
            Destroy(chip);
            CalculateBetValue();
            SetChipsInteractable();
        }).AddTo(chip);
        chipObject.Placed();

        chipObjects.Add(chipObject);

        CalculateBetValue();
        SetChipsInteractable();
    }

    public int SetToMinBet()
    {
        var minBet = RuleController.Instance.MinBet;
        var chipTypes = GameController.Instance.GetChipZone().GetChipTypes().OrderByDescending(x => x).ToList();
        
        foreach (var chip in chipObjects) {
            Destroy(chip.gameObject);
        }

        chipObjects.Clear();

        foreach (var chip in chipTypes)
        {
            while (minBet >= (int)chip)
            {
                AddChip(chip);
                minBet -= (int)chip;
            }
        }

        return BetValue;
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

    void SetChipsInteractable()
    {
       for(int i=0; i<chipObjects.Count; i++)
        {
            chipObjects[i].SetInteractable(i == chipObjects.Count - 1);
        }
    }

    public bool IsInteractable()
    {
        return GameController.Instance.IsBetPhase();
    }

    public void Interact(KeyCode keyCode)
    {
        if(keyCode == KeyCode.Mouse0)
        {
            GameController.Instance.PlayerAddChipToBetZone();
        }
    }
}
