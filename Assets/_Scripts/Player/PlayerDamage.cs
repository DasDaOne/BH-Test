using Mirror;
using UnityEngine;
using Utilities;

public class PlayerDamage : NetworkBehaviour
{
    [SerializeField] private float invulnerabilityTime;
    [SerializeField] private PlayerVisualController playerVisualController;
    
    private bool isInvulnerable;
    
    [Server]
    public void TryDamage(GameObject sender)
    {
        if(isInvulnerable || !ServerController.Instance.GameIsRunning) return;
        
        isInvulnerable = true;
        playerVisualController.playerCurrentMaterial = PlayerMaterial.DamagedMaterial;
        StartCoroutine(TimeUtilities.Timer(invulnerabilityTime, ResetInvulnerability));
        
        ServerController.Instance.AddScore(sender.GetComponent<NetworkIdentity>().connectionToClient.connectionId);
    }
    
    [Server]
    private void ResetInvulnerability()
    {
        isInvulnerable = false;
        playerVisualController.playerCurrentMaterial = PlayerMaterial.DefaultMaterial;
    }
}
