using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomPlacement : MonoBehaviour
{
    [SerializeField] [Range(8, 32)] private float _gridCellHeight = 8, _gridCellWidth = 8;
    [SerializeField] private int _iterations;
    [SerializeField] [Range(10, 20)] private int _numberOfRooms = 10;
    private HashSet<Vector2> _roomPositions = new HashSet<Vector2>();

    [SerializeField] private List<Sprite> _EncounterRooms;
    [SerializeField] private List<Sprite> _CorridorRooms;
    [SerializeField] private List<Sprite> _StartRooms;

    [SerializeField] private List<GameObject> _rooms = new List<GameObject>();

    public List<GameObject> Rooms => _rooms;

    [SerializeField] private GameObject _roomPrefab;
    [SerializeField] private GameObject _TestPrefab;

    void Start()
    {
        _iterations = 6;
        _numberOfRooms = 10;
        MapGen();
    }

    private IEnumerator MapGeneration()
    {
        _roomPositions = RunRandomWalk(Vector2.zero);

        foreach (var position in _roomPositions)
        {
            var room = Instantiate(_roomPrefab, position, Quaternion.identity, null);
            _rooms.Add(room);
            yield return new WaitForSeconds(0.05f);
        }
        StopAllCoroutines();
    }

    private HashSet<Vector2> RunRandomWalk(Vector2 startPosition)
    {
        var curPos = startPosition;
        var roomPositions = new HashSet<Vector2>();
        for (int i = 0; i < _iterations; i++)
        {
            var path = RandomWalker.RandomWalk(curPos, _numberOfRooms, _gridCellHeight);
            roomPositions.UnionWith(path);
            curPos = roomPositions.ElementAt(Random.Range(0, roomPositions.Count));
        }

        return roomPositions;
    }

    private void MapGen()
    {
        var startPos = Vector2.zero;
        var startingRoom = Instantiate(_roomPrefab, startPos, Quaternion.identity, null);
        _rooms.Add(startingRoom);
        var currentRoom = startingRoom;
        for (var i = 1; i < 20; i++)
        {
            var currentRoomData = currentRoom.GetComponent<RoomData>();
            var randomIndex = Random.Range(0, currentRoomData._OpenConnections.Count);
            if (currentRoomData._OpenConnections.Count == 0) continue;
            
            var randomDirection = currentRoomData._OpenConnections[randomIndex].ConDirection;
            Vector2 newPosition = currentRoom.transform.position;
            switch (randomDirection)
            {
                case RoomConnections.ConnectionDirections.Up:
                    newPosition += Vector2.up * _gridCellHeight;
                    break;
                case RoomConnections.ConnectionDirections.Down:
                    newPosition += Vector2.down * _gridCellHeight;
                    break;
                case RoomConnections.ConnectionDirections.Left:
                    newPosition += Vector2.left * _gridCellWidth;
                    break;
                case RoomConnections.ConnectionDirections.Right:
                    newPosition += Vector2.right * _gridCellWidth;
                    break;
            }

            bool roomInPlace = false;
            var roomIndex = 0;
            var removeIndex = 0;
            foreach (var room in _rooms)
            {
                if (room.transform.position == (Vector3) newPosition)
                {
                    roomInPlace = true;
                    roomIndex = _rooms.IndexOf(room);
                }
            }

            switch (roomInPlace)
            {
                case true:
                    var connectingRoomData = _rooms[roomIndex].GetComponent<RoomData>();
                    currentRoomData._OpenConnections[randomIndex].RoomIndex = roomIndex;
                    currentRoomData._usedConnections.Add(currentRoomData._OpenConnections[randomIndex]);
                    connectingRoomData._usedConnections.Add(new RoomConnections(currentRoomData._OpenConnections[randomIndex].OppositeDirection, i));
                    removeIndex = connectingRoomData._OpenConnections.FindIndex(a =>
                        a.ConDirection == currentRoomData._OpenConnections[randomIndex].OppositeDirection);
                    connectingRoomData._OpenConnections.RemoveAt(removeIndex);
                    currentRoomData._OpenConnections.RemoveAt(randomIndex);
                    break;
                case false:
                    var newRoom = Instantiate(_roomPrefab, newPosition, Quaternion.identity, null);
                    _rooms.Add(newRoom);
                    var door = currentRoom.GetComponent<RoomData>()._OpenConnections[randomIndex];
                    door.RoomIndex = i;
                    currentRoomData._usedConnections.Add(door);
                    var newRoomData = newRoom.GetComponent<RoomData>();
                    newRoomData._usedConnections.Add(new RoomConnections(door.OppositeDirection, _rooms.IndexOf(currentRoom)));
                    removeIndex = newRoomData._OpenConnections.FindIndex(a => a.ConDirection == door.OppositeDirection);
                    newRoomData._OpenConnections.RemoveAt(removeIndex);
                    currentRoomData._OpenConnections.RemoveAt(randomIndex);
                    if (Random.Range(0f, 1f) < 0.5f)
                        currentRoom = newRoom;
                    else
                    {
                        currentRoom = _rooms[Random.Range(0, _rooms.Count)];
                    }
                    break;
            }
        }
    }

    public void Spawn()
    {
        var Room = _rooms[0].GetComponent<RoomData>();

        var newRoom = Instantiate(_TestPrefab, new Vector3(50, 50), Quaternion.identity, null);
        newRoom.GetComponent<RoomData>().SetRoom(Room);
        _rooms.Add(newRoom);
    }

    private void PlaceRoom(RoomData room, Vector2 position)
    {
        var CurrentRoom = Instantiate(_roomPrefab, position, Quaternion.identity, null);
    }
    
    private void FindRoomLocations()
    {
        _roomPositions = RandomWalker.RandomWalk(Vector2.zero, _numberOfRooms, _gridCellHeight);
    }

    private void OnDrawGizmos()
    {
        foreach (var position in _roomPositions)
            Gizmos.DrawWireCube(position,new Vector3(9,9));
    }
}
