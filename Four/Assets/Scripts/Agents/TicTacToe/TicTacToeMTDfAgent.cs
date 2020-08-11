public class TicTacToeMTDfAgent : MTDfAgent
{
    public override float GuessScore(Board board, IGameAgent gameAgent)
    {
        return +1 - board.moveIndex / 10f;
    }
}