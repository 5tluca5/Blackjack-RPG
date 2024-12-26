using GamConstant;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] GameObject ScoreboardItemPrefab;
    [SerializeField] List<Sprite> playersIcon;

    Dictionary<Players, ScoreboardItem> scoreboardItems = new();

    private void Awake()
    {
        // Create ScoreboardItem for each player
        for(Players player = Players.Dealer; player <= Players.Player1; player++)
        //foreach (Players player in System.Enum.GetValues(typeof(Players)))
        {
            var scoreboardItem = Instantiate(ScoreboardItemPrefab, transform).GetComponent<ScoreboardItem>();
            scoreboardItem.Setup(player, playersIcon[(int)player], 0);
            scoreboardItems.Add(player, scoreboardItem);
        }
    }

    private void Start()
    {
        
    }

    public void UpdatePoint(Players player, int point)
    {
        if (!scoreboardItems.ContainsKey(player)) return;

        scoreboardItems[player].SetPlayerPoint(point);
    }
}
