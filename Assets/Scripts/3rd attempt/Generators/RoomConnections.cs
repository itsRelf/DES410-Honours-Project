using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Based on Steps written by User Inverno969
[Serializable]
public class RoomConnections
{
    public enum ConnectionDirections
    {
        Up,
        Down,
        Left,
        Right
    }

    public ConnectionDirections ConDirection;
    public ConnectionDirections OppositeDirection;
    public int RoomIndex;

    public RoomConnections(ConnectionDirections conDir, int roomIndex)
    {
        ConDirection = conDir;
        RoomIndex = roomIndex;
        switch (ConDirection)
        {
            case ConnectionDirections.Up:
                OppositeDirection = ConnectionDirections.Down;
                break;
            case ConnectionDirections.Down:
                OppositeDirection = ConnectionDirections.Up;
                break;
            case ConnectionDirections.Left:
                OppositeDirection = ConnectionDirections.Right;
                break;
            case ConnectionDirections.Right:
                OppositeDirection = ConnectionDirections.Left;
                break;
        }
    }
}
