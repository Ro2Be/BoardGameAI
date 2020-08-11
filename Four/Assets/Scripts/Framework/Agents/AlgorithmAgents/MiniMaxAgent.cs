using System.Collections.Generic;
using UnityEngine;

public class MiniMaxAgent : AlgorithmAgent
{
    public override void RequestMove()
    {
        int maxMoveIndex = -1;
        float maxScore = float.MinValue;
        List<Position> possibleMoves = game.GetPossibleMoves(game.board, this);
        for (int i = 0; i < possibleMoves.Count; ++i)
        {
            float score = GetScore(new Board(game.board, this, possibleMoves[i]), opponent, maximumSearchDepth);
            //Debug.Log($"{possibleMoves[i].x}, {possibleMoves[i].y}: {score}");
            if (maxScore < score)
            {
                maxMoveIndex = i;
                maxScore = score;
            }
        }
        game.DoMove(possibleMoves[maxMoveIndex]);
    }

    protected float GetScore(Board board, IGameAgent gameAgent, int depth = int.MaxValue)
    {
        Game.State gameState = game.GetState(board, game.activeGameAgent);
        if (gameState != Game.State.playing || depth == 0)
            return game.GetScore(board, game.activeGameAgent, gameState);
        else
        {
            if(gameAgent == game.activeGameAgent)
            {
                float score = float.MinValue;
                List<Position> possibleMoves = game.GetPossibleMoves(board, gameAgent);
                for (int i = 0; i < possibleMoves.Count; ++i)
                    score = Mathf.Max(score, GetScore(new Board(board, gameAgent, possibleMoves[i]), gameAgent.opponent, depth - 1));
                return score;
            }
            else
            {
                float score = float.MaxValue;
                List<Position> possibleMoves = game.GetPossibleMoves(board, gameAgent);
                for (int i = 0; i < possibleMoves.Count; ++i)
                    score = Mathf.Min(score, GetScore(new Board(board, gameAgent, possibleMoves[i]), gameAgent.opponent, depth - 1));
                return score;
            }
        }
    }
}