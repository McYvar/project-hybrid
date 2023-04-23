using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.VFX;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] Animator animator = null;
    [SerializeField] LoadbarInstance loadbarFromTelescope = null;
    
    [SerializeField] AudioSource hintsAudioSource = null;
    [SerializeField] AudioClip audioOnHints = null;

    [SerializeField] MeshRenderer tableMaterial = null;
    [SerializeField] GameObject telescopeFX = null;

    [SerializeField] VisualEffect vsfxStars = null;
    [SerializeField] GameObject[] animCube = null;

    [SerializeField] FlipUp flipUpPlanets = null;
    [SerializeField] MeshRenderer[] puzzleBalls;

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space)) OnCompletingDiscsPuzzle();
        
    }

    public void OnCompletingBallPuzzle()
    {
        // Show the hints, play a sound
        animator.SetBool("Hints", true);
        loadbarFromTelescope.hasLock = false;
        hintsAudioSource.PlayOneShot(audioOnHints);
        telescopeFX.SetActive(true);
        foreach (MeshRenderer ball in puzzleBalls)
        {
            ball.enabled = false;
        }
    }

    public void OnCompletingDiscsPuzzle()
    {
        // Show the roof openening
        CameraPathing.isFinished = true;
        animator.SetBool("Dak", true);
        vsfxStars.SetBool("OnDakOpenen", true);
        foreach (GameObject cube in animCube)
        {
            cube.SetActive(false);
        }
    }

    public void OnCompletingTelescopePuzzle()
    {
        animator.SetBool("Hints", false);
        hintsAudioSource.PlayOneShot(audioOnHints);
        animator.SetBool("LightPlanets", true);
        tableMaterial.material.SetFloat("_EmissionSwitch", 1);
        flipUpPlanets.doFlip = true;
    }
}
