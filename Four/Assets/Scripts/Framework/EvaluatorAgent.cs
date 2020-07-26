using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

public abstract class EvaluatorAgent : GameAgent
{
    #region Variables

    protected HeatMap heatMap;

    private Position evaluatedMove;

    private float evaluation;
    
    #endregion
    #region Agent override functions

    public override void CollectObservations(VectorSensor sensor)
    {
        for (Position position = new Position(0, 0); position.x < board.size.x; ++position.x)
            for (position.y = 0; position.y < board.size.y; ++position.y)
                //just show the board
                //sensor.AddObservation(game.board.GetState(position));                
                if (board.GetState(position) == 0)
                    //supposed to give better results than 0.f
                    sensor.AddObservation(0.01f);
                else
                    //show the board from the active players perspective
                    sensor.AddObservation(behaviorParameters.TeamId * board.GetState(position));
    }

    public override void OnActionReceived(float[] vectorAction)
        => evaluation = vectorAction[0] + float.Epsilon;

    #endregion
    #region GameAgent main functions

    public override void RequestMove()
        => StartCoroutine(RequestMoveCoroutine());

    protected IEnumerator RequestMoveCoroutine()
    {
        int possibleMoveAmount = -1;
        if (behaviorParameters.BehaviorType == BehaviorType.HeuristicOnly)
        {
            if (board.GetState(humanMove) != 0)
                yield break; //Wait for new input
            else
                DoMove(humanMove, behaviorParameters.TeamId);
        }
        else if (doRandomMoves)
        {
            List<Position> possibleMoves = new List<Position>();
            for (Position position = new Position(0, 0); position.x < board.size.x; ++position.x)
                for (position.y = 0; position.y < board.size.y; ++position.y)
                    if (board.GetState(position) == 0)
                        possibleMoves.Add(position);
            possibleMoveAmount += possibleMoves.Count;
            DoMove(possibleMoves[Random.Range(0, possibleMoves.Count)], behaviorParameters.TeamId);
        }
        else
        {
            heatMap = new HeatMap(board.size);
            for (Position position = new Position(0, 0); position.x < board.size.x; ++position.x)
                for (position.y = 0; position.y < board.size.y; ++position.y)
                    if (board.GetState(position) == 0)
                    {
                        ++possibleMoveAmount;
                        yield return Evaluate(position);
                    }
                    else
                    {
                        heatMap.SetHeat(position, float.MinValue);
                    }
            DoMove(heatMap.GetHottestPosition(), behaviorParameters.TeamId);
        }
        if (GetIsWin(behaviorParameters.TeamId))
            ProcesWin();
        else if (possibleMoveAmount == 0)
            ProcesDraw();
        else if (opponent.behaviorParameters.BehaviorType != BehaviorType.HeuristicOnly)
            opponent.RequestMove();
    }

    IEnumerator Evaluate(Position position)
    {
        //board.SetState(position, behaviorParameters.TeamId);
        //evaluatedMove = position;
        //evaluation = float.MinValue;
        //RequestDecision();
        //yield return new WaitUntil(() => evaluation != float.MinValue);
        //board.SetState(position, 0);
        //heatMap.SetHeat(evaluatedMove, evaluation);

        ulong bitsPlayer0 = board.GetBitMask(-1).bits;
        ulong bitsPlayer1 = board.GetBitMask(+1).bits;
        evaluatedMove = position;
        evaluation = float.MinValue;

        switch (position.x + board.size.y * position.y)
        {
            case 2:
                board.MirrorHorizontally();
                position = new Position(0, 0);
                break;
            case 3:
                board.MirrorDiagonally();
                position = new Position(0, 1);
                break;
            case 5:
                board.MirrorHorizontally();
                board.MirrorDiagonally();
                position = new Position(0, 1);
                break;
            case 6:
                board.MirrorVertically();
                position = new Position(0, 0);
                break;
            case 7:
                board.MirrorVertically();
                position = new Position(0, 1);
                break;
            case 8:
                board.MirrorHorizontally();
                board.MirrorVertically();
                position = new Position(0, 0);
                break;
        }
        board.SetState(position, behaviorParameters.TeamId);
        RequestDecision();
        yield return new WaitUntil(() => evaluation != float.MinValue);
        board.GetBitMask(-1).bits = bitsPlayer0;
        board.GetBitMask(+1).bits = bitsPlayer1;
        heatMap.SetHeat(evaluatedMove, evaluation);
    }

    protected abstract bool GetIsWin(int teamId);

    protected abstract void ProcesWin();

    protected abstract void ProcesDraw();

    #endregion
}
