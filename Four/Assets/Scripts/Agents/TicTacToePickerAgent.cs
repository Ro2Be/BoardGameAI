﻿using System.Collections.Generic;

public class TicTacToePickerAgent : ActorAgent
{
    List<int> actionMask = new List<int>();

    protected override Position GetMove(float[] vectorAction)
        => new Position((int)vectorAction[0] % 3, (int)vectorAction[0] / 3);

    protected override List<int> GetActionMask()
    => actionMask;

    public override void OnGameBegin()
        => actionMask.Clear();

    public override void OnGameMove(Position move)
        => actionMask.Add(move.x + 3 * move.y);

    public override float GetReward(Game.State gameState, Board board, GameAgent gameAgent)
    {
        switch (gameState)
        {
            case Game.State.win:
                return +(1 - (game.moveIndex - 4) / 5f);
            case Game.State.draw:
                return 0.1f;
            case Game.State.loss:
                return -(1 - (game.moveIndex - 4) / 5f);
            default:
                return 0;
        }
    }
}