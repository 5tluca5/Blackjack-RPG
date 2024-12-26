using GamConstant;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScoreboardItem : MonoBehaviour
{
    [SerializeField] Players owner;
    [SerializeField] Image playerIcon;
    [SerializeField] TMPro.TextMeshProUGUI pointText;

    int point;

    public void Setup(Players owner, Sprite icon, int point)
    {
        this.owner = owner;
        this.point = point;
        playerIcon.sprite = icon;
        pointText.text = point.ToString();
    }

    public void SetPlayerPoint(int point)
    {
        DOVirtual.Int(this.point, point, 0.5f, (x) => pointText.text = x.ToString()).SetEase(Ease.OutExpo);

        //this.point = point;
        //pointText.text = point.ToString();
    }
}
