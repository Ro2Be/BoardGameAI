using System.Collections.Generic;
using Unity.MLAgents.Sensors;

public class TicTacToeCNNAgent : ActorAgent
{
    List<int> actionMask = new List<int>();

    private readonly BitMask[] convolutionalLayers = new BitMask[8]
    {
        new BitMask(0b_00000011, new Size(2, 1)),          // -
        new BitMask(0b_00000001_00000001, new Size(1, 2)), // |
        new BitMask(0b_00000001_00000010, new Size(2, 2)), // /
        new BitMask(0b_00000010_00000001, new Size(2, 2)),  // \
        new BitMask(0b_00000101, new Size(3, 1)),
        new BitMask(0b_00000001_00000000_00000001, new Size(1, 3)),
        new BitMask(0b_00000100_00000000_00000001, new Size(3, 3)),
        new BitMask(0b_00000001_00000000_00000100, new Size(3, 3))
    };

    public override void CollectObservations(VectorSensor sensor)
    {
        //Make the agent aware whether a certain position is claimed by a player (2x9)
        for (Position position = new Position(0, 0); position.x < game.board.size.x; ++position.x)
            for (position.y = 0; position.y < game.board.size.y; ++position.y)
            {
                sensor.AddObservation(game.board.GetState(position) == -id);
                sensor.AddObservation(game.board.GetState(position) == +id);
            }

        //Make the agent aware of all two in a rows + three in a rows with missing center (2x28 extra inputs)
        for (int i = 0; i < convolutionalLayers.Length; ++i)
            for (Position position = new Position(0, 0); position.x <= game.board.size.x - convolutionalLayers[i].size.x; ++position.x)
                for (position.y = 0; position.y <= game.board.size.y - convolutionalLayers[i].size.y; ++position.y)
                {
                    ulong mask = convolutionalLayers[i].bits * BitMask.GetBits(position);
                    sensor.AddObservation((game.board.GetBitMask(-id).bits & mask) == mask);
                    sensor.AddObservation((game.board.GetBitMask(+id).bits & mask) == mask);
                }
    }

    protected override Position GetMove(float[] vectorAction)
        => new Position((int)vectorAction[0] % 3, (int)vectorAction[0] / 3);

    protected override List<int> GetActionMask()
        => actionMask;

    public override void HandleOnGameBegin()
    {
        base.HandleOnGameBegin();
        actionMask.Clear();
    }

    public override void HandleOnGameMove(Position move)
        => actionMask.Add(move.x + 3 * move.y);

    public override float GetReward(Game.State gameState, Board board, IGameAgent gameAgent)
    {
        switch (gameState)
        {
            case Game.State.win:
                return +(1 - (board.moveIndex - 4) / 5f);
            case Game.State.draw:
                return 0.01f;
            case Game.State.loss:
                return -(1 - (board.moveIndex - 4) / 5f);
            default:
                return 0;
        }
    }
}