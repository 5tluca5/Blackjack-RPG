using GamConstant;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using System.Drawing;

public class ScoreboardItem : MonoBehaviour
{
    [SerializeField] Players owner;
    [SerializeField] Image playerIcon;
    [SerializeField] TMPro.TextMeshProUGUI playerNameText;
    [SerializeField] TMPro.TextMeshProUGUI playerLevelText;
    [SerializeField] TMPro.TextMeshProUGUI totalChipText;
    [SerializeField] TMPro.TextMeshProUGUI handCardPointText;

    int handCardPoint;
    int targetPoint;
    int curTotalChip;
    PlayerProfile playerProfile;
    Tween tween;

    public void Setup(Players owner, PlayerProfile playerProfile)
    {
        playerProfile.OnPlayerChipUpdated().Subscribe(x =>
        {
            DOVirtual.Int(curTotalChip, x, 1f, (x) =>
            {
                totalChipText.text = x.ToString();
                curTotalChip = x;
            })
            .SetEase(Ease.OutExpo);
        }).AddTo(this);

        this.owner = owner;
        this.playerProfile = playerProfile;
        curTotalChip = playerProfile.Chips;
        handCardPoint = 0;
        targetPoint = 0;

        playerIcon.sprite = playerProfile.PlayerIcon;
        playerNameText.text = playerProfile.PlayerName;
        playerLevelText.text = $" <size=18>Lv.</size>{playerProfile.PlayerLevel}";

        handCardPointText.text = handCardPoint.ToString();

        playerProfile.Refresh();
    }

    public void SetPlayerPoint(int point)
    {
        if(point == handCardPoint) return;

        targetPoint = point;

        DOVirtual.Int(handCardPoint, targetPoint, 0.5f,
            (x) =>
            {
                handCardPointText.text = x.ToString();
                handCardPoint = x; // Update handCardPoint here
            })
            .SetEase(Ease.OutExpo);
    }

    public Players Owner => owner;
}
