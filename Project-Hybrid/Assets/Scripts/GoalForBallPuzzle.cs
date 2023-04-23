using System.Collections.Generic;
using UnityEngine;

public class GoalForBallPuzzle : MonoBehaviour
{
    public static bool completed;
    [SerializeField] BallType forType = 0;
    [SerializeField] CameraPathing pathing = null;

    [SerializeField] List<GoalForBallPuzzle> othergoals = null;

    [HideInInspector] public bool imIn;

    [SerializeField] float jumpForce = 0;

    private void Start()
    {
        completed = false;
    }

    private void Update()
    {
        int amountInGoals = 0;
        if (othergoals.Count < 3) return;
        foreach (GoalForBallPuzzle goal in othergoals)
        {
            if (goal.imIn) amountInGoals++;
        }

        if (amountInGoals >= 3)
        {
            completed = true;
            pathing.state = CurrentState.leaveHighlightedPuzzle;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        BallPuzzleBalls current = other.GetComponent<BallPuzzleBalls>();
        if (current != null)
        {
            if (forType == current.type)
            {
                imIn = true;
            }
            else
            {
                current.rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BallPuzzleBalls current = other.GetComponent<BallPuzzleBalls>();
        if (current != null)
        {
            if (forType == current.type)
            {
                imIn = false;
            }
        }
    }
}
