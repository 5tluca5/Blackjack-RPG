using UnityEngine;

public static class LogExtension 
{
    public static string LogPlayerName(this string name)
    {
        return $"<color=#4fe344>{name}</color>";
    }

    public static string LogWinningChips(this int chips)
    {
        return $"<color=#eddd51>{chips}</color>";
    }
    public static string LogLossingChips(this int chips)
    {
        return $"<color=##ed5156>{chips}</color>";
    }
}
