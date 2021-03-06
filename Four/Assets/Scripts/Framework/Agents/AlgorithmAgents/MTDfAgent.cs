﻿using System.Collections.Generic;
using UnityEngine;

public abstract class MTDfAgent : AlgorithmAgent
{
    Dictionary<ulong, Dictionary<ulong, float>> cache = new Dictionary<ulong, Dictionary<ulong, float>>();

    public abstract float GuessScore(Board board, IGameAgent gameAgent);

    public override void HandleOnGameBegin()
    {
        base.HandleOnGameBegin();
        cache.Clear();
    }

    public override void RequestMove()
    {
        int maxMoveIndex = -1;
        float maxScore = float.MinValue;
        List<Position> possibleMoves = game.GetPossibleMoves(game.board, this);
            for (int i = 0; i<possibleMoves.Count; ++i)
            {
                float score = -GetScore(new Board(game.board, this, possibleMoves[i]), opponent, maximumSearchDepth);
                if (maxScore<score)
                {
                    maxMoveIndex = i;
                    maxScore = score;
                }
            }
        game.DoMove(possibleMoves[maxMoveIndex]);
    }

    protected float GetScore(Board board, IGameAgent gameAgent, int depth = int.MaxValue)
    {
        float guess = GuessScore(board, gameAgent);
        float lowerBound = float.MinValue;
        float upperBound = float.MaxValue;
        while (lowerBound < upperBound)
        {
            float beta = Mathf.Max(guess, lowerBound + 1);
            guess = TTNegaMaxAlphaBeta(board, gameAgent, beta - 1, beta, depth);
            if (guess < beta)
                upperBound = guess;
            else
                lowerBound = guess;
        }
        return guess;
    }

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