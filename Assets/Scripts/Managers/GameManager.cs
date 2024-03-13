using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField] [Range(0,100)] private int _tension;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private RoomPlacement _roomGenerator;
    [SerializeField] private RoomData _currentRoom;
    [SerializeField] private List<DoorScript> _currentRoomDoors;
    [SerializeField] private List<GameObject> _currentRoomEnemies;
    [SerializeField] private bool _currentRoomLocked;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _player;
    [SerializeField] private int _playerHealthOnEntry;
    [SerializeField] private int _playerHealthOnRoomClear;
    [SerializeField] private ItemSpawner _itemSpawner;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _roomGenerator = FindObjectOfType<RoomPlacement>();
        _itemSpawner = FindObjectOfType<ItemSpawner>();
    }

    void Start()
    {
        SpawnPlayer();
        GetCurrentRoom();
    }

    void Update()
    {
        MoveCamera();
    }

    private void FixedUpdate()
    {
        GetCurrentRoom();
        CheckDoorForConnectingRoom();
        EnemyTracker();
    }

    //Moves the camera smoothly between room positions.
    private void MoveCamera()
    {
        if (_currentRoom != null && _mainCamera.transform.position != _currentRoom.transform.position)
            _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, _currentRoom.transform.position, 10 * Time.fixedDeltaTime);
    }

    #region Room Specific Functions

    //Gets the current room and stores the active doorways and enemies in room.
    private void GetCurrentRoom()
    {
        if (_currentRoom != null && _currentRoom.GetComponent<RoomData>()._playerInRoom)
        {
            LockDoors();
            return;
        }
        foreach (var room in _roomGenerator.Rooms.Where(room => room.GetComponent<RoomData>()._playerInRoom))
        {
            _currentRoom = room.GetComponent<RoomData>();
            _currentRoomDoors = _currentRoom.GetComponentsInChildren<DoorScript>().ToList();
            _currentRoomEnemies = _currentRoom.GetComponent<RoomData>()._enemies;
            _playerHealthOnEntry = _player.GetComponent<PlayerScript>().HealthCheckIn();
        }

        if (_currentRoom == null) return;
        foreach (var enemy in _currentRoom._enemies)
        {
            if(enemy.activeSelf) continue;
            enemy.SetActive(true);
        }
    }
    
    //Checks a door the player is about to enter. Determines if the room is to be replaced or not.
    private void CheckDoorForConnectingRoom()
    {
        foreach (var Index in from door in _currentRoomDoors where door.PlayerAtDoor select door.RoomConnection.RoomIndex)
        {
            Debug.Log(_roomGenerator.Rooms[Index].GetComponent<RoomData>()._firstVisit);
            ReplaceBaseRoom(Index);
        }
    }

    private void ReplaceBaseRoom(int roomIndex)
    {
        _roomGenerator.Spawn(roomIndex);
    }

    private void LockDoors()
    {
        foreach (var door in _currentRoomDoors)
        {
            door.TriggerBox.enabled = _currentRoomEnemies.Count == 0;
            door.CollisionBox.enabled = _currentRoomEnemies.Count > 0;
        }
    }
    #endregion

    //Deletes Enemies marked as Dead.
    private void EnemyTracker()
    {
        for (var i = 0; i < _currentRoomEnemies.Count; i++)
        {
            if (!_currentRoomEnemies[i].GetComponent<EnemyScript>().IsDead) continue;
            var chanceOfItem = Random.Range(0, 10);
            if(chanceOfItem > 4)
                SpawnItem(_currentRoomEnemies[i].transform.position);
            Destroy(_currentRoomEnemies[i]);
            _currentRoomEnemies.RemoveAt(i);
            CheckInOnRoomClear();
        }
    }

    #region Player Specific Functions
    private void SpawnPlayer()
    {
        _player = Instantiate(_playerPrefab, Vector2.zero, Quaternion.identity, null);
        _player.name = "Player";
    }

    private void CheckInOnRoomClear()
    {
        if (_currentRoomEnemies.Count != 0) return;
        _playerHealthOnRoomClear = _player.GetComponent<PlayerScript>().HealthCheckIn();
        var chanceOfItem = Random.Range(0, 10);
        if(chanceOfItem > 3)
            _itemSpawner.SpawnRandomItem(_currentRoom.transform.position);
    }
    #endregion

    #region Game Tension Functions

    private void TensionCheck()
    {
        var difference = _playerHealthOnEntry - _playerHealthOnRoomClear;

        switch (difference)
        {
            case 0:
                break;
            case > 0 and <= 25:
                break;
            case > 25 and <= 50:
                break;
            case > 50 and <= 100:
                break;
        }
    }
    #endregion

    private void SpawnItem(Vector2 position)
    {
        _itemSpawner.SpawnRandomItem(position);
    }
}
