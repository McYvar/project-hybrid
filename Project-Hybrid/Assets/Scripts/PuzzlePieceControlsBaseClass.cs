using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class PuzzlePieceControlsBaseClass
{
    protected Transform[] objects;
    protected bool completed;

    public PuzzlePieceControlsBaseClass(Transform[] objects)
    {
        this.objects = objects;
        completed = false;
    }
    abstract public void EnterPiece();
    abstract public void UpdatePiece();
    abstract public bool ExitPiece();
}

public class DiscRotationPuzzlePiece : PuzzlePieceControlsBaseClass
{
    Transform currentDisc;
    int[] currentNumber;
    int[] lastNumber;
    int currentDiscIndex;
    int lastDiscIndex;
    int availableNumbers = 1;
    float waitTime = 1f;
    float timer = 0;
    Quaternion currentRotation;
    LoadbarInstance[] instances;

    Transform disc0, disc1, disc2, disc3;
    CameraPathing pathing;

    public DiscRotationPuzzlePiece(Transform[] objects, LoadbarInstance[] loadBarInstances, CameraPathing pathing) : base(objects) 
    {
        if (objects.Length > 0) currentDisc = objects[0];
        currentDiscIndex = 0;

        currentNumber = new int[objects.Length];
        lastNumber = new int[objects.Length];
        instances = loadBarInstances;
        instances[0].status = 1;

        disc0 = objects[0];
        disc1 = objects[1];
        disc2 = objects[2];
        disc3 = objects[3];

        currentRotation = disc0.localRotation;

        currentNumber[0] = Mathf.RoundToInt(disc0.localRotation.eulerAngles.z / 30);
        currentNumber[1] = Mathf.RoundToInt(disc1.localRotation.eulerAngles.z / 45);
        currentNumber[2] = Mathf.RoundToInt(disc2.localRotation.eulerAngles.z / 45);
        currentNumber[3] = Mathf.RoundToInt(disc3.localRotation.eulerAngles.z / 90);

        this.pathing = pathing;
    }

    public override void EnterPiece()
    {
    }

    public override bool ExitPiece()
    {
        return completed;
    }

    public override void UpdatePiece()
    {
        if (timer > waitTime) {
            currentDiscIndex += Facing.xAxisIterator;
            currentDiscIndex = currentDiscIndex < 0 ? 4 + currentDiscIndex : currentDiscIndex;
            currentDiscIndex %= 4;

            currentDisc = objects[currentDiscIndex];

            int n = 1;
            if (currentDiscIndex == 2) n = 2;
            else if (currentDiscIndex == 1) n = 2;
            else if (currentDiscIndex == 0) n = 3;

            availableNumbers = n * 4;

            currentNumber[currentDiscIndex] += Facing.yAxisIterator;
            currentNumber[currentDiscIndex] = currentNumber[currentDiscIndex] < 0 ? availableNumbers + currentNumber[currentDiscIndex] : currentNumber[currentDiscIndex];
            currentDiscIndex %= availableNumbers;

            if (currentDiscIndex != lastDiscIndex || currentNumber[currentDiscIndex] != lastNumber[currentDiscIndex])
            {
                timer = 0;
                currentRotation = currentDisc.localRotation;
            }
        }
        else
        {
            instances[lastDiscIndex].status = 0;
            instances[currentDiscIndex].status = 1;

            timer += Time.deltaTime;
            Facing.reset = true;
            lastDiscIndex = currentDiscIndex;
            lastNumber[currentDiscIndex] = currentNumber[currentDiscIndex];
        }

        currentDisc.localRotation = Quaternion.Slerp(currentRotation,
            Quaternion.Euler(currentDisc.localRotation.eulerAngles.x, currentDisc.localRotation.eulerAngles.x, currentNumber[currentDiscIndex] * (360.0f / availableNumbers)),
            timer);

        int rotatedCorrectly = 0;
        if (disc0.localRotation.eulerAngles.z % 360 > 89 && disc0.localRotation.eulerAngles.z % 360 < 91) rotatedCorrectly++;
        if (disc1.localRotation.eulerAngles.z % 360 > 89 && disc1.localRotation.eulerAngles.z % 360 < 91) rotatedCorrectly++;
        if (disc2.localRotation.eulerAngles.z % 360 > 179 && disc2.localRotation.eulerAngles.z % 360 < 181) rotatedCorrectly++;
        if (disc3.localRotation.eulerAngles.z % 360 > 89 && disc3.localRotation.eulerAngles.z % 360 < 91) rotatedCorrectly++;

        if (rotatedCorrectly >= 4)
        {
            completed = true;
            pathing.state = CurrentState.leaveHighlightedPuzzle;
        }
        else completed = false;
    }
}

