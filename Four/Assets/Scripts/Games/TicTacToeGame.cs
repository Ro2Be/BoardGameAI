﻿using System.Collections.Generic;
using UnityEngine;

public class TicTacToeGame : Game
{
    private const ulong fullBoardBits = 0b_00000111_00000111_00000111;

    private readonly BitMask[] winMasks = new BitMask[4]
    {
        new BitMask(0b_00000111, new Size(3, 1)),                   // -
        new BitMask(0b_00000001_00000001_00000001, new Size(1, 3)), // |
        new BitMask(0b_00000001_00000010_00000100, new Size(3, 3)), // /
        new BitMask(0b_00000100_00000010_00000001, new Size(3, 3))  // \
    };

    protected override void Awake()
    {
        base.Awake();
        board = new Board(new Size(3, 3));
        moves = new Position[board.size.x * board.size.y];
    }

    public override List<Position> GetPossibleMoves(Board board, GameAgent gameAgent)
    {
        List<Position> possibleMoves = new List<Position>();
        for (Position position = new Position(0, 0); position.x < board.size.x; ++position.x)
            for (position.y = 0; position.y < board.size.y; ++position.y)
                if (board.GetState(position) == 0)
                    possibleMoves.Add(position);
        return possibleMoves;
    }

    public override State GetState(Board board, GameAgent gameAgent)
    {
        if (((board.GetBitMask(-1).bits | board.GetBitMask(+1).bits) & fullBoardBits) == fullBoardBits)
            return State.draw;

        for (int winMaskIndex = 0; winMaskIndex < winMasks.Length; ++winMaskIndex)
            for (Position position = new Position(0, 0); position.x <= board.size.x - winMasks[winMaskIndex].size.x; ++position.x)
                for (position.y = 0; position.y <= board.size.y - winMasks[winMaskIndex].size.y; ++position.y)
                {
                    ulong winBits = winMasks[winMaskIndex].bits * BitMask.GetBits(position);
                    if ((board.GetBitMask(gameAgent.id).bits & winBits) == winBits)
                        return State.win;
                    if ((board.GetBitMask(gameAgent.opponent.id).bits & winBits) == winBits)
                        return State.loss;
                }

        return State.playing;
    }
}