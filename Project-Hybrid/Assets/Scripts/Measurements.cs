using UnityEngine;
using UnityEngine.UI;

public class Measurements : MonoBehaviour
{
    [SerializeField] Transform plane = null;
    [SerializeField] Transform xRectTransform = null;
    [SerializeField] Transform yRectTransform = null;

    [SerializeField] float leftOffset = 0;
    [SerializeField] float rightOffset = 0;
    [SerializeField] float downOffset = 0;
    [SerializeField] float upOffset = 0;

    [SerializeField] Transform followingObject = null;

    Vector2 min;

    private void Start()
    {
        min = new Vector2(plane.position.x - 0.5f * plane.localScale.x / 10, plane.position.z - 0.5f * plane.localScale.z / 10);
    }

    private void Update()
    {
        Vector2 objectCurrentPosition = new Vector2(followingObject.position.x, followingObject.position.z) - min;
        Vector2 delta = new Vector2(objectCurrentPosition.x / plane.localScale.x, objectCurrentPosition.y / plane.localScale.z);
        delta = new Vector2(((delta.x / 5) + 1) / 2, ((delta.y / 5) + 1) / 2);  


        xRectTransform.localPosition = new Vector3(Mathf.Lerp(leftOffset, rightOffset, delta.x), xRectTransform.localPosition.y, xRectTransform.position.z);
        yRectTransform.localPosition = new Vector3(yRectTransform.localPosition.x, Mathf.Lerp(upOffset, downOffset, delta.y), yRectTransform.position.z);
    }
}
