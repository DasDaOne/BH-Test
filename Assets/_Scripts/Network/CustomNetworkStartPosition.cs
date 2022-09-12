using Mirror;
using UnityEngine;

public class CustomNetworkStartPosition : MonoBehaviour
{
    private void Awake()
    {
        NetworkManager.RegisterStartPosition(transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        NetworkManager.UnRegisterStartPosition(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        NetworkManager.RegisterStartPosition(transform);
    }
}
