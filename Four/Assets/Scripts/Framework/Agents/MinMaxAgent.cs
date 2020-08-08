using System.Collections.Generic;

public abstract class MinMaxAgent : GameAgent
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

    protected float GetScore(GameAgent player, Position move)
    {
        float score = float.MaxValue;

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
                float possibleMoveScore = -GetScore(player.opponent, possibleMoves[i]);
                if (possibleMoveScore < score)
                    score = possibleMoveScore;
            }
        }

        game.board.SetState(move, 0);
        --game.moveIndex;
        return score;
    }
}