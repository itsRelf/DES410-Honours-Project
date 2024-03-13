using System;
using UnityEngine;

[Serializable]
public class Edge 
{
    public Node NodeA, NodeB;
    [field: SerializeField] public bool OneWay { set; get; }
    [field: SerializeField] public int Weight { set; get; }

    public Edge(Node nodeA, Node nodeB)
    {
        NodeA = nodeA;
        NodeB = nodeB;
        Weight = Mathf.FloorToInt(Vector2.Distance(NodeA.Position, NodeB.Position));
    }
}