public class TelescopeSeekingPuzzlePiece : PuzzlePieceControlsBaseClass
{
    Transform mainCamera;
    Transform ball;
    PhysicalMenuSelectorBall physicalMenuSelectorBall;
    GameObject[] astralBodies;
    GameObject UICanvasCamera;

    static int bodies;
    static int currentBody;

    float timer;
    static bool cooldown;

    static List<int> found = new List<int>();
    CameraPathing pathing;

    public TelescopeSeekingPuzzlePiece(Transform[] objects, 
        PhysicalMenuSelectorBall physicalMenuSelectorBall,
        Transform mainCamera,
        GameObject[] astralBodies,
        GameObject UICanvasCamera,
        CameraPathing pathing) : base(objects) 
    {
        this.physicalMenuSelectorBall = physicalMenuSelectorBall;
        physicalMenuSelectorBall.enabled = false;
        this.mainCamera = mainCamera;
        if (objects.Length > 0) ball = objects[0];
        this.astralBodies = astralBodies;
        this.UICanvasCamera = UICanvasCamera;
        currentBody = -1;
        found.Clear();
        this.pathing = pathing;
        bodies = 0;
    }

    public override void EnterPiece()
    {
        physicalMenuSelectorBall.enabled = true;
        UICanvasCamera.SetActive(true);
    }

    public override bool ExitPiece()
    {
        physicalMenuSelectorBall.enabled = false;
        UICanvasCamera.SetActive(false);
        return completed;
    }

    public override void UpdatePiece()
    {
        if (currentBody != -1 && currentBody < astralBodies.Length)
        {
            astralBodies[currentBody].SetActive(true);
            currentBody = -1;
        }

        if (timer < 3)
        {
            timer += Time.deltaTime;
            cooldown = true;
        }
        else
        {
            cooldown = false;
        }

        Debug.Log(bodies);
        if (bodies >= 4)
        {
            completed = true;
            pathing.state = CurrentState.leaveHighlightedPuzzle;
        }

        mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
        mainCamera.transform.position = ball.transform.position + (Vector3.up * 5) + new Vector3(0.3285293f, 0, -0.24946022f);
    }

    public static void BodyFound(int index)
    {
        if (cooldown) return;
        foreach  (int body in found)
        {
            if (index == body) return;
        }
        found.Add(index);
        currentBody = index;
        bodies++;
    }
}

public class BallsRotationPuzzle : PuzzlePieceControlsBaseClass
{
    Quaternion startRotation;
    Transform mainCamera;

    public BallsRotationPuzzle(Transform[] objects, Transform mainCamera) : base(objects) 
    { 
        if (objects.Length > 0) startRotation = objects[0].localRotation;
        this.mainCamera = mainCamera;
    }

    public override void EnterPiece()
    {
        objects[0].localRotation = startRotation;
        BallPuzzleBalls.canPlay = true;
        BallPuzzleBalls.reset = true;
    }

    public override bool ExitPiece()
    {
        completed = GoalForBallPuzzle.completed;
        objects[0].localRotation = startRotation;
        BallPuzzleBalls.canPlay = false;
        BallPuzzleBalls.reset = true;
        return completed;
    }

    public override void UpdatePiece()
    {
        Quaternion result;
        if (SerialHandler.usesPS4) result = Quaternion.Euler(-SerialHandler.rotX, SerialHandler.rotZ, startRotation.z);
        else result = Quaternion.Euler(-SerialHandler.roll + 90, -SerialHandler.pitch, startRotation.z);

        objects[0].localRotation = Quaternion.Slerp(objects[0].localRotation,
            Quaternion.Euler(0, 0, mainCamera.localRotation.eulerAngles.y) *
            result, 0.1f);
    }
}

public class PlanetsView : PuzzlePieceControlsBaseClass
{
    Transform rotatingObject;
    Quaternion startingRotation;

    public PlanetsView(Transform[] objects) : base(objects) 
    {
        rotatingObject = objects[0];
    }

    public override void EnterPiece()
    {
        startingRotation = rotatingObject.localRotation;
    }

    public override bool ExitPiece()
    {
        return completed;
    }

    public override void UpdatePiece()
    {
        rotatingObject.localRotation = startingRotation * Quaternion.Euler(0, 0, SerialHandler.rotY);
    }
}