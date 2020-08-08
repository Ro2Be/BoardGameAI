using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using UnityEngine;

public abstract class Game : MonoBehaviour
{
    public GameAgent[] players = new GameAgent[2];

    [HideInInspector]
    public int moveIndex = -1;

    [HideInInspector]
    public Board board;

    protected Position[] moves;

    protected UserInterface userInterface;

    public GameAgent activePlayer
        => players[moveIndex % 2];

    public abstract List<Position> GetPossibleMoves();

    public abstract bool GetIsWin(GameAgent gameAgent);

    public abstract bool GetIsDraw();

    protected virtual void Awake()
    {
        players[0].game = this;
        players[0].opponent = players[1];
        players[1].game = this;
        players[1].opponent = players[0];
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
        moveIndex = 0;
        board.Clear();
        userInterface?.Clear();

        players[0].OnGameBegin();
        players[1].OnGameBegin();

        StartCoroutine(NextMove());
    }

    public void HandleInput(Position position)
    {
        if (activePlayer.controlState == GameAgent.ControlState.human
         && GetPossibleMoves().Contains(position))
            DoMove(position);
    }

    public virtual void DoMove(Position position)
    {
        board.SetState(position, activePlayer.behaviorParameters.TeamId);
        userInterface?.DoMove(position, activePlayer.behaviorParameters.TeamId);
        moves[moveIndex++] = position;

        if (GetIsWin(players[0]))
        {
            Debug.Log($"Player -1 wins");
            players[0].SetReward(players[0].GetReward(GameAgent.GameState.win));
            players[1].SetReward(players[1].GetReward(GameAgent.GameState.loss));
            StartCoroutine(End());
            return;
        }
        if (GetIsWin(players[1]))
        {
            Debug.Log($"Player +1 wins");
            players[0].SetReward(players[0].GetReward(GameAgent.GameState.loss));
            players[1].SetReward(players[1].GetReward(GameAgent.GameState.win));
            StartCoroutine(End());
            return;
        }
        if (GetIsDraw())
        {
            Debug.Log($"Draw");
            players[0].SetReward(players[0].GetReward(GameAgent.GameState.draw));
            players[1].SetReward(players[1].GetReward(GameAgent.GameState.draw));
            StartCoroutine(End());
            return;
        }

        players[0].OnGameMove(position);
        players[1].OnGameMove(position);

        StartCoroutine(NextMove());
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
        switch (activePlayer.controlState)
        {
            case GameAgent.ControlState.agent:
                activePlayer.RequestMove();
                break;
            case GameAgent.ControlState.human:
                //wait for input
                break;
            case GameAgent.ControlState.random:
                DoMove(GetPossibleMoves().GetRandom());
                break;
        }
        yield break;
    }

    protected IEnumerator End()
    {
        yield return new WaitForSeconds(1.0f);
        players[0].isReady = false; //Avoid buggy random OnEpisodeBegin calls
        players[1].isReady = false; //Avoid buggy random OnEpisodeBegin calls
        players[0].EndEpisode();
        players[1].EndEpisode();
        yield break;
    }
}