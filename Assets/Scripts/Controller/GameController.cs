using GamConstant;
using UnityEngine;
using UniRx;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("References")]
    [SerializeField] DeckController deckController;
    [SerializeField] CameraRaycast cameraRaycast;

    [Header("Parameters")]
    [SerializeField] GamePhase currentPhase;
    [SerializeField] public float GameSpeed { get; private set; } = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartNewTurn().ToObservable().Subscribe();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator StartNewTurn()
    {
        currentPhase = GamePhase.InitialDeal;

        deckController.InitDeck();
        deckController.ShuffleDeck(2f);

        yield return new WaitUntil(() => !deckController.IsShuffling());

        yield return new WaitForSeconds(1 * (1 / GameSpeed));

        deckController.InitialDealCard().ToObservable().Subscribe(x =>
        {
            Debug.Log("InitialDealCard completed");
            currentPhase = GamePhase.PlayerTurn;
        }).AddTo(this);

        yield return null;
    }

    public bool IsPlayerTurn() => currentPhase == GamePhase.PlayerTurn;
    public bool IsInitialDeal() => currentPhase == GamePhase.InitialDeal;

    public void ReceivedInput(KeyCode keyCode)
    {
        GameObject go = cameraRaycast.GetRaycastedObject();

        if (go != null && go.TryGetComponent(out Interactable interactable))
        {
            interactable.Interact(keyCode);
        }
    }

    public void DealCard()
    {
        deckController.DealCard(Players.Player1);
    }

    public void RevealCard(CardDisplay cardDisplay, RevealCardType type)
    {
        if (currentPhase == GamePhase.PlayerTurn)
        {
            if (type == RevealCardType.Flip)
            {
                cardDisplay.FlipCard();
            }
            else
            {
                cardDisplay.PeakCard();
            }
        }
    }

}
