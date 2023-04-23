using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuCubeAnimationState : MonoBehaviour
{
    [SerializeField] int startState = 0;

    private void Start()
    {
        GetComponent<Animator>().SetInteger("GameState", startState);
    }
}
