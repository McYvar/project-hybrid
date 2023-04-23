using UnityEngine;

public class MotionObject : MonoBehaviour
{
    enum Axis { x, y, z };
    [SerializeField] Axis axis = Axis.x;
    Quaternion startRotation = Quaternion.identity;

    private void Start()
    {
        startRotation = transform.localRotation;
    }

    private void Update()
    {
        switch (axis)
        {
            case Axis.x:
                transform.localRotation = Quaternion.Euler(new Vector3(startRotation.eulerAngles.x + SerialHandler.rotX, transform.localEulerAngles.y, startRotation.eulerAngles.z));
                break;
            case Axis.y:
                transform.localRotation = Quaternion.Euler(new Vector3(startRotation.eulerAngles.x, startRotation.eulerAngles.y + SerialHandler.rotY, startRotation.eulerAngles.z));
                break;
            case Axis.z:
                transform.localRotation = Quaternion.Euler(new Vector3(startRotation.eulerAngles.x, startRotation.eulerAngles.y, startRotation.eulerAngles.z + SerialHandler.rotZ));
                break;
        }
    }
}
