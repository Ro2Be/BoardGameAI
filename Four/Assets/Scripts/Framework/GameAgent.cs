using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;

public abstract class GameAgent : Agent
{
    [SerializeField]
    protected GameAgent opponent;

    [SerializeField]
    protected bool doRandomMoves;

    [HideInInspector]
    public BehaviorParameters behaviorParameters;

    protected static GameAgent activeAgent;
    protected static Board board;
    protected static int moveIndex = -1;
    protected static Position humanMove;

    private static bool isInitialized = false;
    private static Position[] moves;
    private static UserInterface userInterface;

    public abstract void RequestMove();

    public static void HandleInput(Position position)
    {
        if (activeAgent.behaviorParameters.BehaviorType == BehaviorType.HeuristicOnly)
        {
            humanMove = position;
            activeAgent.RequestMove();
        }
    }

    public override void Initialize()
    {
        if (!isInitialized)
        {
            userInterface = FindObjectOfType<UserInterface>();
            board = new Board(userInterface.size);
            moves = new Position[board.size.x * board.size.y];
            isInitialized = true;
        }
        behaviorParameters = GetComponent<BehaviorParameters>();
    }

    public override void OnEpisodeBegin()
    {
        if (behaviorParameters.TeamId == -1)
            activeAgent = this;
        if(moveIndex == 0)
        {
            if (activeAgent.behaviorParameters.BehaviorType != BehaviorType.HeuristicOnly)
                activeAgent.RequestMove();
        }
        else
        {
            moveIndex = 0;
            board.Clear();
            userInterface.Clear();
        }
    }

    protected void Update()
    {
        //Player can revert move using right mouse click
        if (behaviorParameters.TeamId == -1 && Input.GetMouseButtonDown(1) && 0 < moveIndex)
            UndoMove();
    }

    protected void DoMove(Position position, int player)
    {
        moves[moveIndex++] = position;
        board.SetState(position, player);
        userInterface.DoMove(position, player);
        activeAgent = activeAgent.opponent;
    }

    protected void UndoMove()
    {
        Position position = moves[--moveIndex];
        board.SetState(position, 0);
        userInterface.UndoMove(position);
        activeAgent = activeAgent.opponent;
    }
}