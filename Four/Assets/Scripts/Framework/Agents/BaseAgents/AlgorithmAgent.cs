using UnityEngine;

public abstract class AlgorithmAgent : MonoBehaviour, IGameAgent
{
    [SerializeField]
    private GameAgent.Player player;
    [SerializeField]
    protected int maximumSearchDepth = int.MaxValue;
    public GameAgent.Player GetPlayer() => player;
    public bool isReady { get; set; } = true;
    public int id { get; set; }
    public Game game { get; set; }
    public IGameAgent opponent { get; set; }

    public string GetName() => name;
    public abstract void RequestMove();
    public virtual void HandleOnGameBegin() { }
    public virtual void HandleOnGameMove(Position move) { }
    public virtual void HandleOnGameEnd(Game.State gameState) { }

    public float GetReward(Game.State gameState, Board board, IGameAgent gameAgent)
    {
        Debug.LogError("AlgorithmAgent.GetReward(...): this function should not be used");
        return 0;
    }
}
