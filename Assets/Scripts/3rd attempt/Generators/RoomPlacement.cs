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

    /// <summary>
    /// Function Generates the Dungeon Layout.
    /// Generator picks an open possible connection point (Up, Down, Left, Right).
    /// If a Room is already placed on that point in the grid the rooms are connected together creating loops.
    /// Once a room has been placed the function determines if the dungeon is built from the newly placed room or another from the list.
    /// </summary>
    private void MapGen()
    {
        var startPos = Vector2.zero;
        var startingRoom = Instantiate(_roomPrefab, startPos, Quaternion.identity, null);
        startingRoom.name = "StartRoom";
        _rooms.Add(startingRoom);
        var currentRoom = startingRoom;
        for (var i = 1; i < 20; i++)
        {
            //get the current room data
            var currentRoomData = currentRoom.GetComponent<RoomData>();
            //Select an open door slot
            var RandomDoorIndex = Random.Range(0, currentRoomData.OpenConnections.Count);
            if (currentRoomData.OpenConnections.Count == 0) continue; //move to another room if no open door slots
            
            //return a new position based on the door slot chosen. 
            var randomDirection = currentRoomData.OpenConnections[RandomDoorIndex].ConDirection;
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
                    roomInPlace = true; //Room already placed here
                    roomIndex = _rooms.IndexOf(room); //get the rooms index
                }
            }

            switch (roomInPlace)
            {
                case true:
                    var connectingRoomData = _rooms[roomIndex].GetComponent<RoomData>(); //get the room index of the connecting room
                    currentRoomData.OpenConnections[RandomDoorIndex].RoomIndex = roomIndex; //set the door slots index to the connecting room.
                    currentRoomData.UsedConnections.Add(currentRoomData.OpenConnections[RandomDoorIndex]); //add the door to the used connections list.
                    var currentRoomIndex = _rooms.IndexOf(currentRoom);
                    connectingRoomData.UsedConnections.Add(new RoomConnections(currentRoomData.OpenConnections[RandomDoorIndex].OppositeDirection, currentRoomIndex));
                    removeIndex = connectingRoomData.OpenConnections.FindIndex(a =>
                        a.ConDirection == currentRoomData.OpenConnections[RandomDoorIndex].OppositeDirection);
                    connectingRoomData.OpenConnections.RemoveAt(removeIndex);
                    currentRoomData.OpenConnections.RemoveAt(RandomDoorIndex);
                    break;
                case false:
                    var newRoom = Instantiate(_roomPrefab, newPosition, Quaternion.identity, null);
                    newRoom.name = "Room " + (_rooms.Count - 1);
                    _rooms.Add(newRoom);
                    var door = currentRoom.GetComponent<RoomData>().OpenConnections[RandomDoorIndex];
                    door.RoomIndex = _rooms.IndexOf(newRoom);
                    currentRoomData.UsedConnections.Add(door);
                    var newRoomData = newRoom.GetComponent<RoomData>();
                    newRoomData.UsedConnections.Add(new RoomConnections(door.OppositeDirection, _rooms.IndexOf(currentRoom)));
                    removeIndex = newRoomData.OpenConnections.FindIndex(a => a.ConDirection == door.OppositeDirection);
                    newRoomData.OpenConnections.RemoveAt(removeIndex);
                    currentRoomData.OpenConnections.RemoveAt(RandomDoorIndex);
                    newRoomData._firstVisit = true;
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

    public void Spawn(int index)
    {
        var Room = _rooms[index].GetComponent<RoomData>();
        if(!Room._firstVisit) 
        {
            Debug.Log("Player already been here");
            return;
        }
        var newRoom = Instantiate(_TestPrefab, Room.transform.position, Quaternion.identity, null);
        newRoom.GetComponent<RoomData>()._firstVisit = false;
        newRoom.GetComponent<RoomData>().SetRoom(Room);
        newRoom.name = "newRoomTest " + index;
        _rooms.Insert(index,newRoom);
        _rooms.Remove(Room.gameObject);
        Destroy(Room.gameObject);
    }

    private void PlaceRoom(RoomData room, Vector2 position)
    {
        var CurrentRoom = Instantiate(_roomPrefab, position, Quaternion.identity, null);
    }

    private void ReplaceRoom()
    {
        
    }

    private void RemoveEmptyRoom()
    {

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
