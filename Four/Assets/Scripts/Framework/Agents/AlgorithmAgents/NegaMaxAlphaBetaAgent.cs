using System.Collections.Generic;
using UnityEngine;

public class NegaMaxAlphaBetaAgent : AlgorithmAgent
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
            float score = -GetScore(new Board(game.board, this, possibleMoves[i]), opponent, maximumSearchDepth);
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
        Game.State gameState = game.GetState(board, gameAgent);
        if (gameState != Game.State.playing || depth == 0)
            return game.GetScore(board, gameAgent, gameState);
        else
        {
            float score = float.MinValue;
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
            for (int i = 0; i < possibleMoves.Count; ++i)
            {
                score = Mathf.Max(score, -GetScore(new Board(board, gameAgent, possibleMoves[i]), gameAgent.opponent, -beta, -alpha, depth - 1));
                beta = Mathf.Max(score, beta);
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