using UnityEngine;

public class Line
{
    public Orientation Orientation { get; set; }
    public Vector2Int Coordinates { get; set; }

    public Line(Orientation orientation, Vector2Int coordinates)
    {
        Orientation = orientation;
        Coordinates = coordinates;
    }
}

public enum Orientation
{
    Horizontal = 0,
    Vertical = 1
}