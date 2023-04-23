using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadbarInstance : MonoBehaviour
{
    [HideInInspector] public bool isActive = false;
    [HideInInspector] public bool confirm = false;
    [HideInInspector] public float waitTime = 0;
    [Range(0f, 1f)] public float status = 0;

    [SerializeField] Color color = new Color(1, 1, 1);
    [SerializeField] Color lockedColor = new Color(0, 0, 0);
    [SerializeField] float timerReductionSpeed = 1;
    [SerializeField] Transform objectToFollow = null;
    [SerializeField] bool useRotation = false;
    [SerializeField] Transform rotationPivot = null;

    GameObject instance = null;
    MeshRenderer meshRenderer = null;
    Material material = null;

    [SerializeField] bool StayOn = false;
    [SerializeField] float offsetY = 0;

    public float cooldown = 0;
    float cooldownTime = 2f;

    public bool isFinished = false;
    Quaternion originalRotation = Quaternion.identity;

    public bool hasLock = false;
    [SerializeField] bool useLockedColor = false;
    [SerializeField] float intensity = 1;

    private void Start()
    {
        instance = this.gameObject;
        meshRenderer = instance.GetComponent<MeshRenderer>();
        material = meshRenderer.material;
        if (StayOn) waitTime = 1;
        else status = 0;
        isActive = false;
        confirm = false;
        isFinished = false;

        cooldown = cooldownTime;
        originalRotation = transform.localRotation;

    }

    private void Update()
    {
        material.SetFloat("_Intensity", intensity);
        if (cooldown < cooldownTime && !StayOn)
        {
            cooldown += Time.deltaTime;
            status = 0;
            material.SetFloat("_Status", 0);
            meshRenderer.enabled = false;
            return;
        }

        if (objectToFollow != null) transform.position = objectToFollow.position + Vector3.up * offsetY;
        if (useRotation) transform.rotation = Quaternion.LookRotation(rotationPivot.forward) * originalRotation;

        if (isFinished) return;
        if (hasLock)
        {
            if (useLockedColor)
            {
                material.color = lockedColor;
                material.SetFloat("_Status", 1);
            }
            return;
        }

        material.color = color;
        status = Mathf.Clamp(status, 0f, waitTime);
        material.SetFloat("_Status", status / waitTime);
        
        if (status == 0) meshRenderer.enabled = false;
        else meshRenderer.enabled = true;

        if (StayOn) return;

        if (isActive && !confirm)
        {
            if (status == waitTime) confirm = true;

            status += Time.deltaTime;
        }
        else
        {
            status -= Time.deltaTime * timerReductionSpeed;
        }
    }
}
