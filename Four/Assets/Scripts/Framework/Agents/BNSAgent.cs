using System.Collections.Generic;
using UnityEngine;

public abstract class BNSAgent : GameAgent
{
    public override void RequestMove()
    {
        List<Position> possibleMoves = game.GetPossibleMoves(game.board, this);
        float betterCount = 0;
        Position bestMove = new Position(-1, -1);
        //do
        {
            //float testValue = Guess(alpha, beta, possibleMoves.Count);
            betterCount = 0;
            for (int i = 0; i < possibleMoves.Count; ++i)
            {
                float bestValue = -AlphaBeta(new Board(game.board, this, possibleMoves[i]), opponent, -testValue, -testValue + 1);
                //if (testValue <= bestValue)
                {
                    ++betterCount;
                    bestMove = possibleMoves[i];
                }
            }

            //(update number of sub - trees that exceeds separation test value)
            //(update alpha - beta range)
        }
        //while (2 < beta - alpha && betterCount != 1);
        //game.DoMove(bestMove);
    }

    protected float Guess(float alpha, float beta, float possibleMovesAmount)
        => alpha + (beta - alpha) * (possibleMovesAmount - 1) / possibleMovesAmount;

    protected float AlphaBeta(Board board, GameAgent gameAgent, float alpha = float.MinValue, float beta = float.MaxValue)
    {
        Debug.Log("AlphaBeta not implemented");
        return 0;
    }
}