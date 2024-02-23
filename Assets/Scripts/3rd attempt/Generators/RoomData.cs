using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Empty,
    Encounter,
    Shop,
    Treasure,
    Boss,
}
public class RoomData : MonoBehaviour
{
    
    private HashSet<Vector2Int> _branchRoomSpawns;
    public RoomType _roomType { get; set;}
    [SerializeField] public List<RoomConnections> _OpenConnections = new List<RoomConnections>();
    [SerializeField] public List<RoomConnections> _usedConnections = new List<RoomConnections>();
    [SerializeField] private GameObject _connectedUp, _connectedDown, _connectedLeft, _connectedRight;
    [SerializeField] private GameObject _doorUp, _doorDown, _doorLeft, _doorRight;
    [SerializeField] private Collider2D _mainTrigger, _doorUpTrigger, _doorDownTrigger, _doorLeftTrigger, _doorRightTrigger;
    [SerializeField] private List<Vector3Int> _positions;
    [field: SerializeField] public List<Vector3Int> _EnemyPositions { get; private set; }
    [field: SerializeField] public List<Vector3Int> _ItemPositions { get; private set; }

    [SerializeField] public bool _playerInRoom { get; set; }
    [SerializeField] public bool _firstVisit { get; set; }

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
        SetDoors();
        SetType();
        SetEnemyPositions();
        
        _firstVisit = true;
        _playerInRoom = false;
    }

    private void SetEnemyPositions()
    { 
        for(var i = 0; i < _EnemyPositions.Count; i++)
        {
            _EnemyPositions[i] += new Vector3Int((int)transform.position.x, (int)transform.position.y);
        }
    }

    private void FixedUpdate()
    {
        //transform.GetChild(0).gameObject.SetActive(_playerInRoom);
    }

    private void SetDoors()
    {
        foreach (var connection in _usedConnections)
        {
            if (connection.ConDirection == RoomConnections.ConnectionDirections.Up)
                _doorUp.SetActive(false);
            else if (connection.ConDirection == RoomConnections.ConnectionDirections.Down)
                _doorDown.SetActive(false);
            else if (connection.ConDirection == RoomConnections.ConnectionDirections.Left)
                _doorLeft.SetActive(false);
            else if (connection.ConDirection == RoomConnections.ConnectionDirections.Right)
                _doorRight.SetActive(false);
        }
    }

    public void SetRoom(RoomData otherRoom)
    {
        _OpenConnections = otherRoom._OpenConnections;
        _usedConnections = otherRoom._usedConnections;
        SetEnemyPositions();
    }

    private void SetType()
    {
        if (_usedConnections.Count == 1)
        {
            _roomType = (RoomType)Random.Range(2, 5);

        }

        tag = _roomType switch
        {
            RoomType.Empty => "EmptyRoom",
            RoomType.Encounter => "EncounterRoom",
            RoomType.Treasure => "TreasureRoom",
            RoomType.Shop => "ShopRoom",
            RoomType.Boss => "BossRoom",
            _ => tag
        };
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.tag);
        var tag = other.tag;
        if (_mainTrigger.bounds.Intersects(other.bounds))
        {
            if (tag != "Player") return;
            Debug.Log("Player in Room");
            _playerInRoom = true;
            if (_firstVisit)
                _firstVisit = false;
        }
        if (_doorUpTrigger.bounds.Intersects(other.bounds))
        {
            if (tag != "Player") return;
            Debug.Log("Player Coming Down");
        }
        if (_doorDownTrigger.bounds.Intersects(other.bounds))
        {
            if (tag != "Player") return;
            Debug.Log("Player Coming Up");
        }
        if (_doorLeftTrigger.bounds.Intersects(other.bounds))
        {
            if (tag != "Player") return;
            Debug.Log("Player Coming From the Right");
        }
        if (_doorRightTrigger.bounds.Intersects(other.bounds))
        {
            if (tag != "Player") return;
            Debug.Log("Player Coming From the Left");
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
