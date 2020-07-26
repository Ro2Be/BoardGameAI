using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

public abstract class ActorAgent : GameAgent
{
    #region Variables

    [SerializeField]
    private int vectorActionLength;

    protected static List<int> actionMask = new List<int>();

    #endregion

    public override void RequestMove()
        => RequestDecision();

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

    public override void CollectDiscreteActionMasks(DiscreteActionMasker discreteActionMasker)
    {
        //sometimes the actionMask seems to fail (I found some bug report from 2018 comfirmed by a Unity Dev)
        //this function is called at the wrong moment
        //ignoring the mask if this happens fixes this
        if (actionMask.Count != vectorActionLength)
            discreteActionMasker.SetMask(0, actionMask);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        //if player input was illegal, stop processing
        if (behaviorParameters.BehaviorType == BehaviorType.HeuristicOnly
         && actionMask.Contains((int)vectorAction[0]))
            return;

        //sometimes the actionMask seems to fail (I found some bug report from 2018 comfirmed by a Unity Dev)
        //CollectDiscreteActionMasks is called at the wrong moment
        //just asking to make the decision again fixes this
        if (actionMask.Contains((int)vectorAction[0]) && !doRandomMoves)
        {
            //Debug.Log("ActionMask failed");
            RequestDecision();
            return;
        }

        Position position = doRandomMoves ? GetRandomMove() : GetMove(vectorAction);
        UpdateActionMask(position);
        DoMove(position, behaviorParameters.TeamId);
        if (GetIsWin(behaviorParameters.TeamId))
            ProcesWin();
        else if (actionMask.Count == vectorActionLength)
            ProcesDraw();
        else if (opponent.behaviorParameters.BehaviorType != BehaviorType.HeuristicOnly)
            opponent.RequestMove();
    }

    #endregion
    #region GameAgent abstract functions

    protected abstract Position GetMove(float[] vectorAction);

    protected abstract void UpdateActionMask(Position lastMove);

    protected abstract bool GetIsWin(int teamId);

    protected abstract void ProcesWin();

    protected abstract void ProcesDraw();

    #endregion
    #region GameAgent helper functions

    private Position GetRandomMove()
    {
        int action = 0;
        do action = Random.Range(0, vectorActionLength);
        while (actionMask.Contains(action));
        return GetMove(new float[] { action });
    }

    #endregion
}
