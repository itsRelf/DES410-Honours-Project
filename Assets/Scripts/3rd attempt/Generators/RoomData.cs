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
    public List<RoomConnections> OpenConnections;
    public List<RoomConnections> UsedConnections;
    [SerializeField] private GameObject _doorUp, _doorDown, _doorLeft, _doorRight;
    [SerializeField] private Collider2D _mainTrigger;
    [SerializeField] private List<Vector3Int> _positions;
    [field: SerializeField] public List<Vector3Int> _EnemyPositions { get; private set; }
    [field: SerializeField] public List<GameObject> _enemies { get; private set; }
    [field: SerializeField] public List<Vector3Int> _ItemPositions { get; private set; }

    [field: SerializeField] public bool _playerInRoom { get; set; }
    [field: SerializeField] public bool _firstVisit { get; set; }
    [SerializeField] private GameObject _DoorUPPrefab;
    [SerializeField] private GameObject _DoorDownPrefab;
    [SerializeField] private GameObject _DoorLeftPrefab;
    [SerializeField] private GameObject _DoorRightPrefab;
    [field: SerializeField] public bool _finalBossRoom;
    [field: SerializeField] public bool _roomCleared;

    private void Awake()
    {
        OpenConnections = new List<RoomConnections>();
        UsedConnections = new List<RoomConnections>();
        OpenConnections.Add(new RoomConnections(RoomConnections.ConnectionDirections.Up, 0));
        OpenConnections.Add(new RoomConnections(RoomConnections.ConnectionDirections.Down, 0));
        OpenConnections.Add(new RoomConnections(RoomConnections.ConnectionDirections.Left, 0));
        OpenConnections.Add(new RoomConnections(RoomConnections.ConnectionDirections.Right, 0));
    
    }
    private void Start()
    {
        SetDoors();
        _playerInRoom = false;
        foreach (var enemy in _enemies)
        {
            enemy.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        transform.GetChild(0);
    }


    //display the appropriate doors for the rooms connected to the top, bottom, left, or right of this room.
    private void SetDoors()
    {
        UsedConnections.ForEach(connection =>
        {
            switch (connection.ConDirection)
            {
                case RoomConnections.ConnectionDirections.Up:
                    _doorUp.SetActive(false);
                    var doorUp = Instantiate(_DoorUPPrefab, _doorUp.transform.position, Quaternion.identity, transform.GetChild(0));
                    doorUp.GetComponent<DoorScript>().SetConnection(connection);
                    break;
                case RoomConnections.ConnectionDirections.Down:
                    _doorDown.SetActive(false);
                    var doorDown = Instantiate(_DoorDownPrefab, _doorDown.transform.position, Quaternion.identity, transform.GetChild(0));
                    doorDown.GetComponent<DoorScript>().SetConnection(connection);
                    break;
                case RoomConnections.ConnectionDirections.Left:
                    _doorLeft.SetActive(false);
                    var doorLeft = Instantiate(_DoorLeftPrefab, _doorLeft.transform.position, Quaternion.identity, transform.GetChild(0));
                    doorLeft.GetComponent<DoorScript>().SetConnection(connection);
                    break;
                case RoomConnections.ConnectionDirections.Right:
                    _doorRight.SetActive(false);
                    var doorRight = Instantiate(_DoorRightPrefab, _doorRight.transform.position, Quaternion.identity, transform.GetChild(0));
                    doorRight.GetComponent<DoorScript>().SetConnection(connection);
                    break;
            }
        });
    }

    //replace the connection point data for the new room with data from the old room. Used during room replacement.
    public void SetRoom(RoomData otherRoom)
    {
        OpenConnections = otherRoom.OpenConnections;
        UsedConnections = otherRoom.UsedConnections;
        _firstVisit = false;
        SetEnemyPositions();
    }

    //Place shop items into appropriate positions, apply the offset for the rooms current world space position.
    public void PlaceShopItems(List<GameObject> shopItems)
    {
        foreach (var position in _ItemPositions)
        {
            var random = Random.Range(0, shopItems.Count);
            var shopItem = shopItems[random];
            switch (shopItem.tag)
            {
                case "Potion":
                    shopItem.GetComponent<potionScript>().ShopItem = true;
                    break;
            }
            Instantiate(shopItem, position + transform.position, Quaternion.identity, this.transform);
        }
    }

    private void SetEnemyPositions()
    { 
        for(var i = 0; i < _EnemyPositions.Count; i++)
        {
            _EnemyPositions[i] = new Vector3Int((int)transform.position.x + _EnemyPositions[i].x, (int)transform.position.y + _EnemyPositions[i].y);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var tag = other.tag;
        if (_mainTrigger.bounds.Intersects(other.bounds))
        {
            if (tag != "Player") return;
            _playerInRoom = true;
            if (_firstVisit)
                _firstVisit = false;
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        var tag = other.tag;
        if (_mainTrigger.bounds.Intersects(other.bounds))
        {
            if (tag != "Player") return;
            _playerInRoom = true;
            if (_firstVisit)
                _firstVisit = false;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        var tag = other.tag;
        if (!_mainTrigger.bounds.Intersects(other.bounds))
        {
            if (tag != "Player") return;
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
