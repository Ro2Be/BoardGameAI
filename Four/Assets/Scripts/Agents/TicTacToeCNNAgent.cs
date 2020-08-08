public class TicTacToeCNNAgent : ActorAgent
{
    protected override Position GetMove(float[] vectorAction)
        => new Position((int)vectorAction[0] % 3, (int)vectorAction[0] / 3);

    public override void OnGameMove(Position move)
        => actionMask.Add(move.x + 3 * move.y);

    public override float GetReward(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.win:
                return +(1 - (game.moveIndex - 4) / 5f);
            case GameState.draw:
                return 0.1f * behaviorParameters.TeamId;
            case GameState.loss:
                return -(1 - (game.moveIndex - 4) / 5f);
            default:
                return 0;
        }
    }
}