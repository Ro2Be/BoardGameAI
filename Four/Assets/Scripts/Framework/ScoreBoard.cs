using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard
{
    private Dictionary<GameAgent, int> playerToWinAmounts = new Dictionary<GameAgent, int>();
    private int gameAmount;
    private int drawAmount;

    public void RegisterPlayers(GameAgent[] players)
    {
        foreach (GameAgent player in players)
            playerToWinAmounts.Add(player, 0);
    }

    public void RegisterWin(GameAgent player)
    {
        ++gameAmount;
        if (player != null)
            ++playerToWinAmounts[player];
        else
            ++drawAmount;
        if (gameAmount % 1001 == 1000)
        {
            string s = string.Empty;
            foreach (KeyValuePair<GameAgent, int> playerToWinAmount in playerToWinAmounts)
            {
                s += $"{playerToWinAmount.Key.name}: {playerToWinAmount.Value}\n";
                playerToWinAmounts[playerToWinAmount.Key] = 0;
            }
            Debug.Log($"{s}Draws: {drawAmount}\n");
            drawAmount = 0;
        }
    }
}
