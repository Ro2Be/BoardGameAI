using System.Collections.Generic;
using UnityEngine;

public abstract class AlphaBetaAgent : GameAgent
{
    public override void RequestMove()
    {
        int maxMoveIndex = -1;
        float maxScore = float.MinValue;
        List<Position> possibleMoves = game.GetPossibleMoves();
        for (int i = 0; i < possibleMoves.Count; ++i)
        {
            float score = GetScore(this, possibleMoves[i]);
            if (maxScore < score)
            {
                maxMoveIndex = i;
                maxScore = score;
            }
        }
        game.DoMove(possibleMoves[maxMoveIndex]);
    }

    protected float GetScore(GameAgent player, Position move, float alpha = float.MinValue, float beta = float.MaxValue)
    {
        float score = float.MaxValue; ;

        game.board.SetState(move, player.behaviorParameters.TeamId);
        ++game.moveIndex;

        if (game.GetIsWin(player))
            score = GetReward(GameState.win);
        //else if (game.GetIsWin(player.opponent))
        //    score = GetReward(GameState.loss);
        else if (game.GetIsDraw())
            score = GetReward(GameState.draw);
        else
        {
            List<Position> possibleMoves = game.GetPossibleMoves();
            for (int i = 0; i < possibleMoves.Count; ++i)
            {
                score = Mathf.Min(score, -GetScore(player.opponent, possibleMoves[i], -beta, -alpha));
                beta = Mathf.Min(score, beta);
                if (beta <= alpha)
                    break;
            }
        }

        game.board.SetState(move, 0);
        --game.moveIndex;
        return score;
    }
}