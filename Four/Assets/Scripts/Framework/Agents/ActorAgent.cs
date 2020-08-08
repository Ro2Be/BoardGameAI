using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public abstract class ActorAgent : GameAgent
{
    [SerializeField]
    private int vectorActionLength;

    public static List<int> actionMask = new List<int>();

    protected abstract Position GetMove(float[] vectorAction);

    public override void RequestMove()
        => RequestDecision();

    public override void CollectObservations(VectorSensor sensor)
    {
        if (game.moveIndex == 0)
            actionMask.Clear();
        for (Position position = new Position(0, 0); position.x < game.board.size.x; ++position.x)
            for (position.y = 0; position.y < game.board.size.y; ++position.y)
                sensor.AddObservation(game.board.GetState(position) == +behaviorParameters.TeamId);
        for (Position position = new Position(0, 0); position.x < game.board.size.x; ++position.x)
            for (position.y = 0; position.y < game.board.size.y; ++position.y)
                sensor.AddObservation(game.board.GetState(position) == -behaviorParameters.TeamId);
    }

    public override void CollectDiscreteActionMasks(DiscreteActionMasker discreteActionMasker)
    {
        //sometimes the actionMask seems to fail (I found some bug report from 2018 comfirmed by a Unity Dev)
        //this function is called at the wrong moment
        //ignoring the mask if this happens fixes this
        if (actionMask.Count == vectorActionLength)
            return;

        discreteActionMasker.SetMask(0, actionMask);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        //sometimes the actionMask seems to fail (I found some bug report from 2018 comfirmed by a Unity Dev)
        //CollectDiscreteActionMasks is called at the wrong moment
        //just asking to make the decision again fixes this
        if (actionMask.Contains((int)vectorAction[0]))
        {
            //string text = $"ActionMask contains move\nMove: {(int)vectorAction[0]}\nList: +";
            //foreach (int action in actionMask)
            //    text += $"{action.ToString()}, ";
            //Debug.Log(text);
            RequestDecision();
            return;
        }

        game.DoMove(GetMove(vectorAction));
    }
}
