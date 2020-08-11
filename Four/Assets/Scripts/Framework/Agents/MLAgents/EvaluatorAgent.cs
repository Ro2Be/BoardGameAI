using System.Collections;
using Unity.MLAgents.Sensors;
using UnityEngine;

public abstract class EvaluatorAgent : MLAgent
{

    protected HeatMap heatMap;
    protected int possibleMoveAmount;

    private Position evaluatedMove;
    private float evaluation;
    
    public override void CollectObservations(VectorSensor sensor)
    {
        for (Position position = new Position(0, 0); position.x < game.board.size.x; ++position.x)
            for (position.y = 0; position.y < game.board.size.y; ++position.y)
                //just show the board
                //sensor.AddObservation(game.board.GetState(position));                
                if (game.board.GetState(position) == 0)
                    //supposed to give better results than 0.f
                    sensor.AddObservation(0.01f);
                else
                    //show the board from the active players perspective
                    sensor.AddObservation(id * game.board.GetState(position));
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        evaluation = vectorAction[0] + float.Epsilon;
        //Debug.Log($"Player {behaviorParameters.TeamId}.OnActionReceived : evaluated move = ({evaluatedMove.x}, {evaluatedMove.y}) evaluation = {evaluation}");
    }

    public override void RequestMove()
        => StartCoroutine(RequestMoveCoroutine());

    protected IEnumerator RequestMoveCoroutine()
    {
        possibleMoveAmount = -1;
        heatMap = new HeatMap(game.board.size);
        for (Position position = new Position(0, 0); position.x < game.board.size.x; ++position.x)
            for (position.y = 0; position.y < game.board.size.y; ++position.y)
                if (game.board.GetState(position) == 0)
                {
                    ++possibleMoveAmount;
                    yield return Evaluate(position);
                }
                else
                {
                    heatMap.SetHeat(position, float.MinValue);
                }
        Position move = heatMap.GetHottestPosition();
        game.DoMove(heatMap.GetHottestPosition());
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

        ulong bitsPlayer0 = game.board.GetBitMask(-1).bits;
        ulong bitsPlayer1 = game.board.GetBitMask(+1).bits;
        evaluatedMove = position;
        evaluation = float.MinValue;
        switch (position.x + game.board.size.y * position.y)
        {
            case 2:
                game.board.MirrorHorizontally();
                position = new Position(0, 0);
                break;
            case 3:
                game.board.MirrorDiagonally();
                position = new Position(0, 1);
                break;
            case 5:
                game.board.MirrorHorizontally();
                game.board.MirrorDiagonally();
                position = new Position(0, 1);
                break;
            case 6:
                game.board.MirrorVertically();
                position = new Position(0, 0);
                break;
            case 7:
                game.board.MirrorVertically();
                position = new Position(0, 1);
                break;
            case 8:
                game.board.MirrorHorizontally();
                game.board.MirrorVertically();
                position = new Position(0, 0);
                break;
        }
        game.board.SetState(position, id);
        RequestDecision();
        yield return new WaitUntil(() => evaluation != float.MinValue);
        game.board.GetBitMask(-1).bits = bitsPlayer0;
        game.board.GetBitMask(+1).bits = bitsPlayer1;
        heatMap.SetHeat(evaluatedMove, evaluation);
    }
}