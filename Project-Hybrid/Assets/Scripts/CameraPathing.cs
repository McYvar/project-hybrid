using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CameraPathing : MonoBehaviour
{
    [SerializeField] Animator animator = null;

    [SerializeField] float pullThreshold = 0;
    [SerializeField] float wait = 0;
    float timer = 0;
    bool hasPulled = false;

    [SerializeField] Transform mainCamera = null;
    [HideInInspector] public CurrentState state = 0;

    float moveInterpolation = 0;
    float rotationInterpolation = 0;
    [SerializeField] float moveInterpolationSpeed = 0;
    [SerializeField] float rotationInterpolationSpeed = 0;
    [SerializeField, Range(0f, 10f)] float standardInterpolation = 0;

    // hover state vars
    [Space(20), Header("Hover around")]
    [SerializeField] Vector3 cameraHoverPosition = Vector3.zero;
    [SerializeField] Vector3 cameraHoverRotation = Vector3.zero;
    float currentAroundTheTableYAxis = 0;

    // rotate disc vars
    [Space(20), Header("Rotating disc")]
    [SerializeField] Transform disc = null;
    [SerializeField] Vector3 cameraDiscPosition = Vector3.zero;
    [SerializeField] Vector3 cameraDiscRotation = Vector3.zero;
    [SerializeField] float puzzleSelectionWait = 0;
    LoadbarInstance currentLoadBar = null;
    PuzzlePiece currentPiece = null;
    float currentAroundTheDiscYAxis = 0;

    // Accessing puzzle vars
    Vector3 cameraPositionBeforeAccessingPuzzle = Vector3.zero;
    Quaternion cameraRotationBeforeAccessingPuzzle = Quaternion.identity;
    [SerializeField] RawImage image = null;
    [SerializeField] GameObject telescopeUI = null;
    [SerializeField] Vector3 cameraZoomPosition = Vector3.zero;
    [SerializeField] GameObject canvasPlanets = null;

    public static bool isFinished;
    bool firstTimeAcces = true;

    private void Start()
    {
        hasPulled = false;
        isFinished = false;
        firstTimeAcces = true;
    }

    private void Update()
    {
        if (isFinished)
        {
            // tilt camera up
            mainCamera.localRotation = Quaternion.Slerp(mainCamera.localRotation, Quaternion.Euler(-80, 0, 0), rotationInterpolation);
            return;
        }

        if (timer > wait)
        {
            Vector3 pull = new Vector3(SerialHandler.accX, SerialHandler.accY, SerialHandler.accZ);
            if (pull.magnitude > pullThreshold)
            {
                hasPulled = true;
                timer = 0;
            }
        }
        else
        {
            timer += Time.deltaTime;
        }

        if (state == CurrentState.stationaryOnPuzzle)
        {
            currentPiece.pieceControls?.UpdatePiece();
        }

        if (moveInterpolation < 1) moveInterpolation = moveInterpolationSpeed * Time.deltaTime;
        if (rotationInterpolation < 1) rotationInterpolation = rotationInterpolationSpeed * Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (isFinished) return;
        switch (state)
        {
            // STATE WHEN HOVERING AROUND THE TABLE SEARCHING FOR CLUES, ONLY USES THE Y AXIS AND NEEDS TO BE CALIBRATED USING SPACE
            case CurrentState.hoverAroundTable:
                animator.enabled = true;
                animator.SetInteger("GameState", 1);
                HoverAroundTableState();
                break;

            // STATE WHEN ROTATING THE DISC WITH THE FOUR PUZZLES, IF YOU LOOK AT THE DISC LONG ENOUGH YOU ENTER THE PUZZLE
            case CurrentState.rotateDisc:
                animator.enabled = true;
                animator.SetInteger("GameState", 2);
                RotateTableDiscState();
                break;

            case CurrentState.accessHighlightedPuzzle:
                animator.enabled = false;
                JustZoomIn();
                break;

            // Per puzzle piece the controls have to be different, here the controlls are implemented
            case CurrentState.stationaryOnPuzzle:

                if (hasPulled)
                {
                    state = CurrentState.leaveHighlightedPuzzle;
                    moveInterpolation = 0;
                    rotationInterpolation = 0;
                }

                animator.enabled = true;
                switch (currentPiece.pieceType)
                {
                    case PuzzlePieceType.telescope:
                        animator.SetInteger("GameState", 4);
                        break;
                    case PuzzlePieceType.rollTheBalls:
                        animator.SetInteger("GameState", 3);
                        break;
                    case PuzzlePieceType.planetsView:
                        animator.SetInteger("GameState", 2);
                        break;
                    case PuzzlePieceType.rotatingDiscs:
                        animator.SetInteger("GameState", 5);
                        break;
                }

                break;

            case CurrentState.leaveHighlightedPuzzle:
                animator.enabled = false;
                JustZoomOut();
                break;

        }

        hasPulled = false;
    }

    private void HoverAroundTableState()
    {
        mainCamera.localPosition = Vector3.Lerp(mainCamera.localPosition, cameraHoverPosition, standardInterpolation * Time.deltaTime);
        mainCamera.localRotation = Quaternion.Slerp(mainCamera.localRotation, Quaternion.Euler(cameraHoverRotation), standardInterpolation * Time.deltaTime);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, currentAroundTheTableYAxis + SerialHandler.rotY, 0), standardInterpolation * Time.deltaTime);

        if (hasPulled)
        {
            state = CurrentState.rotateDisc;
            currentAroundTheDiscYAxis = disc.localEulerAngles.y - SerialHandler.rotY;
        }
    }

    private void RotateTableDiscState()
    {
        if (currentLoadBar != null) currentLoadBar.isActive = false;

        // Rotating the main disc around the y-axis
        mainCamera.localPosition = Vector3.Lerp(mainCamera.localPosition, cameraDiscPosition, standardInterpolation * Time.deltaTime);
        mainCamera.localRotation = Quaternion.Slerp(mainCamera.localRotation, Quaternion.Euler(cameraDiscRotation), standardInterpolation * Time.deltaTime);

        if (firstTimeAcces)
        {
            currentAroundTheDiscYAxis = mainCamera.localEulerAngles.y + 180;
            firstTimeAcces = false;
        }
        disc.localRotation = Quaternion.Slerp(disc.localRotation, Quaternion.Euler(0, currentAroundTheDiscYAxis + SerialHandler.rotY, 0), standardInterpolation * Time.deltaTime);

        // timed part while waiting to select the puzzle
        RaycastHit hit;
        int layermask = 1 << 6;
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, 10, layermask))
        {
            if (!hit.Equals(null)) currentPiece = hit.collider.GetComponent<PuzzlePiece>();
            if (currentPiece != null)
            {
                currentLoadBar = currentPiece.loadBar;
                currentLoadBar.waitTime = puzzleSelectionWait;
                currentLoadBar.isActive = true;
            }
        }

        if (hasPulled)
        {
            state = CurrentState.hoverAroundTable;
            currentAroundTheTableYAxis = transform.localEulerAngles.y - SerialHandler.rotY;
        }

        if (currentLoadBar != null && currentPiece != null)
        {
            cameraPositionBeforeAccessingPuzzle = mainCamera.position;
            cameraRotationBeforeAccessingPuzzle = mainCamera.rotation;
            if (currentLoadBar.confirm)
            {
                state = CurrentState.accessHighlightedPuzzle;
                moveInterpolation = 0;
                rotationInterpolation = 0;
                currentLoadBar.confirm = false;
                currentLoadBar.isActive = false;
            }
        }
    }

    private void JustZoomIn()
    {
        mainCamera.localPosition = Vector3.Lerp(mainCamera.localPosition, cameraZoomPosition, moveInterpolation);
        mainCamera.rotation = Quaternion.Slerp(mainCamera.rotation, Quaternion.LookRotation(currentPiece.transform.position - transform.rotation * cameraZoomPosition), rotationInterpolation);

        if (Vector3.Distance(mainCamera.position, transform.rotation * cameraZoomPosition) < 0.1f)
        {
            state = CurrentState.stationaryOnPuzzle;
            currentPiece.pieceControls?.EnterPiece();
            if (currentPiece.pieceType == PuzzlePieceType.telescope) telescopeUI.SetActive(true);
            image.enabled = false;
            canvasPlanets.SetActive(true);
        }

        if (currentPiece.pieceType == PuzzlePieceType.telescope)
            image.color = new Color(0, 0, 0, Mathf.Lerp(image.color.a, 1, Vector3.Distance(mainCamera.position, cameraPositionBeforeAccessingPuzzle)));
    }

    private void JustZoomOut()
    {
        image.enabled = true;
        mainCamera.position = Vector3.Lerp(mainCamera.position, cameraPositionBeforeAccessingPuzzle, moveInterpolation);
        mainCamera.rotation = Quaternion.Slerp(mainCamera.rotation, cameraRotationBeforeAccessingPuzzle, rotationInterpolation);
        if (Vector3.Distance(mainCamera.position, cameraPositionBeforeAccessingPuzzle) < 0.1f)
        {
            state = CurrentState.hoverAroundTable;
            if (currentPiece.pieceControls.ExitPiece())
            {
                // If puzzle is completed
                currentPiece.OnPuzzleSolved();
                currentPiece.OnPuzzleCompletion.Invoke();
            }

            currentPiece.loadBar.cooldown = 0;
            currentPiece.loadBar.confirm = false;
            currentAroundTheTableYAxis = transform.localEulerAngles.y - SerialHandler.rotY;
        }

        if (currentPiece.pieceType == PuzzlePieceType.telescope)
        {
            image.color = new Color(0, 0, 0, Mathf.Lerp(image.color.a, 0, 1 - Vector3.Distance(mainCamera.position, cameraPositionBeforeAccessingPuzzle)));
            telescopeUI.SetActive(false);
            canvasPlanets.SetActive(false);
        }
    }
}

public enum CurrentState { hoverAroundTable, rotateDisc, accessHighlightedPuzzle, stationaryOnPuzzle, leaveHighlightedPuzzle }
