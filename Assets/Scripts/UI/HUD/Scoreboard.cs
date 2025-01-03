using GamConstant;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] GameObject ScoreboardItemPrefab;
    [SerializeField] List<Sprite> playersIcon;

    Dictionary<Players, ScoreboardItem> scoreboardItems = new();

    private void Awake()
    {
        
    }
    
    private void Start()
    {
        
    }
    
    public void Setup(Dictionary<Players, PlayerProfile> playerProfiles)
    {
        playerProfiles = playerProfiles.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

        foreach (var playerProfile in playerProfiles)
        {
            var scoreboardItem = Instantiate(ScoreboardItemPrefab, transform).GetComponent<ScoreboardItem>();
            scoreboardItem.Setup(playerProfile.Key, playerProfile.Value);
            scoreboardItems.Add(playerProfile.Key, scoreboardItem);
        }
    }

    public void UpdatePoint(Players player, int point)
    {
        if (!scoreboardItems.ContainsKey(player)) return;

        scoreboardItems[player].SetPlayerPoint(point);
    }

    public void Reset()
    {
        
    }
}
