using System.Collections.Generic;

public class ThreeAgent : ActorAgent
{
    List<int> actionMask = new List<int>();

    private readonly BitMask[] bitThrees = new BitMask[4]
    {
        new BitMask(0b_00000111, new Size(3, 1)),                   // -
        new BitMask(0b_00000001_00000001_00000001, new Size(1, 3)), // |
        new BitMask(0b_00000001_00000010_00000100, new Size(3, 3)), // /
        new BitMask(0b_00000100_00000010_00000001, new Size(3, 3))  // \
    };

    protected override List<int> GetActionMask()
        => actionMask;

    public override void HandleOnGameBegin()
        => actionMask.Clear();

    protected override Position GetMove(float[] vectorAction)
    {
        Position position = new Position((int)vectorAction[0], 0);
        while (game.board.GetState(position) != 0)
            ++position.y;
        return position;
    }

    public override void HandleOnGameMove(Position move)
    {
        if (move.y  == game.board.size.y - 1)
            actionMask.Add(move.x);
    }

    public override float GetReward(Game.State gameState, Board board, IGameAgent gameAgent)
    {
        switch (gameState)
        {
            case Game.State.win:
                return 1 - board.moveIndex / 32.0f;
            case Game.State.draw:
                return -0.01f;
            case Game.State.loss:
                return -1 - board.moveIndex / 32.0f;
            default:
                return 0;
        }
    }
}