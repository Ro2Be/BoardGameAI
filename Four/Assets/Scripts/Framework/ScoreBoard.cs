using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard
{
    private Dictionary<IGameAgent, int> playerToWinAmounts = new Dictionary<IGameAgent, int>();
    private Dictionary<IGameAgent, int> playerToLosAmounts = new Dictionary<IGameAgent, int>();
    private int gameAmount;
    private int drawAmount;

    public void RegisterPlayers(IGameAgent[] players)
    {
        foreach (IGameAgent player in players)
        {
            playerToWinAmounts.Add(player, 0);
            playerToLosAmounts.Add(player, 0);
        }
    }

    public void RegisterWin(IGameAgent player)
    {
        ++gameAmount;
        if (player != null)
        {
            ++playerToWinAmounts[player];
            ++playerToLosAmounts[player.opponent];
        }
        else
            ++drawAmount;
        if (gameAmount % 1000 == 999)
        {
            string s = string.Empty;
            List<IGameAgent> gameAgents = new List<IGameAgent>(playerToWinAmounts.Keys);
            foreach (IGameAgent gameAgent in gameAgents)
            {
                s += $"{gameAgent.GetName()}: wins: {playerToWinAmounts[gameAgent]} - losses: {playerToLosAmounts[gameAgent]}\n";
                playerToWinAmounts[gameAgent] = 0;
                playerToLosAmounts[gameAgent] = 0;
            }
            Debug.Log($"{s}Draws: {drawAmount}\n");
            drawAmount = 0;
        }
    }
}
