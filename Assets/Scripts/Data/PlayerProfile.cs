using UnityEngine;
using UniRx;
using GamConstant;

[CreateAssetMenu(fileName = "PlayerProfile", menuName = "Scriptable Objects/PlayerProfile")]
public class PlayerProfile : ScriptableObject
{
    [SerializeField] private Players owner;
    [SerializeField] private string playerName;
    [SerializeField] private Sprite playerIcon;
    [SerializeField] private int playerLevel;
    [SerializeField] private int playerExp;
    [SerializeField] private int playerChips;

    private ReactiveProperty<int> playerChipRP = new(0);
    public Players Owner => owner;
    public int Chips => playerChips;
    public string PlayerName => playerName;
    public string LogPlayerName => playerName.LogPlayerName();

    public Sprite PlayerIcon => playerIcon;

    public void SetPlayer(Players p) => owner = p;

    public void AddChips(int chips)
    {
        playerChips += chips;
        playerChipRP.Value = playerChips;
    }

    public void SetChips(int chips)
    {
        playerChips = chips;
        playerChipRP.Value = playerChips;
    }
    public void Refresh()
    {
        playerChipRP.Value = playerChips;
    }

    public ReactiveProperty<int> OnPlayerChipUpdated() => playerChipRP;

}
