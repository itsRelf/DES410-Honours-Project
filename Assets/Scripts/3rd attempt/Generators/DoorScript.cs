using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private RoomConnections _roomConnection;
    public void SetConnection(RoomConnections connection)
    {
        _roomConnection = connection;
    }
}
