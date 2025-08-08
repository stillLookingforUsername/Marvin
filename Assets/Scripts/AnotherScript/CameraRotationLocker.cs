using UnityEngine;

[DefaultExecutionOrder(10000)]
public class CameraRotationLocker : MonoBehaviour
{
    [SerializeField] public bool lockX = true;
    [SerializeField] public bool lockY = true;
    [SerializeField] public bool lockZ = true;
    [SerializeField] public Vector3 lockedEulerAngles = Vector3.zero;

    private void LateUpdate()
    {
        Vector3 euler = transform.eulerAngles;
        if (lockX) euler.x = lockedEulerAngles.x;
        if (lockY) euler.y = lockedEulerAngles.y;
        if (lockZ) euler.z = lockedEulerAngles.z;
        transform.rotation = Quaternion.Euler(euler);
    }
}