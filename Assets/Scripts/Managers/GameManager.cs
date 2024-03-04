using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Range(0,100)]private int _tension;
    [SerializeField] private RoomPlacement _roomGenerator;
    [SerializeField] private List<DoorScript> _currentRoomDoors;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _player;
    [SerializeField] private List<GameObject> _enemies;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private RoomData _currentRoom;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _roomGenerator = FindObjectOfType<RoomPlacement>();
    }

    void Start()
    {
        SpawnPlayer();
        Checkroom();
    }

    private void SpawnPlayer()
    {
        _player = Instantiate(_playerPrefab, Vector2.zero, Quaternion.identity, null);
    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
        Checkroom();
        CheckDoor();
    }

    private void Checkroom()
    {
        if(_currentRoom != null && _currentRoom.GetComponent<RoomData>()._playerInRoom) return;
        foreach(var room in _roomGenerator.Rooms)
        {
            if(room.GetComponent<RoomData>()._playerInRoom)
            {
                _mainCamera.transform.position = room.transform.position;
                _currentRoom = room.GetComponent<RoomData>();
                _currentRoomDoors = _currentRoom.GetComponentsInChildren<DoorScript>().ToList();
            }
        }
    }

    private void CheckDoor()
    {
        foreach(var door in _currentRoomDoors)
        {
            if(door.PlayerAtDoor)
            {
                var Index = door.RoomConnection.RoomIndex;
                Debug.Log(_roomGenerator.Rooms[Index].GetComponent<RoomData>()._firstVisit);
                _roomGenerator.Spawn(Index);
            }
        }
    }
}
