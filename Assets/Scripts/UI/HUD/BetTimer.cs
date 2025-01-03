using TMPro;
using UnityEngine;
using DG.Tweening;

public class BetTimer : MonoBehaviour
{
    [SerializeField] Transform timerTF;
    [SerializeField] TextMeshProUGUI timeText;

    float initialTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timerTF.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(float initialTime, float duration)
    {
        this.initialTime = initialTime;

        timerTF.gameObject.SetActive(true); 
        var initialPos = timerTF.localPosition;
        timerTF.DOLocalMoveY(initialPos.y + 200f, duration).From().SetEase(Ease.OutBack);
    }

    public void UpdateTimer(float time)
    {
        timeText.text = Mathf.CeilToInt(time).ToString();

        var color = timeText.color;
        color.g = 255 * (time / initialTime);
        timeText.color = color;
    }

    public void Hide(float duration)
    {
        timerTF.DOLocalMoveY(200f, duration).SetEase(Ease.InBack).OnComplete(() => timerTF.gameObject.SetActive(false));
    }
}
