using System.Collections.Generic;
using UnityEngine;

public class BNSAgent : AlgorithmAgent
{
    Dictionary<ulong, Dictionary<ulong, float>> cache = new Dictionary<ulong, Dictionary<ulong, float>>();

    public override void HandleOnGameBegin()
    {
        base.HandleOnGameBegin();
        cache.Clear();
    }

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
                //float bestValue = -AlphaBeta(new Board(game.board, this, possibleMoves[i]), opponent, -testValue, -testValue + 1);
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

    protected float TTNegaMaxAlphaBeta(Board board, IGameAgent gameAgent, float alpha = float.MinValue, float beta = float.MaxValue, int depth = int.MaxValue)
    { 
        if (cache.ContainsKey(board.GetBitMask(gameAgent.id).bits))
        {
            if (cache[board.GetBitMask(gameAgent.id).bits].ContainsKey(board.GetBitMask(-gameAgent.id).bits))
                return cache[board.GetBitMask(gameAgent.id).bits][board.GetBitMask(-gameAgent.id).bits];
        }
        else
        {
            cache.Add(board.GetBitMask(gameAgent.id).bits, new Dictionary<ulong, float>());
        }

        float score = float.MinValue; ;
        Game.State gameState = game.GetState(board, gameAgent);
        if (gameState != Game.State.playing || depth == 0)
            score = game.GetScore(board, gameAgent, gameState);
        else
        {
            List<Position> possibleMoves = game.GetPossibleMoves(board, gameAgent);
            for (int i = 0; i < possibleMoves.Count; ++i)
            {
                score = Mathf.Max(score, -TTNegaMaxAlphaBeta(new Board(board, gameAgent, possibleMoves[i]), gameAgent.opponent, -beta, -alpha, depth - 1));
                beta = Mathf.Max(score, beta);
                if (beta <= alpha)
                    break;
            }
        }

        if (depth != 0) //don't cache a heuristic score
            cache[board.GetBitMask(gameAgent.id).bits].Add(board.GetBitMask(-gameAgent.id).bits, score);

        return score;
    }
}