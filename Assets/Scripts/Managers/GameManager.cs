using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private RoomPlacement _roomGenerator;
    [SerializeField] private RoomData _currentRoom;
    [SerializeField] private List<DoorScript> _currentRoomDoors;
    [SerializeField] private List<GameObject> _currentRoomEnemies;
    [SerializeField] private bool _currentRoomLocked;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _player;
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
        Exit();
    }

    private void FixedUpdate()
    {
        GetCurrentRoom();
        CheckDoorForConnectingRoom();
        EnemyTracker();
        PlayerTracker();
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
            //Debug.Log(_roomGenerator.Rooms[Index].GetComponent<RoomData>()._firstVisit);
            ReplaceBaseRoom(Index);
        }
    }

    private void ReplaceBaseRoom(int roomIndex)
    {
        //_roomGenerator.Spawn(roomIndex);
        _roomGenerator.EmptyRoomReplacer(roomIndex);
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

    //resets the level if the player dies
    private void PlayerTracker()
    {
        if (_player.GetComponent<PlayerScript>().Dead)
            SceneManager.LoadScene("SampleScene");
    }

    //resets the level if the final boss room is cleared or potentially spawns a random item.
    private void CheckInOnRoomClear()
    {
        if (_currentRoomEnemies.Count != 0) return;
        if (_currentRoom.GetComponent<RoomData>()._finalBossRoom)
            SceneManager.LoadScene("SampleScene");
        var chanceOfItem = Random.Range(0, 10);
        if(chanceOfItem > 3)
            _itemSpawner.SpawnRandomItem(_currentRoom.transform.position);
    }
    #endregion

    private void SpawnItem(Vector2 position)
    {
        _itemSpawner.SpawnRandomItem(position);
    }

    private void Exit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Menu");
    }
}
