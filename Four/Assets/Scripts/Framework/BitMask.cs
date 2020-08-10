/// <summary>
/// 8x8 bitmask
/// </summary>
public class BitMask
{
    /// <summary>
    /// masks for columns
    /// </summary>
    private static readonly ulong[] columnMasks = new ulong[8]
    {
        0b_00000001_00000001_00000001_00000001_00000001_00000001_00000001_00000001,
        0b_00000010_00000010_00000010_00000010_00000010_00000010_00000010_00000010,
        0b_00000100_00000100_00000100_00000100_00000100_00000100_00000100_00000100,
        0b_00001000_00001000_00001000_00001000_00001000_00001000_00001000_00001000,
        0b_00010000_00010000_00010000_00010000_00010000_00010000_00010000_00010000,
        0b_00100000_00100000_00100000_00100000_00100000_00100000_00100000_00100000,
        0b_01000000_01000000_01000000_01000000_01000000_01000000_01000000_01000000,
        0b_10000000_10000000_10000000_10000000_10000000_10000000_10000000_10000000
    };

    /// <summary>
    /// masks for rows
    /// </summary>
    private static readonly ulong[] rowMasks = new ulong[8]
    {
        0b_00000000_00000000_00000000_00000000_00000000_00000000_00000000_11111111,
        0b_00000000_00000000_00000000_00000000_00000000_00000000_11111111_00000000,
        0b_00000000_00000000_00000000_00000000_00000000_11111111_00000000_00000000,
        0b_00000000_00000000_00000000_00000000_11111111_00000000_00000000_00000000,
        0b_00000000_00000000_00000000_11111111_00000000_00000000_00000000_00000000,
        0b_00000000_00000000_11111111_00000000_00000000_00000000_00000000_00000000,
        0b_00000000_11111111_00000000_00000000_00000000_00000000_00000000_00000000,
        0b_11111111_00000000_00000000_00000000_00000000_00000000_00000000_00000000
    };

    /// <summary>
    /// Bits of the mask
    /// </summary>
    public ulong bits;

    /// <summary>
    /// Size of the mask
    /// </summary>
    public Size size;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="bits"></param>
    /// <param name="size">Max 8x8</param>
    public BitMask(ulong bits, Size size)
    {
        this.bits = bits;
        this.size = size;
    }

    /// <summary>
    /// Get bit at position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool GetBit(Position position)
        => (bits & GetBits(position)) != 0;

    /// <summary>
    /// Get bit at position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="bit"></param>
    public bool GetBit(int x, int y)
        => (bits & GetBitMask(x, y)) != 0;

    /// <summary>
    /// Set bit at position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="bit"></param>
    public void SetBit(int x, int y, bool bit)
    {
        if (bit)
            bits |= GetBitMask(x, y);
        else
            bits &= ~GetBitMask(x, y);
    }

    /// <summary>
    /// Set bit at position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="bit"></param>
    public void SetBit(Position position, bool bit)
    {
        if (bit)
            bits |= GetBitMask(position.x, position.y);
        else
            bits &= ~GetBitMask(position.x, position.y);
    }

    /// <summary>
    /// Mirror the bit mask horizontally
    /// </summary>
    public void MirrorHorizontally() // |
    {
        for (int x = 0; x < size.x / 2; ++x)
        {
            ulong column0 = bits & columnMasks[x];
            ulong column1 = bits & columnMasks[size.x - x - 1];
            bits &= ~(column0 | column1);
            column0 <<= (size.x - 2 * x - 1);
            column1 >>= (size.x - 2 * x - 1);
            bits |= column0 | column1;
        }
    }

    /// <summary>
    /// Mirror the bit mask vertically
    /// </summary>
    public void MirrorVertically() // -
    {
        for (int y = 0; y < size.y / 2; ++y)
        {
            ulong row0 = bits & rowMasks[y];
            ulong row1 = bits & rowMasks[size.y - y - 1];
            bits &= ~(row0 | row1);
            row0 <<= 8 * (size.y - 2 * y - 1);
            row1 >>= 8 * (size.y - 2 * y - 1);
            bits |= row0 | row1;
        }
    }

    /// <summary>
    /// Mirror the bit mask over the (0, 0)-(8, 8) diagonal
    /// </summary>
    public void MirrorDiagonally() // /
    {
        for (int x = 1; x < (size.x < size.y ? size.x : size.y); ++x)
            for (int y = 0; y < x; ++y)
            {
                bool xy = GetBit(x, y);
                bool yx = GetBit(y, x);
                SetBit(x, y, yx);
                SetBit(y, x, xy);
            }
    }

    /// <summary>
    /// Get bitMask for a certain position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static ulong GetBitMask(int x, int y)
        => columnMasks[x] & rowMasks[y];

    /// <summary>
    /// Get bitMask for a certain position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static ulong GetBits(Position position)
        => columnMasks[position.x] & rowMasks[position.y];
}