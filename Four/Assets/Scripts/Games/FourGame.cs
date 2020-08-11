using System.Collections.Generic;

public class FourGame : Game
{
    private const ulong fullBoardBits = 0b_01111111_01111111_01111111_01111111_01111111_01111111;

    private readonly BitMask[] winMasks = new BitMask[4]
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

    public override List<Position> GetPossibleMoves(Board board, IGameAgent gameAgent)
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

    public override State GetState(Board board, IGameAgent gameAgent)
    {
        if (((board.GetBitMask(-1).bits | board.GetBitMask(+1).bits) & fullBoardBits) == fullBoardBits)
            return State.draw;

        for (int winMaskIndex = 0; winMaskIndex < winMasks.Length; ++winMaskIndex)
            for (Position position = new Position(0, 0); position.x <= board.size.x - winMasks[winMaskIndex].size.x; ++position.x)
                for (position.y = 0; position.y <= board.size.y - winMasks[winMaskIndex].size.y; ++position.y)
                {
                    ulong winBits = winMasks[winMaskIndex].bits * BitMask.GetBits(position);
                    if ((board.GetBitMask(gameAgent.id).bits & winBits) == winBits)
                        return State.win;
                    if ((board.GetBitMask(gameAgent.opponent.id).bits & winBits) == winBits)
                        return State.loss;
                }

        return State.playing;
    }

    public override float GetScore(Board board, IGameAgent gameAgent, State gameState)
    {
        switch (gameState)
        {
            case Game.State.win:
                return +1 - board.moveIndex / 100f;
            case Game.State.draw:
                return 0;
            case Game.State.loss:
                return -1 + board.moveIndex / 100f;
            default:
                return 0;
        }
    }
}