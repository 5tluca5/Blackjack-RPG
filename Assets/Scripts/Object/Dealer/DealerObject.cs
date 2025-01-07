using GamConstant;
using System;
using System.Collections;
using UniRx;
using UnityEngine;

public class DealerObject : MonoBehaviour, Interactable
{
    [Header("References")]
    [SerializeField] PlayerZone playerZone;
    [SerializeField] InteractableOutline interactableOutline;

    bool executingDealerPhase = false;
    IDisposable dealerActionPhase = null;
    public void Interact(KeyCode keyCode)
    {
        if (GameController.Instance.IsBetPhase())
        {
            if(keyCode == KeyCode.Space)
            {
                GameController.Instance.PlayerBetConfirmed();
            }
        }
        else if (GameController.Instance.IsPlayerTurn())
        {
            if (keyCode == KeyCode.Space)
            {
                GameController.Instance.PlayerHit();
            }
            else if (keyCode == KeyCode.E)
            {
                GameController.Instance.PlayerStand();
            }
            else if (keyCode == KeyCode.D)
            {
                GameController.Instance.PlayerDoubleDown();
            }
            else if (keyCode == KeyCode.S)
            {
                GameController.Instance.PlayerSplit();
            }
            else if (keyCode == KeyCode.Q)
            {
                GameController.Instance.PlayerSurrender();
            }
        }
        else if(GameController.Instance.IsSettlementPhase())
        {
            if (keyCode == KeyCode.Space)
            {
                GameController.Instance.PlayerGoToNextRound();
            }
        }
    }

    public bool IsInteractable()
    {
        return GameController.Instance.IsDealerInteractable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (interactableOutline == null)
            interactableOutline = TryGetComponent<InteractableOutline>(out var io) ? io : GetComponentInChildren<InteractableOutline>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Subject<bool> ExecuteDealerActionPhase()
    {
        if (executingDealerPhase) return null;

        var onDealerActionPhaseCompleted = new Subject<bool>();

        executingDealerPhase = true;
        var currentCardSet = playerZone.GetCurrentCardSet();

        dealerActionPhase = currentCardSet.SubscribeRevealedCardPoint().Subscribe(x =>
        {
            if (!executingDealerPhase) return;

            if(x < 17)
            {
                Invoke("FlipSecondCard", 0.5f * (1 / GameController.Instance.GameSpeed));
            }
            else
            {
                executingDealerPhase = false;
                dealerActionPhase?.Dispose();
                onDealerActionPhaseCompleted.OnNext(true);
            }

        }).AddTo(currentCardSet);

        //DoDealerActionPhase().ToObservable().Subscribe(_ =>
        //{
        //    executingDealerPhase = false;
        //    onDealerActionPhaseCompleted.OnNext(true);
        //});

        return onDealerActionPhaseCompleted;
    }

    IEnumerator DoDealerActionPhase()
    {
        var currentCardSet = playerZone.GetCurrentCardSet();
        var point = currentCardSet.SubscribeRevealedCardPoint().Value;

        while (point < 17)
        {
            var cd = currentCardSet.FlipNextCard();
            yield return cd.OnCardRevealCompleted().AsObservable()
            .Do(_ => point = currentCardSet.SubscribeRevealedCardPoint().Value)
            .ToYieldInstruction();
            point = currentCardSet.SubscribeRevealedCardPoint().Value;
            yield return new WaitForSeconds(0.5f);
        }



        dealerActionPhase = currentCardSet.SubscribeRevealedCardPoint().Subscribe(x =>
        {


        }).AddTo(currentCardSet);

        yield return null;
    }
    void FlipSecondCard()
    {
        var currentCardSet = playerZone.GetCurrentCardSet();
        var cd = currentCardSet.FlipNextCard();

        if(cd == null)
        {
            GameController.Instance.DealCard(Players.Dealer);
        }
    }

    public void SetInteractable(bool set)
    {
        interactableOutline.SetInteractable(set);
    }
}
