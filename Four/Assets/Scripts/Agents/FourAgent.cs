using System.Collections.Generic;

public class FourAgent : ActorAgent
{
    List<int> actionMask = new List<int>();

    protected override Position GetMove(float[] vectorAction)
    {
        Position position = new Position((int)vectorAction[0], 0);
        while (game.board.GetState(position) != 0)
            ++position.y;
        return position;
    }

    protected override List<int> GetActionMask()
        => actionMask;

    public override void OnGameBegin()
        => actionMask.Clear();

    public override void OnGameMove(Position move)
    {
        if (move.y + 1 == game.board.size.y)
            actionMask.Add(move.x);
    }

    public override float GetReward(Game.State gameState, Board board, GameAgent gameAgent)
    {
        switch (gameState)
        {
            case Game.State.win:
                return 1 - game.moveIndex / 32.0f;
            case Game.State.draw:
                return -0.01f;
            case Game.State.loss:
                return -1 - game.moveIndex / 32.0f;
            default:
                return 0;
        }
    }
}