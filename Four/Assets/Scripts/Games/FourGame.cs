using System.Collections.Generic;

public class FourGame : Game
{
    private readonly BitMask[] bitFours = new BitMask[4]
    {
        new BitMask(0b_00001111, new Size(4, 1)),                            // -
        new BitMask(0b_00000001_00000001_00000001_00000001, new Size(1, 4)), // |
        new BitMask(0b_00000001_00000010_00000100_00001000, new Size(4, 4)), // /
        new BitMask(0b_00001000_00000100_00000010_00000001, new Size(4, 4))  // \
    };

    protected override void Awake()
    {
        base.Awake();
        board = new Board(new Size(7, 6));
        moves = new Position[board.size.x * board.size.y];
    }

    public override List<Position> GetPossibleMoves()
    {
        List<Position> possibleMoves = new List<Position>();
        for (Position position = new Position(0, 0); position.x < board.size.x; ++position.x)
            for (position.y = 0; position.y < board.size.y; ++position.y)
                if (board.GetState(position) == 0)
                {
                    possibleMoves.Add(position);
                    break;
                }
        return possibleMoves;
    }

    public override bool GetIsWin(GameAgent gameAgent)
    {
        for (int bitFourIndex = 0; bitFourIndex < bitFours.Length; ++bitFourIndex)
            for (Position position = new Position(0, 0); position.x <= board.size.x - bitFours[bitFourIndex].size.x; ++position.x)
                for (position.y = 0; position.y <= board.size.y - bitFours[bitFourIndex].size.y; ++position.y)
                {
                    ulong fourMask = bitFours[bitFourIndex].bits * BitMask.GetBitMask(position);
                    if ((board.GetBitMask(gameAgent.behaviorParameters.TeamId).bits & fourMask) == fourMask)
                        return true;
                }
        return false;
    }

    public override bool GetIsDraw()
        => moveIndex == board.size.x * board.size.y;
}