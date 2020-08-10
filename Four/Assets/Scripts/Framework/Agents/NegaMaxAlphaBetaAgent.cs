﻿using System.Collections.Generic;
using UnityEngine;

public abstract class NegaMaxAlphaBetaAgent : GameAgent
{
    public override void RequestMove()
    {
        int maxMoveIndex = -1;
        float maxScore = float.MinValue;
        List<Position> possibleMoves = game.GetPossibleMoves(game.board, this);
        for (int i = 0; i < possibleMoves.Count; ++i)
        {
            float score = -GetScore(new Board(game.board, this, possibleMoves[i]), opponent);
            if (maxScore < score)
            {
                maxMoveIndex = i;
                maxScore = score;
            }
        }
        game.DoMove(possibleMoves[maxMoveIndex]);
    }

    protected float GetScore(Board board, GameAgent gameAgent, float alpha = float.MinValue, float beta = float.MaxValue, int depth = int.MaxValue)
    {
        Game.State gameState = game.GetState(board, gameAgent);
        if (gameState != Game.State.playing || depth == 0)
            return GetReward(gameState, board, gameAgent);
        else
        {
            float score = float.MinValue;
            List<Position> possibleMoves = game.GetPossibleMoves(board, gameAgent);
            for (int i = 0; i < possibleMoves.Count; ++i)
            {
                score = Mathf.Max(score, -GetScore(new Board(board, gameAgent, possibleMoves[i]), gameAgent.opponent, -beta, -alpha, depth - 1));
                beta = Mathf.Max(score, beta);
                if (beta <= alpha)
                    break;
            }
            return score;
        }
    }
}