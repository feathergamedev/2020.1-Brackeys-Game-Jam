using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Left = 0,
    Right = 1,
    Up,
    Down 
}

public static class DirectionExtension
{
    public static Vector2Int ToVec2(this Direction direction)
    {

        var result = Vector2Int.zero;

        switch(direction)
        {
            case Direction.Left:
                result.x = -1;
                break;
            case Direction.Right:
                result.x = 1;
                break;
            case Direction.Up:
                result.y = 1;
                break;
            case Direction.Down:
                result.y = -1;
                break;
        }

        return result;
    }
}