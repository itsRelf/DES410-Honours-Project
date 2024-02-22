using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    private HashSet<Vector2Int> _branchRoomSpawns;

    [SerializeField] public List<RoomConnections> _OpenConnections = new List<RoomConnections>();
    [SerializeField] public List<RoomConnections> _usedConnections = new List<RoomConnections>();
    [SerializeField] private GameObject _connectedUp, _connectedDown, _connectedLeft, _connectedRight;
    [SerializeField] private GameObject _doorUp, _doorDown, _doorLeft, _doorRight;
    [SerializeField] private List<Vector3Int> _positions;
    [field: SerializeField] public List<Vector3Int> _EnemyPositions { get; private set; }
    [field: SerializeField] public List<Vector3Int> _ItemPositions { get; private set; }

    [SerializeField] private bool _playerInRoom;
    [SerializeField] private bool _firstVisit;

    private void Awake()
    {
        _OpenConnections = new List<RoomConnections>();
        _OpenConnections.Add(new RoomConnections(RoomConnections.ConnectionDirections.Up, 0));
        _OpenConnections.Add(new RoomConnections(RoomConnections.ConnectionDirections.Down, 0));
        _OpenConnections.Add(new RoomConnections(RoomConnections.ConnectionDirections.Left, 0));
        _OpenConnections.Add(new RoomConnections(RoomConnections.ConnectionDirections.Right, 0));
    }
    private void Start()
    {
        foreach (var connection in _usedConnections)
        {
            switch (connection.ConDirection)
            {
                case RoomConnections.ConnectionDirections.Up:
                    _doorUp.SetActive(false);
                    break;
                case RoomConnections.ConnectionDirections.Down:
                    _doorDown.SetActive(false);
                    break;
                case RoomConnections.ConnectionDirections.Left:
                    _doorLeft.SetActive(false);
                    break;
                case RoomConnections.ConnectionDirections.Right:
                    _doorRight.SetActive(false);
                    break;
            }
        }

        _firstVisit = true;
        _playerInRoom = false;
    }

    private void FixedUpdate()
    {
        transform.GetChild(0).gameObject.SetActive(_playerInRoom);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _playerInRoom = true;
            if (_firstVisit = true)
                _firstVisit = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
            _playerInRoom = false;
    }
    private void OnDrawGizmos()
    {
        foreach (var enemyPosition in _EnemyPositions)
        {
            Gizmos.DrawWireSphere(enemyPosition, 0.5f);
        }
    }
}
