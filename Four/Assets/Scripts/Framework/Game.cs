using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Game : MonoBehaviour
{
    public enum State { playing, win, draw, loss };

    public GameAgent[] players = new GameAgent[2];

    [HideInInspector]
    public int moveIndex = -1;

    [HideInInspector]
    public Board board;

    protected Position[] moves;

    protected UserInterface userInterface;

    private ScoreBoard scoreBoard = new ScoreBoard();

    public GameAgent activeGameAgent
        => players[moveIndex % 2];

    public abstract List<Position> GetPossibleMoves(Board board, GameAgent gameAgent);

    public abstract State GetState(Board board, GameAgent gameAgent);

    protected virtual void Awake()
    {
        scoreBoard.RegisterPlayers(players);
        for (int i = 0; i < players.Length; ++i)
            players[i].game = this;
        userInterface = FindObjectOfType<UserInterface>();
    }

    protected void Update()
    {
        //Player can revert move using right mouse click
        if (0 < moveIndex && Input.GetMouseButtonDown(1))
            UndoMove();
    }

    public virtual void Begin()
    {
        /* rotate players */
        GameAgent lastPlayer = players[0];
        for (int i = 1; i < players.Length; ++i)
            players[i - 1] = players[i];
        players[players.Length - 1] = lastPlayer;
        /*----------------*/

        moveIndex = 0;
        board.Clear();
        userInterface?.Clear();

        players[0].id = -1;
        players[1].id = +1;
        players[0].opponent = players[1];
        players[1].opponent = players[0];
        players[0].OnGameBegin();
        players[1].OnGameBegin();

        StartCoroutine(NextMove());
    }

    public void HandleInput(Position position)
    {
        if (activeGameAgent.controlState == GameAgent.Player.human
         && GetPossibleMoves(board, activeGameAgent).Contains(position))
            DoMove(position);
    }

    public virtual void DoMove(Position position)
    {
        board.SetState(position, activeGameAgent.id);
        userInterface?.DoMove(position, activeGameAgent.id);
        moves[moveIndex++] = position;

        switch (GetState(board, activeGameAgent))
        {
            case State.win:
                scoreBoard.RegisterWin(activeGameAgent);
                activeGameAgent.SetReward(activeGameAgent.GetReward(State.win, board, activeGameAgent));
                activeGameAgent.opponent.SetReward(activeGameAgent.opponent.GetReward(State.loss, board, activeGameAgent));
                StartCoroutine(End());
                break;
            case State.loss:
                scoreBoard.RegisterWin(activeGameAgent.opponent);
                activeGameAgent.opponent.SetReward(activeGameAgent.opponent.GetReward(State.win, board, activeGameAgent));
                activeGameAgent.SetReward(activeGameAgent.GetReward(State.loss, board, activeGameAgent));
                StartCoroutine(End());
                break;
            case State.draw:
                scoreBoard.RegisterWin(null);
                players[0].SetReward(players[0].GetReward(State.draw, board, players[0]));
                players[1].SetReward(players[1].GetReward(State.draw, board, players[1]));
                StartCoroutine(End());
                break;
            case State.playing:
                players[0].OnGameMove(position);
                players[1].OnGameMove(position);
                StartCoroutine(NextMove());
                break;
        }
    }

    protected void UndoMove()
    {
        Position position = moves[--moveIndex];
        board.SetState(position, 0);
        userInterface?.UndoMove(position);
    }

    protected IEnumerator NextMove()
    {
        yield return new WaitForSeconds(0.1f);
        switch (activeGameAgent.controlState)
        {
            case GameAgent.Player.agent:
                activeGameAgent.RequestMove();
                break;
            case GameAgent.Player.human:
                //wait for input
                break;
            case GameAgent.Player.random:
                DoMove(GetPossibleMoves(board, activeGameAgent).GetRandom());
                break;
        }
        yield break;
    }

    protected IEnumerator End()
    {
        //yield return new WaitForSeconds(1.0f);
        players[0].isReady = false; //Avoid buggy random OnEpisodeBegin calls
        players[1].isReady = false; //Avoid buggy random OnEpisodeBegin calls
        players[0].EndEpisode();
        players[1].EndEpisode();
        yield break;
    }
}