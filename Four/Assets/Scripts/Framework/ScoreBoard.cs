using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard
{
    private Dictionary<IGameAgent, int> playerToWinAmounts = new Dictionary<IGameAgent, int>();
    private int gameAmount;
    private int drawAmount;

    public void RegisterPlayers(IGameAgent[] players)
    {
        foreach (IGameAgent player in players)
            playerToWinAmounts.Add(player, 0);
    }

    public void RegisterWin(IGameAgent player)
    {
        ++gameAmount;
        if (player != null)
            ++playerToWinAmounts[player];
        else
            ++drawAmount;
        if (gameAmount % 1001 == 1000)
        {
            string s = string.Empty;
            List<IGameAgent> gameAgents = new List<IGameAgent>(playerToWinAmounts.Keys);
            foreach (IGameAgent gameAgent in gameAgents)
            {
                s += $"{gameAgent.GetName()}: {playerToWinAmounts[gameAgent]}\n";
                playerToWinAmounts[gameAgent] = 0;
            }
            Debug.Log($"{s}Draws: {drawAmount}\n");
            drawAmount = 0;
        }
    }
}
