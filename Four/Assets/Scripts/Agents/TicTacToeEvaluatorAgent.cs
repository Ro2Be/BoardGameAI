public class TicTacToeEvaluatorAgent : EvaluatorAgent
{
    public override float GetReward(Game.State gameState, Board board, GameAgent gameAgent)
    {
        switch(gameState)
        {
            case Game.State.win:
                return 1 - 0.5f * game.moveIndex / 9.0f;
            case Game.State.draw:
                return 0.01f;
            case Game.State.loss:
                return -1 + 0.5f * game.moveIndex / 9.0f;
            default:
                return 0;
        }
    }
}