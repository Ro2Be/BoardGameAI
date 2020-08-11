using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Equivalent to NegaScout and MTDF(n)
/// </summary>
public class PVSAgent : AlgorithmAgent
{
    public override void RequestMove()
    {
        int maxMoveIndex = -1;
        float maxScore = float.MinValue;
        List<Position> possibleMoves = game.GetPossibleMoves(game.board, this);
        for (int i = 0; i < possibleMoves.Count; ++i)
        {
            Board newBoard = new Board(game.board, this, possibleMoves[i]);
            float score = -GetScore(newBoard, opponent, depth: maximumSearchDepth);
            if (maxScore < score)
            {
                maxMoveIndex = i;
                maxScore = score;
            }
        }
        game.DoMove(possibleMoves[maxMoveIndex]);
    }

    protected float GetScore(Board board, IGameAgent gameAgent, float alpha = float.MinValue, float beta = float.MaxValue, int depth = int.MaxValue)
    {
        Game.State gameState = game.GetState(board, gameAgent);
        if (gameState != Game.State.playing || depth == 0)
            return game.GetScore(board, gameAgent, gameState);
        else
        {
            float score = float.MinValue;
            List<Position> possibleMoves = game.GetPossibleMoves(board, gameAgent);
            for (int i = 0; i < possibleMoves.Count; ++i)
            {
                Board newBoard = new Board(board, gameAgent, possibleMoves[i]);
                if (i == 0)
                    score = -GetScore(newBoard, gameAgent.opponent, -beta, -alpha, depth - 1);
                else
                    score = -GetScore(newBoard, gameAgent.opponent, -alpha - 1, -alpha,  depth - 1);
                if (alpha < score && score < beta)
                    score = -GetScore(newBoard, gameAgent.opponent,-beta, -score, depth - 1);
                alpha = Mathf.Max(alpha, score);
                if (beta <= alpha)
                    break;
            }
            return alpha;
        }
    }
}
