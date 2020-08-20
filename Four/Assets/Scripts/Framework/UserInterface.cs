using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class UserInterface : MonoBehaviour
{
    public Position input { get; private set; }

    [SerializeField]
    private Square squarePrefab;

    [SerializeField]
    private Game game;

    [HideInInspector]
    [SerializeField]
    private GridLayoutGroup gridLayoutGroup;

    private Size size;

    private Square[,] squares;

    public void DoMove(Position position, int player)
        => squares[position.x, position.y].Place(player);

    public void UndoMove(Position position)
        => squares[position.x, position.y].Clear();

    public void Clear()
    {
        for (Position position = new Position(0, 0); position.x < size.x; ++position.x)
            for (position.y = 0; position.y < size.y; ++position.y)
                squares[position.x, position.y].Clear();
    }

    protected void Reset()
        => gridLayoutGroup = GetComponent<GridLayoutGroup>();

    protected void Awake()
    {
        size = game.board.size;

        Rect rect = (transform as RectTransform).rect;
        gridLayoutGroup.startCorner = GridLayoutGroup.Corner.LowerLeft;
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = size.x;
        gridLayoutGroup.cellSize = Vector2.one * Mathf.Min(rect.width / size.x, rect.height / size.y);

        squares = new Square[size.x, size.y];
        for (Position position = new Position(0, 0); position.y < size.y; ++position.y)
            for (position.x = 0; position.x < size.x; ++position.x)
            {
                Position positionCopy = new Position(position.x, position.y);
                squares[position.x, position.y] = Instantiate(squarePrefab, transform).Init(() => game.HandleInput(positionCopy));
            }
    }
}