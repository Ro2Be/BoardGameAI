using Unity.MLAgents;
using UnityEngine;

public abstract class MLAgent : Agent, IGameAgent
{
    [SerializeField]
    private GameAgent.Player player;
    public GameAgent.Player GetPlayer() => player;
    public bool isReady { get; set; } = false;
    public int id { get; set; }
    public Game game { get; set; }
    public IGameAgent opponent { get; set; }

    public string GetName() => name;
    public abstract void RequestMove();
    public abstract float GetReward(Game.State gameState, Board board, IGameAgent gameAgent);
    public virtual void HandleOnGameBegin() { }
    public virtual void HandleOnGameMove(Position move) { }
    public virtual void HandleOnGameEnd(Game.State gameState)
    {
        SetReward(GetReward(gameState, game.board, this));
        isReady = false; //Avoid buggy random OnEpisodeBegin calls
        EndEpisode();
    }

    public override void OnEpisodeBegin()
    {        
        //Avoid buggy random OnEpisodeBegin calls
        if (isReady == true)
            Debug.Log("MLAgents bug: bad OnEpisodeBegin call");
        else
            isReady = true;
    }
}