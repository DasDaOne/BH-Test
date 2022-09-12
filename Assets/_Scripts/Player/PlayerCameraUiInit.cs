using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] private CameraMovement connectedCamera;

    private void Start()
    {
        if(!isLocalPlayer) return;

        CameraManager.Instance.InitializeCamera(connectedCamera);
        UiPanelManager.Instance.SetPanelState(PanelId.Scoreboard, true);
        UiPanelManager.Instance.SetPanelState(PanelId.WinnerNotification, false);
    }

    public override void OnStopLocalPlayer()
    {
        base.OnStopLocalPlayer();
        
        UiPanelManager.Instance.SetPanelState(PanelId.Scoreboard, false);
        
        CameraManager.Instance.ForgetCamera(connectedCamera);
    }
}
