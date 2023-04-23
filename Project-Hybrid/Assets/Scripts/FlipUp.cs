using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipUp : MonoBehaviour
{
    public bool doFlip = false;
    Quaternion startRotation = Quaternion.identity;
    float timer = 0;
    [SerializeField] float flipSpeed = 1;

    private void Start()
    {
        startRotation = transform.localRotation;
    }

    private void Update()
    {
        if (doFlip)
        {
            transform.localRotation = Quaternion.Slerp(startRotation, Quaternion.Euler(0, 0, 90), timer);
            timer += Time.deltaTime * flipSpeed;
        }
    }
}
