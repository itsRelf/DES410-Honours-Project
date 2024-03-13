using System;

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
        OppositeDirection = ConDirection switch
        {
            ConnectionDirections.Up => ConnectionDirections.Down,
            ConnectionDirections.Down => ConnectionDirections.Up,
            ConnectionDirections.Left => ConnectionDirections.Right,
            ConnectionDirections.Right => ConnectionDirections.Left,
            _ => OppositeDirection
        };
    }
}
