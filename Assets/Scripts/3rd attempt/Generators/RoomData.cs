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
    public List<RoomConnections> OpenConnections = new List<RoomConnections>();
    public List<RoomConnections> UsedConnections = new List<RoomConnections>();
    [SerializeField] private GameObject _connectedUp, _connectedDown, _connectedLeft, _connectedRight;
    [SerializeField] private GameObject _doorUp, _doorDown, _doorLeft, _doorRight;
    [SerializeField] private Collider2D _mainTrigger, _doorUpTrigger, _doorDownTrigger, _doorLeftTrigger, _doorRightTrigger;
    [SerializeField] private List<Vector3Int> _positions;
    [field: SerializeField] public List<Vector3Int> _EnemyPositions { get; private set; }
    [field: SerializeField] public List<Vector3Int> _ItemPositions { get; private set; }

    [field: SerializeField] public bool _playerInRoom { get; set; }
    [field: SerializeField] public bool _firstVisit { get; set; }
    [SerializeField] List<GameObject> _DoorPrefabs = new List<GameObject>();
    private void Awake()
    {
        OpenConnections = new List<RoomConnections>();
        OpenConnections.Add(new RoomConnections(RoomConnections.ConnectionDirections.Up, 0));
        OpenConnections.Add(new RoomConnections(RoomConnections.ConnectionDirections.Down, 0));
        OpenConnections.Add(new RoomConnections(RoomConnections.ConnectionDirections.Left, 0));
        OpenConnections.Add(new RoomConnections(RoomConnections.ConnectionDirections.Right, 0));
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
        transform.GetChild(0).gameObject.SetActive(_playerInRoom);
    }

    private void SetDoors()
    {
        UsedConnections.ForEach(connection =>
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
        });
        _doorUpTrigger.enabled = !_doorUp.activeSelf;
        _doorDownTrigger.enabled = !_doorDown.activeSelf;
        _doorLeftTrigger.enabled = !_doorLeft.activeSelf;
        _doorRightTrigger.enabled = !_doorRight.activeSelf;

    }

    public void SetRoom(RoomData otherRoom)
    {
        OpenConnections = otherRoom.OpenConnections;
        UsedConnections = otherRoom.UsedConnections;
        SetEnemyPositions();
    }

    private void SetType()
    {
        if (UsedConnections.Count == 1)
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
        Debug.Log(other.tag);
        var tag = other.tag;
        if (!_mainTrigger.bounds.Intersects(other.bounds))
        {
            if (tag != "Player") return;
            Debug.Log("Player in Room");
            _playerInRoom = false;
        }
    }
    private void OnDrawGizmos()
    {
        foreach (var enemyPosition in _EnemyPositions)
        {
            Gizmos.DrawWireSphere(enemyPosition, 0.5f);
        }
    }
}
