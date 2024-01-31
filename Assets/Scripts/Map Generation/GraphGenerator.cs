using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphGenerator : MonoBehaviour
{
    [field: SerializeField] private GameObject _nodePrefab;
    [field: SerializeField] private List<GameObject> _nodes;
    [field: SerializeField] private List<Edge> _edges;
    [field: SerializeField] private int _numberOfNodes;
    // Start is called before the first frame update
    void Start()
    {
        _nodes = new List<GameObject>();
        _edges = new List<Edge>();
        _numberOfNodes = Mathf.FloorToInt(Random.Range(4, 10));
        StartCoroutine(GraphGenCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator GraphGenCoroutine()
    {
        for (int i = 0; i < _numberOfNodes; i++)
        {
            yield return new WaitForSeconds(0.5f);
            var posMod = new Vector3(i, 0, 0);
            _nodes.Add(Instantiate(_nodePrefab, Vector3.zero + posMod,Quaternion.identity,null));
        }
        _nodes[0].GetComponent<Node>().SetNodeType(1);
        _nodes[_numberOfNodes - 1].GetComponent<Node>().SetNodeType(8);
    }
}
