using System.Collections.Generic;
using UnityEngine;

public static class RandomWalker
{
    public static HashSet<Vector2> RandomWalk(Vector2 startPosition, int stepAmount)
    {
        var path = new HashSet<Vector2>();

        path.Add(startPosition);
        var prevPosition = startPosition;
        for (var i = 0; i < stepAmount; i++)
        {
            var newPosition = prevPosition + Direction2D.GetRandomDirection();
            path.Add(newPosition);
            prevPosition = newPosition;
        }

        return path;
    }

    public static HashSet<Vector2> RandomWalk(Vector2 startPosition, int stepAmount, float multiplier)
    {
        var path = new HashSet<Vector2>();
        path.Add(startPosition);
        var prevPos = startPosition;

        for (var i = 0; i < stepAmount; i++)
        {
            var newPos = prevPos + Direction2D.GetRandomDirection(multiplier);
            path.Add(newPos);
            prevPos = newPos;
        }
        return path;
    }
}

public static class Direction2D //for returning random direction.
{
    public static List<int> DirectionInt = new List<int>
    {
        0,
        0,
        0,
        0,
        1,
        1,
        1,
        1,
        2,
        2,
        2,
        2,
        3,
        3,
        3,
        3,
    };

    public static List<Vector2> DirectionList = new List<Vector2>
    {
        new Vector2(0,1), //UP
        new Vector2(1,0), //RIGHT
        new Vector2(0,-1), //DOWN
        new Vector2(-1,0), //LEFT
    };

    public static Vector2 GetRandomDirection()
    {
        return DirectionList[Random.Range(0, DirectionList.Count)];
    }

    public static Vector2 GetRandomDirection(float multiplier)
    {
        return DirectionList[Random.Range(0, DirectionList.Count)] * multiplier;
    }

    public static Vector2 GetDirection(float multiplier)
    {
        var value = DirectionInt[Random.Range(0, DirectionInt.Count)];
        var direction = Vector2.zero;
        switch (value)
        {
            case 0:
                direction = Vector2.up * multiplier;
                break;
            case 1:
                direction = Vector2.down * multiplier;
                break;
            case 2:
                direction = Vector2.left * multiplier;
                break;
            case 3:
                direction = Vector2.right * multiplier;
                break;
        }

        return direction;
    }
}
