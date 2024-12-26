using GamConstant;
using UnityEngine;
using UnityEngine.UI;

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
        this.point = point;
        pointText.text = point.ToString();
    }
}
