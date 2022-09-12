using Mirror;
using UnityEngine;
public class DashColliderChecker : NetworkBehaviour
{
    private bool isChecking;
    
    [Command]
    public void CmdStartChecking()
    {
        isChecking = true;
    }

    [Command]
    public void CmdStopChecking()
    {
        isChecking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!isChecking || !isServer) return;

        if (other.TryGetComponent(out PlayerDamage playerDamage))
        {
            playerDamage.TryDamage(gameObject);
        }
    }
}
