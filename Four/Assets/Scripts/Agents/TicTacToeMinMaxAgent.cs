﻿public class TicTacToeMinMaxAgent : MiniMaxAgent
{
    public override float GetReward(Game.State gameState, Board board, GameAgent gameAgent)
    {
        switch (gameState)
        {
            case Game.State.win:
                return +1 - game.moveIndex / 10f;
            case Game.State.draw:
                return 0.01f;
            case Game.State.loss:
                return -1 + game.moveIndex / 10f;
            default:
                return 0;
        }
    }
}