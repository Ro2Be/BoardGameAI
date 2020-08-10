using System.Collections.Generic;
using UnityEngine;

public abstract class MTDfAgent : GameAgent
{
    public abstract float GetF(Board board, GameAgent gameAgent);

    public override void RequestMove()
    {
        int maxMoveIndex = -1;
        float maxScore = float.MinValue;
        List<Position> possibleMoves = game.GetPossibleMoves(game.board, this);
            for (int i = 0; i<possibleMoves.Count; ++i)
            {
                float score = -GetScore(new Board(game.board, this, possibleMoves[i]), opponent);
                if (maxScore<score)
                {
                    maxMoveIndex = i;
                    maxScore = score;
                }
            }
        game.DoMove(possibleMoves[maxMoveIndex]);
    }

    protected float GetScore(Board board, GameAgent gameAgent, int depth = int.MaxValue)
    {
        float guess = GetF(board, gameAgent);
        float lowerBound = float.MinValue;
        float upperBound = float.MaxValue;
        while (lowerBound < upperBound)
        {
            float beta = Mathf.Max(guess, lowerBound + 1);
            guess = MiniMaxAlphaBetaWithMemory(board, gameAgent.opponent, beta - 1, beta, depth);
            if (guess < beta)
                upperBound = guess;
            else
                lowerBound = guess;
        }
        return guess;
    }

    protected float MiniMaxAlphaBetaWithMemory(Board board, GameAgent gameAgent, float alpha = float.MinValue, float beta = float.MaxValue, int depth = int.MaxValue)
    {
        Debug.Log("MiniMaxAlphaBetaWithMemory not implemented");
        return 0;
    }
}
