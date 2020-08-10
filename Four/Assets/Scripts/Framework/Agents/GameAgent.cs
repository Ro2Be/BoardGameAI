using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;

public abstract class GameAgent : Agent
{
    public enum GameState { playing, win, draw, loss };
    public enum ControlState { agent, human, random };

    public ControlState controlState;

    [HideInInspector]
    public bool isReady = false;

    [HideInInspector]
    public int id;

    [HideInInspector]
    public Game game;

    [HideInInspector]
    public GameAgent opponent;

    public abstract void RequestMove();

    public abstract float GetReward(GameState gameState);

    public virtual void OnGameBegin() { }

    public virtual void OnGameMove(Position move) { }

    public override void OnEpisodeBegin()
    {
        //Avoid buggy random OnEpisodeBegin calls
        if (isReady == true)
        {
            Debug.Log("MLAgents bug: bad OnEpisodeBegin call");
        }
        else
        {
            isReady = true;
            if (game.players[0].isReady && game.players[1].isReady)
                game.Begin();
        }
    }
}