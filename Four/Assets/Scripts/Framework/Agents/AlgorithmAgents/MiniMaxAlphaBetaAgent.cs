using System.Collections.Generic;
using UnityEngine;

public class MiniMaxAlphaBetaAgent : AlgorithmAgent
{
    //--- Killer heuristic ---
    private Queue<Position> killers = new Queue<Position>();
    //-------------------------

    public override void RequestMove()
    {
        int maxMoveIndex = -1;
        float maxScore = float.MinValue;
        List<Position> possibleMoves = game.GetPossibleMoves(game.board, this);
        for (int i = 0; i < possibleMoves.Count; ++i)
        {
            float score = GetScore(new Board(game.board, this, possibleMoves[i]), opponent, depth: maximumSearchDepth);
            //Debug.Log($"{possibleMoves[i].x}, {possibleMoves[i].y}: {score}");
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
        Game.State gameState = game.GetState(board, game.activeGameAgent);
        if (gameState != Game.State.playing || depth == 0)
            return GetReward(gameState, board, game.activeGameAgent);
        else
        {
            List<Position> possibleMoves = game.GetPossibleMoves(board, gameAgent);
            //--- Killer heuristic ---
            for (int i = 0; i < possibleMoves.Count; ++i)
                if (killers.Contains(possibleMoves[i]))
                {
                    Position move = possibleMoves[i];
                    possibleMoves.RemoveAt(i);
                    possibleMoves.Insert(0, move);
                }
            //-------------------------
            if (gameAgent == game.activeGameAgent)
            {
                float score = float.MinValue;
                for (int i = 0; i < possibleMoves.Count; ++i)
                {
                    score = Mathf.Max(score, GetScore(new Board(board, gameAgent, possibleMoves[i]), gameAgent.opponent, -beta, -alpha, depth - 1));
                    alpha = Mathf.Max(alpha, score);
                    if (beta <= alpha)
                    {
                        //--- Killer heuristic ---
                        killers.Enqueue(possibleMoves[i]);
                        if (2 < killers.Count)
                            killers.Dequeue();
                        //-------------------------
                        break;
                    }
                }
                return score;
            }
            else
            {
                float score = float.MaxValue;
                for (int i = 0; i < possibleMoves.Count; ++i)
                {
                    score = Mathf.Min(score, GetScore(new Board(board, gameAgent, possibleMoves[i]), gameAgent.opponent, -beta, -alpha, depth - 1));
                    beta = Mathf.Min(beta, score);
                    if (beta <= alpha)
                    {
                        //--- Killer heuristic ---
                        killers.Enqueue(possibleMoves[i]);
                        if (2 < killers.Count)
                            killers.Dequeue();
                        //-------------------------
                        break;
                    }
                }
                return score;
            }
        }
    }
}