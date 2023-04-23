using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(SphereCollider)), SelectionBase]
public class BallPuzzleBalls : MonoBehaviour
{
    public static bool canPlay;
    public static bool reset;

    public BallType type;
    public Rigidbody rb;

    [SerializeField] Transform centreDisc = null;
    Vector3 spawn;

    [SerializeField] float maxDist = 0;

    private void Start()
    {
        canPlay = false;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        spawn = transform.localPosition;
    }

    private void Update()
    {
        if (canPlay) rb.useGravity = true;
        else rb.useGravity = false;

        if (rb.velocity == Vector3.zero) rb.velocity = Vector3.down * 0.01f;

        if (Vector3.Distance(transform.position, centreDisc.position) > maxDist || reset)
        {
            transform.localPosition = spawn;
            rb.velocity = Vector3.zero; 

            if (reset) reset = false;
        }
    }
}

public enum BallType { tiny = 0, medium = 1, large = 2 }