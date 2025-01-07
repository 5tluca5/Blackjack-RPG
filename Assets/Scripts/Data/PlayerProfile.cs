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
    [SerializeField] private int playerTotalFunds;
    [SerializeField] private int playerTableChips;

    private ReactiveProperty<int> playerChipRP = new(0);
    public Players Owner => owner;
    public int Chips => playerTableChips;
    public string PlayerName => playerName;
    public string LogPlayerName => playerName.LogPlayerName();

    public Sprite PlayerIcon => playerIcon;
    public int PlayerLevel => playerLevel;
    public void SetPlayer(Players p) => owner = p;

    public void AddChips(int chips)
    {
        playerTableChips += chips;
        playerChipRP.Value = playerTableChips;
    }

    public void SetChips(int chips)
    {
        playerTableChips = chips;
        playerChipRP.Value = playerTableChips;
    }
    public void Refresh()
    {
        playerChipRP.Value = playerTableChips;
    }

    public ReactiveProperty<int> OnPlayerChipUpdated() => playerChipRP;

}
