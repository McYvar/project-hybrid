using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Facing : MonoBehaviour
{
    PlaneFacingDirection castX = PlaneFacingDirection.None;
    PlaneFacingDirection castY = PlaneFacingDirection.None;
    PlaneFacingDirection castZ = PlaneFacingDirection.None;

    PlaneFacingDirection castXPrev = PlaneFacingDirection.None;
    PlaneFacingDirection castYPrev = PlaneFacingDirection.None;
    PlaneFacingDirection castZPrev = PlaneFacingDirection.None;

    public static int xAxisIterator = 0;
    public static int yAxisIterator = 0;

    public static bool reset;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || reset)
        {
            castX = PlaneFacingDirection.Right;
            castY = PlaneFacingDirection.Up;
            castZ = PlaneFacingDirection.Front;

            yAxisIterator = 0;
            xAxisIterator = 0;

            reset = false;

            return;
        }

        castXPrev = castX;
        castYPrev = castY;
        castZPrev = castZ;

        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(new Vector3(SerialHandler.rotX, SerialHandler.rotY, 0)) * transform.right);
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(new Vector3(SerialHandler.rotX, SerialHandler.rotY, 0)) * transform.up);
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(new Vector3(SerialHandler.rotX, SerialHandler.rotY, 0)) * transform.forward);
        // Step one is casting a ray in all positive axis
        RaycastHit hitX;
        if (Physics.Raycast(transform.position, Quaternion.Euler(new Vector3(SerialHandler.rotX, SerialHandler.rotY, 0)) * Vector3.right, out hitX)) 
        {
            // If a face is found (wich it should) determine which face it is
            DirectionFace faceX = hitX.transform.GetComponent<DirectionFace>();
            if (faceX != null)
            {
                castX = faceX.GetFacingDirection();
            }
        }

        RaycastHit hitY;
        if (Physics.Raycast(transform.position, Quaternion.Euler(new Vector3(SerialHandler.rotX, SerialHandler.rotY, 0)) * Vector3.up, out hitY))
        {
            // If a face is found (wich it should) determine which face it is
            DirectionFace faceY = hitY.transform.GetComponent<DirectionFace>();
            if (faceY != null)
            {
                castY = faceY.GetFacingDirection();
            }
        }

        RaycastHit hitZ;
        if (Physics.Raycast(transform.position, Quaternion.Euler(new Vector3(SerialHandler.rotX, SerialHandler.rotY, 0)) * Vector3.forward, out hitZ))
        {
            // If a face is found (wich it should) determine which face it is
            DirectionFace faceZ = hitZ.transform.GetComponent<DirectionFace>();
            if (faceZ != null)
            {
                castZ = faceZ.GetFacingDirection();
            }
        }

        // Rotation around the y-axis
        if (castX != castXPrev && castZ != castZPrev && castY == castYPrev)
        {
            // Determine the current face the y-axis is facing
            // Then deterermine the last face the x-axis was facing
            // And last dertermine wich face the x-axis is currently facing
            switch (castY)
            {
                case PlaneFacingDirection.Front: // Y facing front
                    switch (castXPrev)
                    {
                        case PlaneFacingDirection.Up: // Last Z facing right
                            if (castX == PlaneFacingDirection.Right) // Z facing down
                            {
                                yAxisIterator--;
                            }
                            else // X facing left, Z facing up
                            {
                                yAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Down: // Last Z facing left
                            if (castX == PlaneFacingDirection.Right) // Z facing down
                            {
                                yAxisIterator++;
                            }
                            else // X facing left, Z facing up
                            {
                                yAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Left: // Last Z facing up
                            if (castX == PlaneFacingDirection.Up) // Z facing right
                            {
                                yAxisIterator--;
                            }
                            else // X facing down, Z facing Left
                            {
                                yAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Right: // Last Z facing down
                            if (castX == PlaneFacingDirection.Up) // Z facing right
                            {
                                yAxisIterator++;
                            }
                            else // X facing down, Z facing left
                            {
                                yAxisIterator--;
                            }
                            break;
                    }
                    break;
                case PlaneFacingDirection.Back: // Y facing back
                    switch (castXPrev)
                    {
                        case PlaneFacingDirection.Up: // Last Z facing left
                            if (castX == PlaneFacingDirection.Right) // Z facing up
                            {
                                yAxisIterator++;
                            }
                            else // X facing left, Z facing down
                            {
                                yAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Down: // Last Z facing right
                            if (castX == PlaneFacingDirection.Right) // Z facing up
                            {
                                yAxisIterator--;
                            }
                            else // X facing left, Z facing down
                            {
                                yAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Left: // Last Z facing down
                            if (castX == PlaneFacingDirection.Up) // Z facing left
                            {
                                yAxisIterator++;
                            }
                            else // X facing down, Z facing right
                            {
                                yAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Right: // Last Z facing up
                            if (castX == PlaneFacingDirection.Up) // Z facing left
                            {
                                yAxisIterator--;
                            }
                            else // X facing down, Z facing right
                            {
                                yAxisIterator++;
                            }
                            break;
                    }
                    break;
                case PlaneFacingDirection.Up: // Y facing up
                    switch (castXPrev)
                    {
                        case PlaneFacingDirection.Front: // Last Z facing left
                            if (castX == PlaneFacingDirection.Right) // Z facing front
                            {
                                yAxisIterator++;
                            }
                            else // X facing left, Z facing back
                            {
                                yAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Back: // Last Z facing right
                            if (castX == PlaneFacingDirection.Right) // Z facing front
                            {
                                yAxisIterator--;
                            }
                            else // X facing left, Z facing back
                            {
                                yAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Left: // Last Z facing back
                            if (castX == PlaneFacingDirection.Front) // Z facing left
                            {
                                yAxisIterator++;
                            }
                            else // X facing back, Z facing right
                            {
                                yAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Right: // Last Z facing front
                            if (castX == PlaneFacingDirection.Front) // Z facing left
                            {
                                yAxisIterator--;
                            }
                            else // X facing back, Z facing right
                            {
                                yAxisIterator++;
                            }
                            break;
                    }
                    break;
                case PlaneFacingDirection.Down: // Y facing down
                    switch (castXPrev)
                    {
                        case PlaneFacingDirection.Front: // Last Z facing right
                            if (castX == PlaneFacingDirection.Right) // Z facing back
                            {
                                yAxisIterator--;
                            }
                            else // X facing left, Z facing front
                            {
                                yAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Back: // Last Z facing left
                            if (castX == PlaneFacingDirection.Right) // Z facing back
                            {
                                yAxisIterator++;
                            }
                            else // X facing left, Z facing front
                            {
                                yAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Left: // Last Z facing front
                            if (castX == PlaneFacingDirection.Front) // Z facing right
                            {
                                yAxisIterator--;
                            }
                            else // X facing back, Z facing left
                            {
                                yAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Right: // Last Z facing back
                            if (castX == PlaneFacingDirection.Front) // Z facing right
                            {
                                yAxisIterator++;
                            }
                            else // X facing back, Z facing left
                            {
                                yAxisIterator--;
                            }
                            break;
                    }
                    break;
                case PlaneFacingDirection.Left: // Y facing left
                    switch (castXPrev)
                    {
                        case PlaneFacingDirection.Up: // Last Z facing front
                            if (castX == PlaneFacingDirection.Front) // Z facing down
                            {
                                yAxisIterator--;
                            }
                            else // X facing back, Z facing up
                            {
                                yAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Down: // Last Z facing back
                            if (castX == PlaneFacingDirection.Front) // Z facing down
                            {
                                yAxisIterator++;
                            }
                            else // X facing back, Z facing up
                            {
                                yAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Front: // Last Z facing down
                            if (castX == PlaneFacingDirection.Up) // Z facing front
                            {
                                yAxisIterator++;
                            }
                            else // X facing down, Z facing back
                            {
                                yAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Back: // Last Z facing up
                            if (castX == PlaneFacingDirection.Up) // Z facing front
                            {
                                yAxisIterator--;
                            }
                            else // X facing down, Z facing back
                            {
                                yAxisIterator++;
                            }
                            break;
                    }
                    break;
                case PlaneFacingDirection.Right: // Y facing right
                    switch (castXPrev)
                    {
                        case PlaneFacingDirection.Up: // Last Z facing back
                            if (castX == PlaneFacingDirection.Front) // Z facing up
                            {
                                yAxisIterator++;
                            }
                            else // X facing back, Z facing down
                            {
                                yAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Down: // Last Z facing front
                            if (castX == PlaneFacingDirection.Front) // Z facing up
                            {
                                yAxisIterator--;
                            }
                            else // X facing back, Z facing down
                            {
                                yAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Front: // Last Z facing up
                            if (castX == PlaneFacingDirection.Up) // Z facing back
                            {
                                yAxisIterator--;
                            }
                            else // X facing down, Z facing front
                            {
                                yAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Back: // Last Z facing down
                            if (castX == PlaneFacingDirection.Up) // Z facing back
                            {
                                yAxisIterator++;
                            }
                            else // X facing down, Z facing front
                            {
                                yAxisIterator--;
                            }
                            break;
                    }
                    break;
            }
        }
        // Rotation around the x-axis
        else if (castX == castXPrev && castZ != castZPrev && castY != castYPrev)
        {
            // Determine the current face the x-axis is facing
            // Then deterermine the last face the y-axis was facing
            // And last dertermine wich face the y-axis is currently facing
            switch (castX)
            {
                case PlaneFacingDirection.Front: // X facing front
                    switch (castZPrev)
                    {
                        case PlaneFacingDirection.Up: // Last Y facing right
                            if (castZ == PlaneFacingDirection.Right) // Y facing down
                            {
                                xAxisIterator--;
                            }
                            else // Z facing left, Y facing up
                            {
                                xAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Down: // Last Y facing left
                            if (castZ == PlaneFacingDirection.Right) // Y facing down
                            {
                                xAxisIterator++;
                            }
                            else // Z facing left, Y facing up
                            {
                                xAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Left: // Last Y facing up
                            if (castZ == PlaneFacingDirection.Up) // Y facing right
                            {
                                xAxisIterator--;
                            }
                            else // Z facing down, Y facing Left
                            {
                                xAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Right: // Last Y facing down
                            if (castZ == PlaneFacingDirection.Up) // Y facing right
                            {
                                xAxisIterator++;
                            }
                            else // Z facing down, Y facing left
                            {
                                xAxisIterator--;
                            }
                            break;
                    }
                    break;
                case PlaneFacingDirection.Back: // X facing back
                    switch (castZPrev)
                    {
                        case PlaneFacingDirection.Up: // Last Y facing left
                            if (castZ == PlaneFacingDirection.Right) // Y facing up
                            {
                                xAxisIterator++;
                            }
                            else // Z facing left, Y facing down
                            {
                                xAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Down: // Last Y facing right
                            if (castZ == PlaneFacingDirection.Right) // Y facing up
                            {
                                xAxisIterator--;
                            }
                            else // Z facing left, Y facing down
                            {
                                xAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Left: // Last Y facing down
                            if (castZ == PlaneFacingDirection.Up) // Y facing left
                            {
                                xAxisIterator++;
                            }
                            else // Z facing down, Y facing right
                            {
                                xAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Right: // Last Y facing up
                            if (castZ == PlaneFacingDirection.Up) // Y facing left
                            {
                                xAxisIterator--;
                            }
                            else // Z facing down, Y facing right
                            {
                                xAxisIterator++;
                            }
                            break;
                    }
                    break;
                case PlaneFacingDirection.Up: // X facing up
                    switch (castZPrev)
                    {
                        case PlaneFacingDirection.Front: // Last Y facing left
                            if (castZ == PlaneFacingDirection.Right) // Y facing front
                            {
                                xAxisIterator++;
                            }
                            else // Z facing left, Y facing back
                            {
                                xAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Back: // Last Y facing right
                            if (castZ == PlaneFacingDirection.Right) // Y facing front
                            {
                                xAxisIterator--;
                            }
                            else // Z facing left, Y facing back
                            {
                                xAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Left: // Last Y facing back
                            if (castZ == PlaneFacingDirection.Front) // Y facing left
                            {
                                xAxisIterator++;
                            }
                            else // Z facing back, Y facing right
                            {
                                xAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Right: // Last Y facing front
                            if (castZ == PlaneFacingDirection.Front) // Y facing left
                            {
                                xAxisIterator--;
                            }
                            else // Z facing back, Y facing right
                            {
                                xAxisIterator++;
                            }
                            break;
                    }
                    break;
                case PlaneFacingDirection.Down: // X facing down
                    switch (castZPrev)
                    {
                        case PlaneFacingDirection.Front: // Last Y facing right
                            if (castZ == PlaneFacingDirection.Right) // Y facing back
                            {
                                xAxisIterator--;
                            }
                            else // Z facing left, Y facing front
                            {
                                xAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Back: // Last Y facing left
                            if (castZ == PlaneFacingDirection.Right) // Y facing back
                            {
                                xAxisIterator++;
                            }
                            else // Z facing left, Y facing front
                            {
                                xAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Left: // Last Y facing front
                            if (castZ == PlaneFacingDirection.Front) // Y facing right
                            {
                                xAxisIterator--;
                            }
                            else // Z facing back, Y facing left
                            {
                                xAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Right: // Last Y facing back
                            if (castZ == PlaneFacingDirection.Front) // Y facing right
                            {
                                xAxisIterator++;
                            }
                            else // Z facing back, Y facing left
                            {
                                xAxisIterator--;
                            }
                            break;
                    }
                    break;
                case PlaneFacingDirection.Left: // X facing left
                    switch (castZPrev)
                    {
                        case PlaneFacingDirection.Up: // Last Y facing front
                            if (castZ == PlaneFacingDirection.Front) // Y facing down
                            {
                                xAxisIterator--;
                            }
                            else // Z facing back, Y facing up
                            {
                                xAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Down: // Last Y facing back
                            if (castZ == PlaneFacingDirection.Front) // Z facing down
                            {
                                xAxisIterator++;
                            }
                            else // Z facing back, Y facing up
                            {
                                xAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Front: // Last Y facing down
                            if (castZ == PlaneFacingDirection.Up) // Y facing front
                            {
                                xAxisIterator++;
                            }
                            else // Z facing down, Y facing back
                            {
                                xAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Back: // Last Y facing up
                            if (castZ == PlaneFacingDirection.Up) // Y facing front
                            {
                                xAxisIterator--;
                            }
                            else // Z facing down, Y facing back
                            {
                                xAxisIterator++;
                            }
                            break;
                    }
                    break;
                case PlaneFacingDirection.Right: // X facing right
                    switch (castZPrev)
                    {
                        case PlaneFacingDirection.Up: // Last Y facing back
                            if (castZ == PlaneFacingDirection.Front) // Y facing up
                            {
                                xAxisIterator++;
                            }
                            else // Z facing back, Y facing down
                            {
                                xAxisIterator--;
                            }
                            break;
                        case PlaneFacingDirection.Down: // Last Y facing front
                            if (castZ == PlaneFacingDirection.Front) // Y facing up
                            {
                                xAxisIterator--;
                            }
                            else // Z facing back, Y facing down
                            {
                                xAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Front: // Last Y facing up
                            if (castZ == PlaneFacingDirection.Up) // Y facing back
                            {
                                xAxisIterator--;
                            }
                            else // Z facing down, Y facing front
                            {
                                xAxisIterator++;
                            }
                            break;
                        case PlaneFacingDirection.Back: // Last Y facing down
                            if (castZ == PlaneFacingDirection.Up) // Y facing back
                            {
                                xAxisIterator++;
                            }
                            else // Z facing down, Y facing front
                            {
                                xAxisIterator--;
                            }
                            break;
                    }
                    break;
            }
        }
        /*
        // Rotation around the z-axis
        else if (castX != castXPrev && castZ == castZPrev && castY != castYPrev)
        {
            // Determine the current face the Z-axis is facing
            // Then deterermine the last face the x-axis was facing
            // And last dertermine wich face the x-axis is currently facing
            switch (castZ)
            {
                case PlaneFacingDirection.Front: // Z facing front
                    switch (castYPrev)
                    {
                        case PlaneFacingDirection.Up: // Last X facing right
                            if (castY == PlaneFacingDirection.Right) // X facing down
                            {
                                Debug.Log("Z-");
                            }
                            else // Y facing left, X facing up
                            {
                                Debug.Log("Z+");
                            }
                            break;
                        case PlaneFacingDirection.Down: // Last X facing left
                            if (castY == PlaneFacingDirection.Right) // X facing down
                            {
                                Debug.Log("Z+");
                            }
                            else // Y facing left, X facing up
                            {
                                Debug.Log("Z-");
                            }
                            break;
                        case PlaneFacingDirection.Left: // Last X facing up
                            if (castY == PlaneFacingDirection.Up) // X facing right
                            {
                                Debug.Log("Z-");
                            }
                            else // Y facing down, X facing Left
                            {
                                Debug.Log("Z+");
                            }
                            break;
                        case PlaneFacingDirection.Right: // Last X facing down
                            if (castY == PlaneFacingDirection.Up) // X facing right
                            {
                                Debug.Log("Z+");
                            }
                            else // Y facing down, X facing left
                            {
                                Debug.Log("Z-");
                            }
                            break;
                    }
                    break;
                case PlaneFacingDirection.Back: // Z facing back
                    switch (castYPrev)
                    {
                        case PlaneFacingDirection.Up: // Last X facing left
                            if (castY == PlaneFacingDirection.Right) // X facing up
                            {
                                Debug.Log("Z+");
                            }
                            else // Y facing left, X facing down
                            {
                                Debug.Log("Z-");
                            }
                            break;
                        case PlaneFacingDirection.Down: // Last X facing right
                            if (castY == PlaneFacingDirection.Right) // X facing up
                            {
                                Debug.Log("Z-");
                            }
                            else // Y facing left, X facing down
                            {
                                Debug.Log("Z+");
                            }
                            break;
                        case PlaneFacingDirection.Left: // Last X facing down
                            if (castY == PlaneFacingDirection.Up) // X facing left
                            {
                                Debug.Log("Z+");
                            }
                            else // Y facing down, X facing right
                            {
                                Debug.Log("Z-");
                            }
                            break;
                        case PlaneFacingDirection.Right: // Last X facing up
                            if (castY == PlaneFacingDirection.Up) // X facing left
                            {
                                Debug.Log("Z-");
                            }
                            else // Y facing down, X facing right
                            {
                                Debug.Log("Z+");
                            }
                            break;
                    }
                    break;
                case PlaneFacingDirection.Up: // Z facing up
                    switch (castYPrev)
                    {
                        case PlaneFacingDirection.Front: // Last X facing left
                            if (castY == PlaneFacingDirection.Right) // X facing front
                            {
                                Debug.Log("Z+");
                            }
                            else // Y facing left, X facing back
                            {
                                Debug.Log("Z-");
                            }
                            break;
                        case PlaneFacingDirection.Back: // Last X facing right
                            if (castY == PlaneFacingDirection.Right) // X facing front
                            {
                                Debug.Log("Z-");
                            }
                            else // Y facing left, X facing back
                            {
                                Debug.Log("Z+");
                            }
                            break;
                        case PlaneFacingDirection.Left: // Last X facing back
                            if (castY == PlaneFacingDirection.Front) // X facing left
                            {
                                Debug.Log("Z+");
                            }
                            else // Y facing back, X facing right
                            {
                                Debug.Log("Z-");
                            }
                            break;
                        case PlaneFacingDirection.Right: // Last X facing front
                            if (castY == PlaneFacingDirection.Front) // X facing left
                            {
                                Debug.Log("Z-");
                            }
                            else // Y facing back, X facing right
                            {
                                Debug.Log("Z+");
                            }
                            break;
                    }
                    break;
                case PlaneFacingDirection.Down: // Z facing down
                    switch (castYPrev)
                    {
                        case PlaneFacingDirection.Front: // Last X facing right
                            if (castY == PlaneFacingDirection.Right) // X facing back
                            {
                                Debug.Log("Z-");
                            }
                            else // Y facing left, X facing front
                            {
                                Debug.Log("Z+");
                            }
                            break;
                        case PlaneFacingDirection.Back: // Last X facing left
                            if (castY == PlaneFacingDirection.Right) // X facing back
                            {
                                Debug.Log("Z+");
                            }
                            else // Y facing left, X facing front
                            {
                                Debug.Log("Z-");
                            }
                            break;
                        case PlaneFacingDirection.Left: // Last X facing front
                            if (castY == PlaneFacingDirection.Front) // X facing right
                            {
                                Debug.Log("Z-");
                            }
                            else // Y facing back, X facing left
                            {
                                Debug.Log("Z+");
                            }
                            break;
                        case PlaneFacingDirection.Right: // Last X facing back
                            if (castY == PlaneFacingDirection.Front) // X facing right
                            {
                                Debug.Log("Z+");
                            }
                            else // Y facing back, X facing left
                            {
                                Debug.Log("Z-");
                            }
                            break;
                    }
                    break;
                case PlaneFacingDirection.Left: // Z facing left
                    switch (castYPrev)
                    {
                        case PlaneFacingDirection.Up: // Last X facing front
                            if (castY == PlaneFacingDirection.Front) // X facing down
                            {
                                Debug.Log("Z-");
                            }
                            else // Y facing back, X facing up
                            {
                                Debug.Log("Z+");
                            }
                            break;
                        case PlaneFacingDirection.Down: // Last X facing back
                            if (castY == PlaneFacingDirection.Front) // X facing down
                            {
                                Debug.Log("Z+");
                            }
                            else // Y facing back, X facing up
                            {
                                Debug.Log("Z-");
                            }
                            break;
                        case PlaneFacingDirection.Front: // Last X facing down
                            if (castY == PlaneFacingDirection.Up) // X facing front
                            {
                                Debug.Log("Z+");
                            }
                            else // Y facing down, X facing back
                            {
                                Debug.Log("Z-");
                            }
                            break;
                        case PlaneFacingDirection.Back: // Last X facing up
                            if (castY == PlaneFacingDirection.Up) // X facing front
                            {
                                Debug.Log("Z-");
                            }
                            else // Y facing down, X facing back
                            {
                                Debug.Log("Z+");
                            }
                            break;
                    }
                    break;
                case PlaneFacingDirection.Right: // Z facing right
                    switch (castYPrev)
                    {
                        case PlaneFacingDirection.Up: // Last X facing back
                            if (castY == PlaneFacingDirection.Front) // X facing up
                            {
                                Debug.Log("Z+");
                            }
                            else // Y facing back, X facing down
                            {
                                Debug.Log("Z-");
                            }
                            break;
                        case PlaneFacingDirection.Down: // Last X facing front
                            if (castY == PlaneFacingDirection.Front) // X facing up
                            {
                                Debug.Log("Z-");
                            }
                            else // Y facing back, X facing down
                            {
                                Debug.Log("Z+");
                            }
                            break;
                        case PlaneFacingDirection.Front: // Last X facing up
                            if (castY == PlaneFacingDirection.Up) // X facing back
                            {
                                Debug.Log("Z-");
                            }
                            else // Y facing down, X facing front
                            {
                                Debug.Log("Z+");
                            }
                            break;
                        case PlaneFacingDirection.Back: // Last X facing down
                            if (castY == PlaneFacingDirection.Up) // X facing back
                            {
                                Debug.Log("Z+");
                            }
                            else // Y facing down, X facing front
                            {
                                Debug.Log("Z-");
                            }
                            break;
                    }
                    break;
            }
        }
        */
    }
}
