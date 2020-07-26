using System.Collections;
using UnityEngine;

public class TicTacToeAgent : ActorAgent
{
     private readonly BitMask[] bitThrees = new BitMask[4]
    {
        new BitMask(0b_00000111, new Size(3, 1)),                   // -
        new BitMask(0b_00000001_00000001_00000001, new Size(1, 3)), // |
        new BitMask(0b_00000001_00000010_00000100, new Size(3, 3)), // /
        new BitMask(0b_00000100_00000010_00000001, new Size(3, 3))  // \
    };

    //678
    //345
    //012

    public override void Heuristic(float[] actionsOut)
        => actionsOut[0] = humanMove.x + 3 * humanMove.y;

    protected override Position GetMove(float[] vectorAction)
        => new Position((int)vectorAction[0] % 3, (int)vectorAction[0] / 3);

    protected override void UpdateActionMask(Position lastMove)
        => actionMask.Add(lastMove.x + 3 * lastMove.y);

    protected override bool GetIsWin(int teamId)
    {
        for (int bitThreeIndex = 0; bitThreeIndex < bitThrees.Length; ++bitThreeIndex)
            for (Position position = new Position(0, 0); position.x <= board.size.x - bitThrees[bitThreeIndex].size.x; ++position.x)
                for (position.y = 0; position.y <= board.size.y - bitThrees[bitThreeIndex].size.y; ++position.y)
                {
                    ulong fourMask = bitThrees[bitThreeIndex].bits * BitMask.GetBitMask(position);
                    if ((board.GetBitMask(teamId).bits & fourMask) == fourMask)
                        return true;
                }
        return false;
    }

    protected override void ProcesWin()
    {
        Debug.Log(behaviorParameters.TeamId);
        SetReward(1 - moveIndex / 32.0f);
        opponent.SetReward(-1 - moveIndex / 32.0f);
        EndEpisode();
        opponent.EndEpisode();
    }
    //=> StartCoroutine(EndGame(1 - game.moveIndex / 32.0f, -1 + game.moveIndex / 32.0f));

    protected override void ProcesDraw()
    {
        Debug.Log("DRAW");
        SetReward(-0.01f);
        opponent.SetReward(-0.01f);
        EndEpisode();
        opponent.EndEpisode();
    }
    //=> StartCoroutine(EndGame(-0.01f, -0.01f));

    IEnumerator EndGame(float myReward, float opponentReward)
    {
        yield return new WaitForSeconds(1.0f);
        SetReward(myReward);
        opponent.SetReward(opponentReward);
        EndEpisode();
        opponent.EndEpisode();
    }
}