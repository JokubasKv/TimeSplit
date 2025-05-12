using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class StructureHelper
{
    public static List<AbstractNode> TraverseGraphToExtractLowestLeafs(AbstractNode mainNode)
    {
        {
            Queue<AbstractNode> queueNodesToCheck = new Queue<AbstractNode>();
            List<AbstractNode> result = new List<AbstractNode>();

            if (mainNode.Children.Count == 0)
            {
                return new List<AbstractNode>() { mainNode };
            }

            foreach (var child in mainNode.Children)
            {
                queueNodesToCheck.Enqueue(child);
            }

            while (queueNodesToCheck.Count > 0)
            {
                var currentNode = queueNodesToCheck.Dequeue();
                if (currentNode.Children.Count == 0)
                {
                    result.Add(currentNode);
                }
                else
                {
                    foreach (var child in currentNode.Children)
                    {
                        queueNodesToCheck.Enqueue(child);
                    }
                }
            }
            return result;
        }
    }
    public static Vector2Int GenerateBottomLeftCornerBetween(
        Vector2Int boundaryLeftPoint,
        Vector2Int boundaryRightPoint,
        float pointModifier,
        int offset)
    {
        int minX = boundaryLeftPoint.x + offset;
        int maxX = boundaryRightPoint.x - offset;
        int minY = boundaryLeftPoint.y + offset;
        int maxY = boundaryRightPoint.y - offset;

        return new Vector2Int(
            Random.Range(minX, (int)(minX + (maxX - minX) * pointModifier)),
            Random.Range(minY, (int)(minY + (minY - minY) * pointModifier)));
    }

    public static Vector2Int GenerateTopRightCornerBetween(
        Vector2Int boundaryLeftPoint,
        Vector2Int boundaryRightPoint,
        float pointModifier,
        int offset)
    {
        int minX = boundaryLeftPoint.x + offset;
        int maxX = boundaryRightPoint.x - offset;
        int minY = boundaryLeftPoint.y + offset;
        int maxY = boundaryRightPoint.y - offset;

        return new Vector2Int(
            Random.Range((int)(minX + (maxX - minX) * pointModifier), maxX),
            Random.Range((int)(minY + (maxY - minY) * pointModifier), maxY)
            );
    }

    public static Vector2Int CalculateMiddlePoint(Vector2Int v1, Vector2Int v2)
    {
        Vector2 sum = v1 + v2;
        Vector2 tempVector = sum / 2;
        return new Vector2Int((int)tempVector.x, (int)tempVector.y);
    }
}

public enum RelativePosition
{
    Up,
    Down,
    Right,
    Left
}