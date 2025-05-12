using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

internal class BinarySpacePartitioner
{
    public RoomNode RootNode { get; set; }

    public BinarySpacePartitioner(int dungeonWidth, int dungeonHeight)
    {
        RootNode = new RoomNode(new Vector2Int(0, 0), new Vector2Int(dungeonWidth, dungeonHeight), 0, null);
    }

    public List<RoomNode> PrepareRoomNodesList(int maxIterations, int minRoomWidth, int minRoomHeight)
    {
        Queue<RoomNode> queue = new Queue<RoomNode>();
        List<RoomNode> roomNodes = new List<RoomNode>();

        queue.Enqueue(RootNode);
        roomNodes.Add(RootNode);
        int iterations = 0;

        while (iterations < maxIterations && queue.Count > 0)
        {
            iterations++;
            RoomNode currentNode = queue.Dequeue();
            if (currentNode.Width >= minRoomWidth * 2 || currentNode.Height >= minRoomHeight * 2)
            {
                SplitTheSpace(currentNode, roomNodes, minRoomWidth, minRoomHeight, queue);
            }
        }

        return roomNodes;
    }

    private void SplitTheSpace(RoomNode currentNode, List<RoomNode> roomNodes, int minRoomWidth, int minRoomHeight, Queue<RoomNode> queue)
    {
        Line line = GetLineDividingSpace(
            currentNode.BottomLeftCorner,
            currentNode.TopRightCorner,
            minRoomWidth,
            minRoomHeight);

        RoomNode roomNode1, roomNode2;
        if (line.Orientation == Orientation.Horizontal)
        {
            roomNode1 = new RoomNode(
                currentNode.BottomLeftCorner,
                new Vector2Int(currentNode.TopRightCorner.x, line.Coordinates.y),
                currentNode.TreeIndex + 1,
                currentNode);

            roomNode2 = new RoomNode(
                new Vector2Int(currentNode.BottomLeftCorner.x, line.Coordinates.y),
                currentNode.TopRightCorner,
                currentNode.TreeIndex + 1,
                currentNode);
        }
        else
        {
            roomNode1 = new RoomNode(currentNode.BottomLeftCorner,
                new Vector2Int(line.Coordinates.x, currentNode.TopRightCorner.y),
                currentNode.TreeIndex + 1,
                currentNode);
            roomNode2 = new RoomNode(new Vector2Int(line.Coordinates.x, currentNode.BottomLeftCorner.y),
                currentNode.TopRightCorner,
                currentNode.TreeIndex + 1,
                currentNode);
        }

        AddNodesToCollections(roomNodes, queue, roomNode1);
        AddNodesToCollections(roomNodes, queue, roomNode2);
    }

    private void AddNodesToCollections(List<RoomNode> roomNodes, Queue<RoomNode> queue, RoomNode node)
    {
        roomNodes.Add(node);
        queue.Enqueue(node);
    }

    private Line GetLineDividingSpace(Vector2Int bottomLeftCorner, Vector2Int topRightCorner, int minRoomWidth, int minRoomHeight)
    {
        Orientation orientation;
        bool heightStatus = (topRightCorner.y - bottomLeftCorner.y) >= 2 * minRoomHeight;
        bool widthStatus = (topRightCorner.x - bottomLeftCorner.x) >= 2 * minRoomWidth;

        if (heightStatus && widthStatus)
        {
            orientation = (Orientation)Random.Range(0, 2);
        }
        else if (widthStatus)
        {
            orientation = Orientation.Vertical;
        }
        else
        {
            orientation = Orientation.Horizontal;
        }

        return new Line(orientation, GetCoordinatesForOrientation(
            orientation,
            bottomLeftCorner,
            topRightCorner,
            minRoomWidth,
            minRoomHeight));
    }

    private Vector2Int GetCoordinatesForOrientation(Orientation orientation, Vector2Int bottomLeftCorner, Vector2Int topRightCorner, int minRoomWidth, int minRoomHeight)
    {
        Vector2Int coordinates = Vector2Int.zero;

        if (orientation == Orientation.Horizontal)
        {
            coordinates = new Vector2Int(
                0,
                Random.Range(
                    bottomLeftCorner.y + minRoomHeight,
                    topRightCorner.y - minRoomHeight)
                );
        }
        else
        {
            coordinates = new Vector2Int(
                Random.Range(
                    bottomLeftCorner.x + minRoomWidth,
                    topRightCorner.x - minRoomWidth),
                0);
        }

        return coordinates;
    }
}