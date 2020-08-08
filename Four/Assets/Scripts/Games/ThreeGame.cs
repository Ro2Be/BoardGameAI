using System.Collections.Generic;

public class ThreeGame : Game
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
        board = new Board(new Size(5, 4));
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
}