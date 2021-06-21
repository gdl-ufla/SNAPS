using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{
    public enum Direction
    {
        Up, Down, Right, Left, Forward, Backward, None
    }
    public enum BehaviourState
    {
        Idle, Patrolling, Chasing, Attacking, OnCooldown
    }
    public static Direction FlipDirection (Enums.Direction direction)
    {
        switch (direction)
        {
            case Enums.Direction.Right:
                return Enums.Direction.Left;
            case Enums.Direction.Left:
                return Enums.Direction.Right;
            case Enums.Direction.Up:
                return Enums.Direction.Down;
            case Enums.Direction.Down:
                return Enums.Direction.Up;
            case Enums.Direction.Forward:
                return Enums.Direction.Backward;
            case Enums.Direction.Backward:
                return Enums.Direction.Forward;
            default:
                return Enums.Direction.None;
        }
    }
}
