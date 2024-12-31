using GamConstant;
using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.Collections;

public class ChipZone : MonoBehaviour
{
    [SerializeField] GameObject chipPrefab;

    Players owner;
    PlayerZone playerZone;
    PlayerProfile playerProfile;

    List<ChipObject> chipObjects = new();

    private void Start()
    {
        
    }

    public void Setup(PlayerZone playerZone)
    {
        this.playerZone = playerZone;
        owner = playerZone.Owner;
        playerProfile = playerZone.GetPlayerProfile();
    }

    public void SetPlayerChipValue(int playerChips)
    {
        //for(ChipType chipType = ChipType.Chip_5000; chipType >= ChipType.Chip_5; chipType--)
        //{
        //    int chipValue = (int)chipType;

        //    while (playerChips >= chipValue)
        //    {
        //        playerChips -= chipValue;
        //        CreateChip(chipType);
        //    }
        //}
        Reset();

        for (ChipType chipType = ChipType.Chip_5; chipType <= ChipType.Chip_5000; chipType++)
        {
            int chipValue = (int)chipType;

            if (playerChips >= chipValue)
            {
                var chip = Instantiate(chipPrefab, transform);
                var chipObject = chip.GetComponent<ChipObject>();
                chipObject.SetChipValue(chipType);
                chipObject.SetKinematic(true);
                chipObject.OnChipInteracted().Subscribe(x =>
                {
                    GameController.Instance.PlayerAddChipToBetZone(chipType);
                }).AddTo(chip);

                chipObjects.Add(chipObject);
            }
            else
                break;
        }
    }

    public void BetPhaseStarted()
    {
        ShowChips().ToObservable().Subscribe(x =>
        {
            // Do nothing
        });
    }

    public void BetPhaseEnded()
    {
        HideChips().ToObservable().Subscribe(x =>
        {
            Reset();
        });
    }

    IEnumerator ShowChips()
    {
        float showDuration = 0.5f * (1 / GameController.Instance.GameSpeed);
        foreach (var chip in chipObjects)
        {
            chip.Show();
            yield return new WaitForSeconds(showDuration / chipObjects.Count);
        }
    }

    IEnumerator HideChips()
    {
        float hideDuration = 0.5f * (1 / GameController.Instance.GameSpeed);
        foreach (var chip in chipObjects)
        {
            chip.Hide();
            yield return new WaitForSeconds(hideDuration / chipObjects.Count);
        }
    }

    public void Reset()
    {
        foreach(var chip in chipObjects)
        {
            Destroy(chip.gameObject);
        }

        chipObjects.Clear();
    }
}
