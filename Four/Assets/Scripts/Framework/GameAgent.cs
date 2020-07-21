using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

public abstract class GameAgent : Agent
{
    #region Variables

    [HideInInspector]
    public BehaviorParameters behaviorParameters;

    [SerializeField]
    protected GameAgent opponent;

    [SerializeField]
    private int vectorActionLength;

    [SerializeField]
    private bool doRandomMoves;

    protected Game game;

    protected static List<int> actionMask = new List<int>();

    #endregion
    #region Agent override functions

    public override void Initialize()
    {
        game = FindObjectOfType<Game>();
        behaviorParameters = GetComponent<BehaviorParameters>();
    }

    public override void OnEpisodeBegin()
    {
        actionMask.Clear();
        game.Start();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        for (Position position = new Position(0, 0); position.x < game.board.size.x; ++position.x)
            for (position.y = 0; position.y < game.board.size.y; ++position.y)
                sensor.AddObservation(game.board.GetState(position));
    }

    public override void CollectDiscreteActionMasks(DiscreteActionMasker discreteActionMasker)
        => discreteActionMasker.SetMask(0, actionMask);

    public override void Heuristic(float[] actionsOut)
        => actionsOut[0] = game.input.x;

    public override void OnActionReceived(float[] vectorAction)
    {
        //if player input was illegal, stop processing
        if (behaviorParameters.BehaviorType == BehaviorType.HeuristicOnly
         && actionMask.Contains((int)vectorAction[0]))
            return;

        //sometimes the actionMask seems to fail (I found some bug report from 2018 comfirmed by a Unity Dev)
        //just asking to make the decision again fixes this
        if (actionMask.Contains((int)vectorAction[0]))
        {
            //Debug.Log("ActionMask failed");
            RequestDecision();
            return;
        }

        Position position = doRandomMoves ? GetRandomMove() : GetMove(vectorAction);
        UpdateActionMask(position);
        game.DoMove(position);
        if (GetIsWin(behaviorParameters.TeamId))
            ProcesWin();
        else if (actionMask.Count == vectorActionLength)
            ProcesDraw();
        else if (opponent.behaviorParameters.BehaviorType != BehaviorType.HeuristicOnly)
            opponent.RequestDecision();
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
