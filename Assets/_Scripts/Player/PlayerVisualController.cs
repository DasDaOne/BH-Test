using Mirror;
using UnityEngine;

public enum PlayerMaterial
{
    DefaultMaterial,
    DamagedMaterial
}

public class PlayerVisualController : NetworkBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material damagedMaterial;
    
    [SyncVar(hook = nameof(SetMaterial))] public PlayerMaterial playerCurrentMaterial;

    private void SetMaterial(PlayerMaterial oldMaterial, PlayerMaterial newMaterial)
    {
        switch (newMaterial)
        {
            case PlayerMaterial.DefaultMaterial:
                meshRenderer.material = defaultMaterial;
                break;
            case PlayerMaterial.DamagedMaterial:
                meshRenderer.material = damagedMaterial;
                break;
        }
    }
}
