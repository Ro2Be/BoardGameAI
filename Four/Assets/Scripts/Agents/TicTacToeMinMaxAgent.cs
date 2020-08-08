public class TicTacToeMinMaxAgent : MinMaxAgent
{
    public override float GetReward(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.win:
                return +1 - game.moveIndex / 10f;
            case GameState.draw:
                return 0;
            case GameState.loss:
                return -1 + game.moveIndex / 10f;
            default:
                return 0;
        }
    }
}