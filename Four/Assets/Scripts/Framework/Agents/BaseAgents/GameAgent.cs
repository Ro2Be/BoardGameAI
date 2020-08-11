using UnityEngine;

public class GameAgent
{
    public enum Player { agent, human, random };
}

public interface IGameAgent
{
    [HideInInspector]
    bool isReady { get; set; }

    [HideInInspector]
    int id { get; set; }

    [HideInInspector]
    Game game { get; set; }

    [HideInInspector]
    IGameAgent opponent { get; set; }

    GameAgent.Player GetPlayer();

    string GetName();

    void RequestMove();

    void HandleOnGameBegin();

    void HandleOnGameMove(Position move);

    void HandleOnGameEnd(Game.State gameState);
}