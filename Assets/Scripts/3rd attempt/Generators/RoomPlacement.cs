using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class RoomPlacement : MonoBehaviour
{
    [SerializeField] [Range(8, 32)] private float _gridCellHeight = 8, _gridCellWidth = 8;
    [SerializeField] private int _iterations;
    [SerializeField] [Range(10, 20)] private int _numberOfRooms = 20;
    private HashSet<Vector2> _roomPositions = new HashSet<Vector2>();

    [SerializeField] private List<GameObject> _rooms = new List<GameObject>();
    [SerializeField] private List<GameObject> _potentialRooms;
    [SerializeField] private List<GameObject> _encounterRooms;
    [SerializeField] private List<GameObject> _shopRoom;
    [SerializeField] private List<GameObject> _treasureRoom;
    [SerializeField] private List<GameObject> _bossRooms;
    [SerializeField] private GameObject _lastAddedRoom;

    public List<GameObject> Rooms => _rooms;

    [SerializeField] private GameObject _roomPrefab;
    [SerializeField] private ItemSpawner _itemSpawner;

    [SerializeField] [Range(0, 100)] private int _encounterWeight;
    [SerializeField] [Range(0, 50)] private int _shopWeight;
    [SerializeField] [Range(0, 40)] private int _treasureWeight;
    [SerializeField] [Range(0, 30)] private int _bossWeight;
    private const int MAXSHOPWEIGHT = 50;
    private const int MAXTREASUREWEIGHT = 40;
    private const int MAXBOSSWEIGHT = 30;

    void Start()
    {
        _iterations = 6;
        MapGen();
    }

    private void FixedUpdate()
    {
        if (_shopWeight > MAXSHOPWEIGHT)
            _shopWeight = MAXSHOPWEIGHT;
        if (_treasureWeight > MAXTREASUREWEIGHT)
            _treasureWeight = MAXTREASUREWEIGHT;
        if (_bossWeight > MAXBOSSWEIGHT)
            _bossWeight = MAXBOSSWEIGHT;
    }

    /// <summary>
    /// Function Generates the Dungeon Layout.
    /// Generator picks an open possible connection point (Up, Down, Left, Right).
    /// If a room is already placed on that point in the grid the rooms are connected together creating loops.
    /// Once a room has been placed the function determines if the dungeon is built from the newly placed room or another from the list.
    /// </summary>
    private void MapGen()
    {
        var startPos = Vector2.zero;
        var startingRoom = Instantiate(_roomPrefab, startPos, Quaternion.identity, null);
        startingRoom.name = "StartRoom";
        _rooms.Add(startingRoom);
        var currentRoom = startingRoom;
        for (var i = 1; i < _numberOfRooms; i++)
        {
            //get the current room data
            var currentRoomData = currentRoom.GetComponent<RoomData>();
            //Select an open door slot
            if (currentRoomData.OpenConnections.Count <= 2)
            {
                var roomsStillOpen = _rooms.Where(room => room.GetComponent<RoomData>().OpenConnections.Count >= 2);
                currentRoom = roomsStillOpen.OrderBy(r => Guid.NewGuid()).FirstOrDefault();
                if (currentRoom != null)
                {
                    currentRoomData = currentRoom.GetComponent<RoomData>();
                }
                else
                {
                    Debug.Log("Restarting Level Generation");
                    SceneManager.LoadScene("SampleScene");
                }
            }

            if (currentRoomData == null)
                SceneManager.LoadScene("SampleScene");
            var randomDoorIndex = Random.Range(0, currentRoomData.OpenConnections.Count);
            //return a new position based on the door slot chosen.
            var randomDirection = currentRoomData.OpenConnections[randomDoorIndex].ConDirection;
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

            var roomInPlace = false;
            var roomIndex = 0;
            var removeIndex = 0;
            foreach (var room in _rooms)
            {
                if (room.transform.position == (Vector3) newPosition)
                {
                    roomInPlace = true; //room already placed here
                    roomIndex = _rooms.IndexOf(room); //get the rooms index
                }
            }

            /*
             * if a room is already in this position, we connect the current room with the room in the occupied position.
             * else, we place a room in the new position and connect the new room to the previous room.
             */
            
            switch (roomInPlace)
            {
                case true:
                    var connectingRoomData = _rooms[roomIndex].GetComponent<RoomData>(); //get the room index of the connecting room
                    currentRoomData.OpenConnections[randomDoorIndex].RoomIndex = roomIndex; //set the door slots index to the connecting room.
                    currentRoomData.UsedConnections.Add(currentRoomData.OpenConnections[randomDoorIndex]); //add the door to the used connections list.
                    var currentRoomIndex = _rooms.IndexOf(currentRoom);
                    connectingRoomData.UsedConnections.Add(new RoomConnections(currentRoomData.OpenConnections[randomDoorIndex].OppositeDirection, currentRoomIndex));
                    removeIndex = connectingRoomData.OpenConnections.FindIndex(a =>
                        a.ConDirection == currentRoomData.OpenConnections[randomDoorIndex].OppositeDirection);
                    connectingRoomData.OpenConnections.RemoveAt(removeIndex);
                    currentRoomData.OpenConnections.RemoveAt(randomDoorIndex);
                    break;
                case false:
                    var newRoom = Instantiate(_roomPrefab, newPosition, Quaternion.identity, null);
                    newRoom.name = "room " + (_rooms.Count - 1);
                    _rooms.Add(newRoom);
                    var door = currentRoom.GetComponent<RoomData>().OpenConnections[randomDoorIndex];
                    door.RoomIndex = _rooms.IndexOf(newRoom);
                    currentRoomData.UsedConnections.Add(door);
                    var newRoomData = newRoom.GetComponent<RoomData>();
                    newRoomData.UsedConnections.Add(new RoomConnections(door.OppositeDirection, _rooms.IndexOf(currentRoom)));
                    removeIndex = newRoomData.OpenConnections.FindIndex(a => a.ConDirection == door.OppositeDirection);
                    newRoomData.OpenConnections.RemoveAt(removeIndex);
                    currentRoomData.OpenConnections.RemoveAt(randomDoorIndex);
                    newRoomData._firstVisit = true;
                    currentRoom = Random.Range(0f, 1f) < 0.5f ? newRoom : _rooms[Random.Range(0, _rooms.Count)];
                    break;
            }

            // if the number of rooms has not been met, find the difference and remove that from i. continues the loop.
            if (i == _numberOfRooms - 1 && _rooms.Count != _numberOfRooms)
            {
                var difference = _numberOfRooms - _rooms.Count;
                i -= difference;
                _iterations++;
                Debug.Log(_iterations);
            }
        }

        
        //get all the rooms with only one used connection and store them in the end rooms list.
        var endRooms = new List<GameObject>();
        var bossRoomCount = 0;
        var maxRange = 6;

        for (var i = 1; i < _rooms.Count; i++)
        {
            var script = _rooms[i].GetComponent<RoomData>();
            if (script.UsedConnections.Count > 1) continue;
            endRooms.Add(_rooms[i]);
        }

        //find the room the furthest away from the spawn room and set that as the final boss room
        var distance = 0f;
        var furthestIndex = 0;
        var bossRoomIndex = 0;
        var startRoom = _rooms[0];
        foreach (var endRoom in endRooms)
        {
            var roomDist = Vector3.Distance(endRoom.transform.position, startRoom.transform.position);
            Debug.Log(roomDist);
            if (roomDist > distance)
            {
                distance = roomDist;
                furthestIndex = _rooms.IndexOf(endRoom);
                bossRoomIndex = endRooms.IndexOf(endRoom);
            }
        }

        endRooms.RemoveAt(bossRoomIndex);
        SpawnFinalBoss(furthestIndex);

        //Replace the remaining end rooms with shops.
        for (var i = 0; i < endRooms.Count; i++)
        {
            Debug.Log(endRooms.Count);
            var script = endRooms[i].GetComponent<RoomData>();
            if (script.UsedConnections.Count > 1) continue;
            var index = _rooms.IndexOf(endRooms[i]);
            Debug.Log(index);
            var random = Random.Range(0, 6);
            SpawnShops(index);
        }
    }

    //Replaces a room with the final boss room.
    public void SpawnFinalBoss(int index)
    {
        var room = GetRoomData(index, _rooms);
        var newRoom = ReturnRoom(_bossRooms);
        var roomName = "FinalBossRoom";
        SpawnRoom(roomName, newRoom, room, index);
        _rooms[index].GetComponent<RoomData>()._finalBossRoom = true;
    }

    //Method to place a show room.
    public void SpawnShops(int index)
    {
        var room = GetRoomData(index, _rooms);
        var newRoom = ReturnRoom(_shopRoom);
        var roomName = "Shop " + index;  
        SpawnRoom(roomName, newRoom, room, index);
        var roomScript = _rooms[index].GetComponent<RoomData>();
        roomScript.PlaceShopItems(_itemSpawner._shopItems);
    }


    /*
     * Using weighted random numbers this function replaces rooms if it is the players first visit.
     * weighted values are increased/decreased depending on the result of the RNG.
     */
    public void EmptyRoomReplacer(int index)
    {
        var roomData = GetRoomData(index, _rooms);
        if (!roomData._firstVisit) return;

        var weightList = new List<int>
        {
            _bossWeight,
            _encounterWeight,
            _shopWeight,
            _treasureWeight
        };

        var randomNumber = Random.Range(1, 101);
        var possibleWeight = _encounterWeight;
        foreach (var weight in weightList)
        {
            if (randomNumber <= weight)
                possibleWeight = weight;
        }

        if (possibleWeight <= _encounterWeight && possibleWeight > _shopWeight && possibleWeight > _treasureWeight && possibleWeight > _bossWeight)
        {
            var newRoom = ReturnRoom(_encounterRooms);
            var roomName = newRoom.name;
            SpawnRoom(roomName, newRoom, roomData, index);
            _bossWeight = _bossWeight >= MAXBOSSWEIGHT ? MAXBOSSWEIGHT : _bossWeight += 1;
            _shopWeight = _shopWeight >= MAXSHOPWEIGHT ? MAXSHOPWEIGHT : _shopWeight += 5;
            _treasureWeight = _treasureWeight >= MAXTREASUREWEIGHT ? MAXTREASUREWEIGHT : _treasureWeight += 3;
        }
        else if (possibleWeight <= _encounterWeight && possibleWeight <= _shopWeight && possibleWeight > _treasureWeight && possibleWeight > _bossWeight)
        {
            var newRoom = ReturnRoom(_shopRoom);
            var roomName = newRoom.name;
            SpawnRoom(roomName, newRoom, roomData, index);
            var shopData = _rooms[index].GetComponent<RoomData>();
            shopData.PlaceShopItems(_itemSpawner._shopItems);
            _bossWeight = _bossWeight >= MAXBOSSWEIGHT ? MAXBOSSWEIGHT : _bossWeight += 1;
            _shopWeight = 2;
            _treasureWeight = 1;
        }
        else if (possibleWeight <= _encounterWeight && possibleWeight <= _shopWeight && possibleWeight <= _treasureWeight && possibleWeight > _bossWeight)
        {
            var newRoom = ReturnRoom(_treasureRoom);
            var roomName = newRoom.name;
            SpawnRoom(roomName, newRoom, roomData, index);
            var treasureData = _rooms[index].GetComponent<RoomData>();
            var position = new Vector2(treasureData._ItemPositions[0].x, treasureData._ItemPositions[0].y) 
                           + (Vector2)treasureData.transform.position;
            _itemSpawner.SpawnTreasure(position);
            _bossWeight = _bossWeight >= MAXBOSSWEIGHT ? MAXBOSSWEIGHT : _bossWeight += 1;
            _shopWeight = 2;
            _treasureWeight = 1;
        }
        else if (possibleWeight <= _encounterWeight && possibleWeight <= _shopWeight && possibleWeight <= _treasureWeight && possibleWeight <= _bossWeight)
        {
            var newRoom = ReturnRoom(_bossRooms);
            var roomName = newRoom.name;
            SpawnRoom(roomName, newRoom, roomData, index);
            _bossWeight = 0;
            _shopWeight = _shopWeight >= MAXSHOPWEIGHT ? MAXSHOPWEIGHT : _shopWeight += 10;
            _treasureWeight = _treasureWeight >= MAXTREASUREWEIGHT ? MAXTREASUREWEIGHT : _treasureWeight += 10;
        }
    }

    /*
     * Instantiates the new room to be added to the dungeon
     * inserts the new room at the passed in index. 
     * removes the empty room from the list of rooms and deletes the game object for the old room.
     */
    private void SpawnRoom(string roomName, GameObject newRoom, RoomData oldRoomData, int index) 
    {
        var newRoom2 = Instantiate(newRoom, oldRoomData.transform.position, Quaternion.identity, null);
        newRoom2.name = roomName;
        newRoom2.GetComponent<RoomData>().SetRoom(oldRoomData);
        newRoom2.GetComponent<RoomData>()._firstVisit = false;
        _rooms.Insert(index, newRoom2);
        _rooms.Remove(oldRoomData.gameObject);
        Destroy(oldRoomData.gameObject);
    }

    //returns the room data script for the desired room.
    private static RoomData GetRoomData(int index, List<GameObject> rooms)
    {
        return rooms[index].GetComponent<RoomData>();
    }

    //returns a room from a passed in list
    private static GameObject ReturnRoom(List<GameObject> roomTypeList)
    {
        return roomTypeList[Random.Range(0, roomTypeList.Count)];
    }

    private void OnDrawGizmos()
    {
        foreach (var position in _roomPositions)
            Gizmos.DrawWireCube(position,new Vector3(9,9));
    }
}
