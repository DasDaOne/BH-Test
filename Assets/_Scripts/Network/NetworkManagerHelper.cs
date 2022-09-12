using Mirror;
using UnityEngine;
using Utilities.Singletons;

public class NetworkManagerHelper : NetworkSingleton<NetworkManagerHelper>
{
    [SerializeField] private ScoreboardController scoreboardController;
    
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isClientOnly)
        {
            NetworkHudManager.Instance.ChangeConnectionStatus($"Successfully connected to the server.\nIP: \"{NetworkManager.singleton.networkAddress}\"");
        }
        else if (isClient)
        {
            NetworkHudManager.Instance.ChangeConnectionStatus($"Host started successfully.");
        }

        if (isClient)
        {
            scoreboardController.RegisterHandlers();
            NetworkHudManager.Instance.RegisterServerClosedHandler();
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        
        if (isServerOnly)
        {
            NetworkHudManager.Instance.ChangeConnectionStatus($"Server started successfully.");
        }
    }
}
