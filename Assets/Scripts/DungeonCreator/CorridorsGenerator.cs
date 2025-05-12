using System.Collections.Generic;
using System.Linq;
public class CorridorsGenerator
{
    public List<AbstractNode> CreateCorridor(List<RoomNode> allNodesCollection, int corridorWidth)
    {
        List<AbstractNode> corridorList = new List<AbstractNode>();
        Queue<RoomNode> structuresToCheck = new Queue<RoomNode>(
            allNodesCollection.OrderByDescending(node => node.TreeIndex).ToList());
        while (structuresToCheck.Count > 0)
        {
            var node = structuresToCheck.Dequeue();
            if (node.Children.Count == 0)
            {
                continue;
            }

            CorridorNode corridor = new CorridorNode(node.Children[0], node.Children[1], corridorWidth);
            corridorList.Add(corridor);
        }
        return corridorList;
    }
}