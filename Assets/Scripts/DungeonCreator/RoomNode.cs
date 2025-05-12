using System.Numerics;
using UnityEngine;

public class RoomNode : AbstractNode
{
    public RoomNode(
        Vector2Int bottomLeftCorner,
        Vector2Int topRightCorner,
        int index,
        AbstractNode parentNode) : base(parentNode)
    {
        BottomLeftCorner = bottomLeftCorner;
        TopRightCorner = topRightCorner;
        BottomRightCorner = new Vector2Int(topRightCorner.x, bottomLeftCorner.y);
        TopLeftCorner = new Vector2Int(bottomLeftCorner.x, topRightCorner.y);
        TreeIndex = index;
    }

    public int Width { get => TopRightCorner.x - BottomLeftCorner.x; }
    public int Height { get => TopRightCorner.y - BottomLeftCorner.y; }

    public RoomType Type { get; set; } = RoomType.NormalRoom;
}

public enum RoomType
{
    StartingRoom,
    ExitRoom,
    NormalRoom
}