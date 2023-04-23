using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionFace : MonoBehaviour
{
    [SerializeField] PlaneFacingDirection Direction = 0;

    public PlaneFacingDirection GetFacingDirection()
    {
        return Direction;
    }
}

public enum PlaneFacingDirection { Front = 0, Back = 1, Up = 2, Down = 3, Left = 4, Right = 5, None = 6 }