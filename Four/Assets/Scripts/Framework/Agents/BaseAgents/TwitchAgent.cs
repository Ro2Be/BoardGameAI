using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class TwitchAgent : MonoBehaviour, IGameAgent
{
    private TcpClient twitchClient;
    private StreamReader streamReader;
    private StreamWriter streamWriter;

    private string username = "ro2be";
    private string password = "oauth:8lffcqh24979ftgy0ufqs3l188vqrm";
    private string channelName = "ro2be";

    [SerializeField]
    private GameAgent.Player player = GameAgent.Player.agent;
    public GameAgent.Player GetPlayer() => player;
    public bool isReady { get; set; } = true;
    public int id { get; set; }
    public Game game { get; set; }
    public IGameAgent opponent { get; set; }

    void Awake()
        => ConnectToTwitch();

    void OnApplicationQuit()
    {
        streamReader?.Close();
        streamWriter?.Close();
    }

    private void ConnectToTwitch()
    {
        twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
        twitchClient.NoDelay = true;
        streamReader = new StreamReader(twitchClient.GetStream());
        streamWriter = new StreamWriter(twitchClient.GetStream());
        streamWriter.WriteLine("PASS " + password);
        streamWriter.WriteLine("NICK " + username);
        streamWriter.WriteLine("USER " + username + " 8 * :" + username);
        streamWriter.WriteLine("JOIN #" + channelName);
        streamWriter.Flush();
    }

    public string GetName() => name;

    public void RequestMove()
        => StartCoroutine(RequestMoveCoroutine());

    protected IEnumerator RequestMoveCoroutine()
    {
        List<Position> possibleMoves = game.GetPossibleMoves(game.board, this);
        Position move = new Position(-1, -1);
        while (!possibleMoves.Contains(move))
        {
            while(!twitchClient.Connected || 0 == twitchClient.Available)
            {
                if (!twitchClient.Connected)
                    ConnectToTwitch();
                yield return new WaitUntil(() => !twitchClient.Connected || 0 < twitchClient.Available);
            };

            string line = streamReader.ReadLine();
            int separator = line.IndexOf(":", 1);
            line = line.Substring(separator + 1);
            if (line.Length == 2)
                move = new Position(char.ToUpper(line[0]) - 65, line[1] - 49);
        }
        game.DoMove(move);
    }

    public virtual void HandleOnGameBegin() { }
    public virtual void HandleOnGameMove(Position move) { }
    public virtual void HandleOnGameEnd(Game.State gameState) { }
}