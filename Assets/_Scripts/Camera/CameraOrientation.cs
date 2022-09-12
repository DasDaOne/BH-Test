using UnityEngine;

public class CameraOrientation : MonoBehaviour
{
    private void Update()
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
