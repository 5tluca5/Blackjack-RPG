using GamConstant;
using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.Collections;
using System;
using DG.Tweening;


//public class ChipZone : MonoBehaviour
//{
//    [SerializeField] GameObject chipPrefab;

//    Players owner;
//    PlayerZone playerZone;
//    PlayerProfile playerProfile;

//    List<ChipObject> chipObjects = new();

//    private void Start()
//    {

//    }

//    public void Setup(PlayerZone playerZone)
//    {
//        this.playerZone = playerZone;
//        owner = playerZone.Owner;
//        playerProfile = playerZone.GetPlayerProfile();
//    }

//    public void SetPlayerChipValue(int playerChips)
//    {
//        //for(ChipType chipType = ChipType.Chip_5000; chipType >= ChipType.Chip_5; chipType--)
//        //{
//        //    int chipValue = (int)chipType;

//        //    while (playerChips >= chipValue)
//        //    {
//        //        playerChips -= chipValue;
//        //        CreateChip(chipType);
//        //    }
//        //}
//        Reset();

//        foreach(ChipType chipType in Enum.GetValues(typeof(ChipType)))
//        {
//            int chipValue = (int)chipType;

//            if (playerChips >= chipValue)
//            {
//                var chip = Instantiate(chipPrefab, transform);
//                var chipObject = chip.GetComponent<ChipObject>();
//                chip.SetActive(false);
//                chipObject.SetChipValue(chipType);
//                chipObject.SetKinematic(true);
//                chipObject.OnChipInteracted().Subscribe(x =>
//                {
//                    chipObject.Hit();
//                    GameController.Instance.PlayerAddChipToBetZone(chipType);
//                }).AddTo(chip);

//                chipObjects.Add(chipObject);
//            }
//            else
//                break;
//        }

//        //for (ChipType chipType = ChipType.Chip_5; chipType <= ChipType.Chip_5000; chipType++)
//        //{
//        //    int chipValue = (int)chipType;

//        //    if (playerChips >= chipValue)
//        //    {
//        //        var chip = Instantiate(chipPrefab, transform);
//        //        var chipObject = chip.GetComponent<ChipObject>();
//        //        chipObject.SetChipValue(chipType);
//        //        chipObject.SetKinematic(true);
//        //        chipObject.OnChipInteracted().Subscribe(x =>
//        //        {
//        //            GameController.Instance.PlayerAddChipToBetZone(chipType);
//        //        }).AddTo(chip);

//        //        chipObjects.Add(chipObject);
//        //    }
//        //    else
//        //        break;
//        //}
//    }

//    public void BetPhaseStarted()
//    {
//        ShowChips().ToObservable().Subscribe(x =>
//        {
//            // Do nothing
//        });
//    }

//    public void BetPhaseEnded()
//    {
//        HideChips().ToObservable().Subscribe(x =>
//        {
//            Reset();
//        });
//    }

//    IEnumerator ShowChips()
//    {
//        float showDuration = 1f * (1 / GameController.Instance.GameSpeed);

//        foreach (var chip in chipObjects)
//        {
//            chip.Show();
//            yield return new WaitForSeconds(showDuration / chipObjects.Count);
//        }
//    }

//    IEnumerator HideChips()
//    {
//        float hideDuration = 1f * (1 / GameController.Instance.GameSpeed);
//        foreach (var chip in chipObjects)
//        {
//            chip.Hide();
//            yield return new WaitForSeconds(hideDuration / chipObjects.Count);
//        }
//    }

//    public void Reset()
//    {
//        foreach(var chip in chipObjects)
//        {
//            Destroy(chip.gameObject);
//        }

//        chipObjects.Clear();
//    }
//}

public class ChipZone : MonoBehaviour
{
    [SerializeField] ChipObject selectingChip;
    [SerializeField] Transform chipSelectTF;

    List<ChipType> chipTypes = new();
    ReactiveProperty<int> curChipIndex = new(0);

    bool isBetPhase = false;
    Vector3 initialPos;

    private void Start()
    {
        initialPos = chipSelectTF.transform.localPosition;
        curChipIndex.Subscribe(x =>
        {
            if (x >= chipTypes.Count || x < 0) return;

            selectingChip.SetChipValue(chipTypes[x]);
            selectingChip.Hit();
        }).AddTo(this);

        chipSelectTF.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(!isBetPhase) return;

        HandleInput();
    }

    public void SetPlayerChipValue(int playerChips)
    {
        chipTypes.Clear();

        foreach (ChipType chipType in Enum.GetValues(typeof(ChipType)))
        {
            int chipValue = (int)chipType;

            if (playerChips >= chipValue)
            {
                chipTypes.Add(chipType);
            }
            else
                break;
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            curChipIndex.Value = curChipIndex.Value - 1 < 0 ? chipTypes.Count - 1 : curChipIndex.Value - 1;
        } else if(Input.GetKeyDown(KeyCode.E))
        {
            curChipIndex.Value = curChipIndex.Value + 1 >= chipTypes.Count ? 0 : curChipIndex.Value + 1;
        }
    }

    public void BetPhaseStarted()
    {
        isBetPhase = true;
        chipSelectTF.gameObject.SetActive(true);

        chipSelectTF.transform.localPosition = initialPos;
        chipSelectTF.DOLocalMoveY(initialPos.y - 200, 0.5f).From().SetEase(Ease.OutBack);

    }

    public void BetPhaseEnded()
    {
        isBetPhase = false;

        chipSelectTF.DOLocalMoveY(-200f, 0.5f).SetEase(Ease.InBack).OnComplete(() => chipSelectTF.gameObject.SetActive(false));

    }

    public ChipType GetSelectingChipType() => curChipIndex.Value >= chipTypes.Count ? ChipType.Chip_5 : chipTypes[curChipIndex.Value];
    public List<ChipType> GetChipTypes() => chipTypes;
}