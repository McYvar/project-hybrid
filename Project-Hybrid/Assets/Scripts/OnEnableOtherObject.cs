using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableOtherObject : MonoBehaviour
{
    [SerializeField] GameObject[] otherObjects = null;

    private void OnEnable()
    {
        foreach (GameObject obj in otherObjects)
        {
            obj.SetActive(true);
        }
    }
}
