using System.Collections.Generic;
using UnityEngine;

public class TicTacToeGame : Game
{
    private readonly BitMask[] winStates = new BitMask[4]
    {
        new BitMask(0b_00000111, new Size(3, 1)),                   // -
        new BitMask(0b_00000001_00000001_00000001, new Size(1, 3)), // |
        new BitMask(0b_00000001_00000010_00000100, new Size(3, 3)), // /
        new BitMask(0b_00000100_00000010_00000001, new Size(3, 3))  // \
    };

    protected override void Awake()
    {
        base.Awake();
        board = new Board(new Size(3, 3));
        moves = new Position[board.size.x * board.size.y];
    }

    public override List<Position> GetPossibleMoves()
    {
        List<Position> possibleMoves = new List<Position>();
        for (Position position = new Position(0, 0); position.x < board.size.x; ++position.x)
            for (position.y = 0; position.y < board.size.y; ++position.y)
                if (board.GetState(position) == 0)
                    possibleMoves.Add(position);
        return possibleMoves;
    }

    public override bool GetIsWin(GameAgent gameAgent)
    {
        for (int winStateIndex = 0; winStateIndex < winStates.Length; ++winStateIndex)
            for (Position position = new Position(0, 0); position.x <= board.size.x - winStates[winStateIndex].size.x; ++position.x)
                for (position.y = 0; position.y <= board.size.y - winStates[winStateIndex].size.y; ++position.y)
                {
                    ulong mask = winStates[winStateIndex].bits * BitMask.GetBitMask(position);
                    if ((board.GetBitMask(gameAgent.behaviorParameters.TeamId).bits & mask) == mask)
                        return true;
                }
        return false;
    }

    public override bool GetIsDraw()
        => moveIndex == board.size.x * board.size.y;

    /// <summary>
    /// Sets up a situation with 2 stones on the board ready to be completed into a three in a row
    /// ! at this point there are still impossible exercises so a score around .6 is perfect
    /// </summary>
    protected void SetUpExercise()
    {
        ActorAgent.actionMask.Clear();
        Position position0 = new Position(Random.Range(0, 3), Random.Range(0, 3));
        ActorAgent.actionMask.Add(position0.x + 3 * position0.y);
        Position position1 = new Position(Random.Range(0, 3), Random.Range(0, 3));
        while (position0.x == position1.x && position0.y == position1.y)
            position1 = new Position(Random.Range(0, 3), Random.Range(0, 3));
        ActorAgent.actionMask.Add(position1.x + 3 * position1.y);
        board.SetState(position0, -1);
        userInterface?.DoMove(position0, -1);
        board.SetState(position1, -1);
        userInterface?.DoMove(position1, -1);
    }
}