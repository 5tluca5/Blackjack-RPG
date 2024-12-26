using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UniRx;
using GamConstant;

public class DeckObject : MonoBehaviour, Interactable
{
    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] Transform deckTF;
    [SerializeField] GameObject frontCard;
    List<CardDisplay> dealedCards = new();

    // Animation Keys
    protected const string ANI_KEY_ShuffleTime = "ShuffleTime";
    protected const string ANI_KEY_gameSpeed = "GameSpeed";

    // Timer
    float shuffleTimer = 0f;

    // Deck size factor
    float deckHeight = 1f;

    public bool IsShuffling() => shuffleTimer > 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        deckTF = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(shuffleTimer > 0)
        {
            shuffleTimer = Mathf.Max(0, shuffleTimer - Time.deltaTime);
            animator.SetFloat(ANI_KEY_ShuffleTime, shuffleTimer);
        }

        animator.SetFloat(ANI_KEY_gameSpeed, GameController.Instance.GameSpeed);
    }

    public void PlayShuffleAnimation(float shuffleTime)
    {
        shuffleTimer = shuffleTime;
    }

    public void SetDeckOriginalHeight(int height)
    {
        deckHeight = height;

        var curScale = deckTF.localScale;
        curScale.z = height;

        deckTF.localScale = curScale;
    }

    public void SetDeckSize(float size)
    {
        if (size == 0) return;

        var curScale = deckTF.localScale;
        curScale.z = deckHeight * size;

        deckTF.localScale = curScale;
    }

    public CardDisplay DealCard(Players target, Card card, Transform refTF)
    {
        var c = Instantiate(frontCard, transform);
        var cd = c.GetComponentInChildren<CardDisplay>();

        cd.Setup(target, card);

        if(GameController.Instance.GetCurrentPlayer() == target)
        {
            cd.SetTag("PlayerCard");
        }

        c.transform.DOMove(refTF.position, 1f * (1 / GameController.Instance.GameSpeed)).SetEase(Ease.OutExpo);
        c.transform.DORotate(refTF.localRotation.eulerAngles, 1f* (1 / GameController.Instance.GameSpeed)).SetEase(Ease.OutExpo);

        dealedCards.Add(cd);

        return cd;
    }

    public bool IsInteractable()
    {
#if UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }

    public void Interact(KeyCode keyCode)
    {
        if (!IsInteractable()) return;

        if (keyCode == KeyCode.E)
        {
            //GameController.Instance.DealCard();
        }
    }
}
