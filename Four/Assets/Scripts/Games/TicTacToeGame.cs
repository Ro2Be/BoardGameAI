using System.Collections.Generic;

public class TicTacToeGame : Game
{
    private const ulong fullBoardBits = 0b_00000111_00000111_00000111;

    private readonly BitMask[] winMasks = new BitMask[4]
    {
        new BitMask(0b_00000111, new Size(3, 1)),                   // -
        new BitMask(0b_00000001_00000001_00000001, new Size(1, 3)), // |
        new BitMask(0b_00000001_00000010_00000100, new Size(3, 3)), // /
        new BitMask(0b_00000100_00000010_00000001, new Size(3, 3))  // \
    };

    Position[] preferedMoves = new Position[9]
    {
        new Position(1, 1), //Center
        new Position(0, 0), //Corners
        new Position(0, 2), //Corners
        new Position(2, 0), //Corners
        new Position(2, 2), //Corners
        new Position(0, 1), //Middles
        new Position(1, 0), //Middles
        new Position(1, 2), //Middles
        new Position(2, 1)  //Middles
    };

    protected override void Awake()
    {
        base.Awake();
        board = new Board(new Size(3, 3));
        moves = new Position[board.size.x * board.size.y];
    }

    public override List<Position> GetPossibleMoves(Board board, IGameAgent gameAgent)
    {
        List<Position> possibleMoves = new List<Position>();
        //Order possible moves based on potential (improve the amount of alpha-beta pruning)
        for (int i = 0; i < preferedMoves.Length; ++i)
            if (board.GetState(preferedMoves[i]) == 0)
                possibleMoves.Add(preferedMoves[i]);
        //--------------------------------------------------------------------------------------
        return possibleMoves;
    }

    public override State GetState(Board board, IGameAgent gameAgent)
    {
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

        if (((board.GetBitMask(-1).bits | board.GetBitMask(+1).bits) & fullBoardBits) == fullBoardBits)
            return State.draw;

        return State.playing;
    }

    public override float GetScore(Board board, IGameAgent gameAgent, State gameState)
    {
        switch (gameState)
        {
            case Game.State.win:
                return +1 - board.moveIndex / 1000f;
            case Game.State.draw:
                return 0;
            case Game.State.loss:
                return -1 + board.moveIndex / 1000f;
            default:
                return 0;
        }
    }
}