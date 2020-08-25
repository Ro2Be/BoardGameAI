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
        => game.DoMove(BestNodeSearch(game.board, opponent, -1, 1, depth: maximumSearchDepth));

    public Position BestNodeSearch(Board board, IGameAgent gameAgent, float alpha = float.MinValue, float beta = float.MaxValue, int depth = int.MaxValue)
    {
        List<Position> possibleMoves = game.GetPossibleMoves(game.board, this);
        Position bestMove = possibleMoves.GetRandom();
        int subtreeCount = possibleMoves.Count;
        int betterMovesAmount = 0;
        do
        {
            float testValue = Guess(alpha, beta, subtreeCount);
            betterMovesAmount = 0;
            for (int i = 0; i < possibleMoves.Count; ++i)
            {
                float moveValue = -TTNegaMaxAlphaBeta(new Board(game.board, this, possibleMoves[i]), opponent, -testValue, -testValue + 1);
                if (testValue <= moveValue)
                {
                    ++betterMovesAmount;
                    bestMove = possibleMoves[i];
                }
            }
            if (betterMovesAmount == 0)
            {
                beta = testValue;
            }
            else
            {
                subtreeCount = betterMovesAmount;
                alpha = testValue;
            }
        }
        while (2 < beta - alpha && betterMovesAmount != 1);
        return bestMove;
    }

    protected float Guess(float alpha, float beta, float subtreeCount)
        => alpha + (beta - alpha) * (subtreeCount - 1) / subtreeCount;

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