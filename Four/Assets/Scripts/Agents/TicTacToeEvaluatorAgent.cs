public class TicTacToeEvaluatorAgent : EvaluatorAgent
{
    public override float GetReward(GameState gameState)
    {
        switch(gameState)
        {
            case GameState.win:
                return 1 - 0.5f * game.moveIndex / 9.0f;
            case GameState.draw:
                return -0.01f;
            case GameState.loss:
                return -1 + 0.5f * game.moveIndex / 9.0f;
            default:
                return 0;
        }
    }
}