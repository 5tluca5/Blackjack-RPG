using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class DeckObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] Transform deckTF;
    [SerializeField] GameObject frontCard;
    List<CardDisplay> dealedCards = new();

    // Animation Keys
    protected const string ANI_KEY_ShuffleTime = "ShuffleTime";

    // Timer
    float shuffleTimer = 0f;

    // Deck size factor
    float deckHeight = 1f;

    
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
            shuffleTimer = Mathf.Max(0, shuffleTimer - Time.deltaTime);

        animator.SetFloat(ANI_KEY_ShuffleTime, shuffleTimer);
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

    public void DealCard(/*Target target, */Card card, Transform refTF)
    {
        var cd = Instantiate(frontCard, transform).GetComponent<CardDisplay>();
        cd.Setup(card);

        //if(target == Target.Player)
        cd.gameObject.tag = "PlayerCard";

        cd.transform.DOMove(refTF.position, 1f).SetEase(Ease.OutExpo);
        cd.transform.DORotate(refTF.localRotation.eulerAngles, 1f);

        dealedCards.Add(cd);
    }
}
