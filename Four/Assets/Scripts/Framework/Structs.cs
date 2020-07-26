using System;

/// <summary>
/// Board position
/// </summary>
[Serializable]
public struct Position
{
    public int x;
    public int y;
    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

/// <summary>
/// Board size
/// </summary>
[Serializable]
public struct Size
{
    public int x;
    public int y;
    public Size(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}