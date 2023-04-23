using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class PuzzlePiece : MonoBehaviour
{
    public UnityEvent OnPuzzleCompletion = null;

    public PuzzlePieceType pieceType = PuzzlePieceType.telescope;
    public LoadbarInstance loadBar = null;
    [HideInInspector] public bool isSolved;
    Quaternion startingRotation = Quaternion.identity;
    float randomZOffset = 0;

    public PuzzlePieceControlsBaseClass pieceControls = null;
    [SerializeField] Transform[] objectsOfInfulence = null;

    [SerializeField] LoadbarInstance[] loadbarsForDicsPuzzle = null;

    [SerializeField] PhysicalMenuSelectorBall physicalMenuSelectorBall = null;
    [SerializeField] Transform mainCamera = null;

    [SerializeField] GameObject[] bodiesForTelescopePuzzle = null;
    [SerializeField] GameObject UICanvasCamera = null;

    [SerializeField] CameraPathing pathing = null;

    private void Awake()
    {
        switch (pieceType)
        {
            case PuzzlePieceType.telescope:
                pieceControls = new TelescopeSeekingPuzzlePiece(objectsOfInfulence, physicalMenuSelectorBall, mainCamera, bodiesForTelescopePuzzle, UICanvasCamera, pathing);
                break;
            case PuzzlePieceType.rollTheBalls:
                pieceControls = new BallsRotationPuzzle(objectsOfInfulence, mainCamera);
                break;
            case PuzzlePieceType.planetsView:
                pieceControls = new PlanetsView(objectsOfInfulence);
                break;
            case PuzzlePieceType.rotatingDiscs:
                pieceControls = new DiscRotationPuzzlePiece(objectsOfInfulence, loadbarsForDicsPuzzle, pathing);
                break;
        }
    }

    private void Start()
    {
        isSolved = false;
        startingRotation = transform.localRotation;
        SphereCollider s = GetComponent<SphereCollider>();
        s.radius = 0.0025f;
        s.isTrigger = true;
        randomZOffset = Random.Range(0, 180);
    }

    private void Update()
    {
        if (!isSolved) return;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(startingRotation.eulerAngles.x, startingRotation.eulerAngles.y + 180, startingRotation.eulerAngles.z + randomZOffset), 0.005f);
    }

    public void OnPuzzleSolved()
    {
        isSolved = true;
        loadBar.confirm = true;
        loadBar.isFinished = true;
    }
}

public enum PuzzlePieceType { telescope = 0, rollTheBalls = 1, planetsView = 2, rotatingDiscs = 3 }
