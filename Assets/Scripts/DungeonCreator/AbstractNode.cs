using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractNode
{
    public List<AbstractNode> Children { get; } = new List<AbstractNode>();
    public bool Visited { get; set; }
    public Vector2Int BottomLeftCorner { get; set; }
    public Vector2Int BottomRightCorner { get; set; }
    public Vector2Int TopLeftCorner { get; set; }
    public Vector2Int TopRightCorner { get; set; }


    public AbstractNode Parent { get; set; }

    public int TreeIndex { get; set; }

    public AbstractNode(AbstractNode parentNode)
    {
        Parent = parentNode;

        if (parentNode != null)
        {
            parentNode.AddChild(this);
        }
    }

    public void AddChild(AbstractNode abstractNode)
    {
        Children.Add(abstractNode);
    }

    public void RemoveChild(AbstractNode abstractNode)
    {
        Children.Remove(abstractNode);
    }
}