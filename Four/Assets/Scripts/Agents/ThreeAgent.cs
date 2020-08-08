public class ThreeAgent : ActorAgent
{
    private readonly BitMask[] bitThrees = new BitMask[4]
    {
        new BitMask(0b_00000111, new Size(3, 1)),                   // -
        new BitMask(0b_00000001_00000001_00000001, new Size(1, 3)), // |
        new BitMask(0b_00000001_00000010_00000100, new Size(3, 3)), // /
        new BitMask(0b_00000100_00000010_00000001, new Size(3, 3))  // \
    };

    protected override Position GetMove(float[] vectorAction)
    {
        Position position = new Position((int)vectorAction[0], 0);
        while (game.board.GetState(position) != 0)
            ++position.y;
        return position;
    }

    public override void OnGameMove(Position move)
    {
        if (move.y + 1  == game.board.size.y)
            actionMask.Add(move.x);
    }

    public override float GetReward(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.win:
                return 1 - game.moveIndex / 32.0f;
            case GameState.draw:
                return -0.01f;
            case GameState.loss:
                return -1 - game.moveIndex / 32.0f;
            default:
                return 0;
        }
    }
}