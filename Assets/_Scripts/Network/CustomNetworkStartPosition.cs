using Mirror;
using UnityEngine;

public class CustomNetworkStartPosition : NetworkStartPosition
{ 
    private void OnTriggerEnter(Collider other)
    {
        NetworkManager.UnRegisterStartPosition(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        NetworkManager.RegisterStartPosition(transform);
    }
}
