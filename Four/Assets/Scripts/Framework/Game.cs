﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Game : MonoBehaviour
{
    public enum State { playing, win, draw, loss };

    public GameObject[] playersGameObjects = new GameObject[2];

    [SerializeField]
    protected bool isTrainingGame = true;

    [SerializeField]
    protected bool randomStartingPlayer = true;

    [HideInInspector]
    public IGameAgent[] players = new IGameAgent[2];

    [HideInInspector]
    public Board board;

    protected Position[] moves;

    protected UserInterface userInterface;

    private ScoreBoard scoreBoard = new ScoreBoard();

    public IGameAgent activeGameAgent;

    public abstract List<Position> GetPossibleMoves(Board board, IGameAgent gameAgent);

    public abstract State GetState(Board board, IGameAgent gameAgent);

    public abstract float GetScore(Board board, IGameAgent gameAgent, State gameState);

    protected virtual void Awake()
    {
        players = new IGameAgent[playersGameObjects.Length];
        for (int i = 0; i < playersGameObjects.Length; ++i)
        {
            AlgorithmAgent algorithmAgent = playersGameObjects[i].GetComponent<AlgorithmAgent>();
            if (algorithmAgent)
                players[i] = algorithmAgent;
            MLAgent MLAgent = playersGameObjects[i].GetComponent<MLAgent>();
            if (MLAgent)
                players[i] = MLAgent;
            TwitchAgent TwitchAgent = playersGameObjects[i].GetComponent<TwitchAgent>();
            if (TwitchAgent)
                players[i] = TwitchAgent;
        }

        scoreBoard.RegisterPlayers(players);
        for (int i = 0; i < players.Length; ++i)
            players[i].game = this;
        userInterface = FindObjectOfType<UserInterface>();
        StartCoroutine(Begin());
    }

    protected void Update()
    {
        //Player can revert move using right mouse click
        if (0 < board.moveIndex && Input.GetMouseButtonDown(1))
            UndoMove();
    }

    public void HandleInput(Position position)
    {
        if (activeGameAgent.GetPlayer() == GameAgent.Player.human
         && GetPossibleMoves(board, activeGameAgent).Contains(position))
            DoMove(position);
    }

    public virtual void DoMove(Position position)
    {
        moves[board.moveIndex] = position;
        board.SetState(position, activeGameAgent.id);
        board.moveIndex++;
        userInterface?.DoMove(position, activeGameAgent.id);
        switch (GetState(board, activeGameAgent))
        {
            case State.win:
                scoreBoard.RegisterWin(activeGameAgent);
                activeGameAgent.HandleOnGameEnd(State.win);
                activeGameAgent.opponent.HandleOnGameEnd(State.loss);
                StartCoroutine(End());
                break;
            case State.loss:
                scoreBoard.RegisterWin(activeGameAgent.opponent);
                activeGameAgent.HandleOnGameEnd(State.loss);
                activeGameAgent.opponent.HandleOnGameEnd(State.win);
                StartCoroutine(End());
                break;
            case State.draw:
                scoreBoard.RegisterWin(null);
                players[0].HandleOnGameEnd(State.draw);
                players[1].HandleOnGameEnd(State.draw);
                StartCoroutine(End());
                break;
            case State.playing:
                players[0].HandleOnGameMove(position);
                players[1].HandleOnGameMove(position);
                NextMove();
                break;
        }
    }

    protected void UndoMove()
    {
        Position position = moves[board.moveIndex];
        board.SetState(position, 0);
        --board.moveIndex;
        userInterface?.UndoMove(position);
        activeGameAgent = activeGameAgent.opponent;
    }

    protected IEnumerator Begin()
    {
        if (randomStartingPlayer)
        { /* randomize players */
            int playerIndex0 = Random.Range(0, players.Length);
            players.Swap(0, playerIndex0);
            int playerIndex1 = Random.Range(0, players.Length);
            while (playerIndex0 == playerIndex1)
                playerIndex1 = Random.Range(0, players.Length);
            players.Swap(1, playerIndex1);
        }

        yield return new WaitUntil(() => players[0].isReady && players[1].isReady);

        board.Clear();
        userInterface?.Clear();
        activeGameAgent = players[0];

        players[0].id = -1;
        players[1].id = +1;
        players[0].opponent = players[1];
        players[1].opponent = players[0];
        players[0].HandleOnGameBegin();
        players[1].HandleOnGameBegin();

        NextMove();
    }

    protected void NextMove()
    {
        if (isTrainingGame)
        {
            activeGameAgent = activeGameAgent.opponent;
            switch (activeGameAgent.GetPlayer())
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
        }
        else
            StartCoroutine(NextMoveCoroutine());
    }

    protected IEnumerator NextMoveCoroutine()
    {
        activeGameAgent = activeGameAgent.opponent;
        yield return new WaitForSeconds(0.1f);
        switch (activeGameAgent.GetPlayer())
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
        if(!isTrainingGame)
            yield return new WaitForSeconds(1.0f);
        yield return Begin();
    }
}