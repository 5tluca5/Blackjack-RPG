using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProfile", menuName = "Scriptable Objects/PlayerProfile")]
public class PlayerProfile : ScriptableObject
{
    [SerializeField] private string playerName;
    [SerializeField] private int playerLevel;
    [SerializeField] private int playerExp;
    [SerializeField] private int playerChips;

    public int Chips => playerChips;

    public void AddChips(int chips)
    {
        playerChips += chips;
    }
}
