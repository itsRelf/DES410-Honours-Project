using UnityEngine;

public class Edge : MonoBehaviour
{
    private Node _nodeA, _nodeB;
    [field: SerializeField] public bool OneWay { set; get; }
    [field: SerializeField] public bool Test;
    public void SetNodes(Node nodeA, Node nodeB)
    {
        _nodeA = nodeA;
        _nodeB = nodeB;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(_nodeA.transform.position,_nodeB.transform.position);
    }
}
