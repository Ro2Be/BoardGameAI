using UnityEngine;

public class HeatMap
{
    private int positionsToBeSet;

    private float[,] positions;

    public HeatMap(Size size)
    {
        this.positions = new float[size.x, size.y];
        this.positionsToBeSet = positions.Length;
        Clear();
    }

    public void Clear()
    {
        for (Position position = new Position(0, 0); position.x < positions.GetLength(0); ++position.x)
            for (position.y = 0; position.y < positions.GetLength(1); ++position.y)
                positions[position.x, position.y] = float.MinValue;
        positionsToBeSet = positions.Length;
    }

    public void SetHeat(Position position, float heat)
    {
        positions[position.x, position.y] = heat;
        --positionsToBeSet;
    }

    public float GetHeat(Position position)
        => positions[position.x, position.y];

    public Position GetHottestPosition()
    {
        Position hottestPosition = new Position(-1, -1);
        float hottestPositionHeat = float.MinValue;
        if (positionsToBeSet <= 0)
        {
            if(positionsToBeSet < 0)
            {
                Debug.Log($"HeatMap.GetHottestPosition(): PositionsToBeSet == {positionsToBeSet}");
                Log();
            }
            for (Position position = new Position(0, 0); position.x < positions.GetLength(0); ++position.x)
                for (position.y = 0; position.y < positions.GetLength(1); ++position.y)
                {
                    float heat = positions[position.x, position.y];
                    if (hottestPositionHeat < heat)
                    {
                        hottestPosition = position;
                        hottestPositionHeat = heat;
                    }
                }
        }
        else
        {
            Debug.Log($"HeatMap.GetHottestPosition(): PositionsToBeSet == {positionsToBeSet}");
            Log();
        }
        return hottestPosition;
    }

    public void Log()
    {
        string s = "HeatMap\n";
        for (Position position = new Position(0, positions.GetLength(1) - 1); 0 <= position.y; --position.y)
        {
            for (position.x = 0; position.x < positions.GetLength(0); ++position.x)
                s += $"{ positions[position.x, position.y].ToString("E1")}\t";
            s += "\n";
        }
        Debug.Log(s);
    }
}