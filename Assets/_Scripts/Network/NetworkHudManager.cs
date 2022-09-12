using System;
using System.Linq;
using kcp2k;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.Singletons;

public class NetworkHudManager : Singleton<NetworkHudManager>
{
    [Header("Connection Panel Components")] 
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField ipAddressInputField;
    [SerializeField] private Button connectButton;
    [SerializeField] private TextMeshProUGUI connectButtonText;
    private string connectButtonDefaultText;

    [Header("Status Panel Components")]
    [SerializeField] private TextMeshProUGUI connectionStatus;
    [SerializeField] private TextMeshProUGUI disconnectButtonText;

    [Header("Other")]
    [SerializeField] private KcpTransport transport;

    private string playerName;
    private string ipAddress;

    public string PlayerName => playerName;

    private void Start()
    {
        playerName = PlayerPrefs.GetString("PlayerName", "");
        playerNameInputField.SetTextWithoutNotify(playerName);

        ipAddress = PlayerPrefs.GetString("LastIpAddress", "");
        ipAddressInputField.SetTextWithoutNotify(ipAddress);

        connectButtonDefaultText = connectButtonText.text;
    }

    #region Utilities
    private void SwitchPanels(PanelId panel)
    {
        switch (panel)
        {
            case PanelId.Connection:
                UiPanelManager.Instance.SetPanelState(PanelId.Status, false);
                UiPanelManager.Instance.SetPanelState(PanelId.Connection, true);
                break;
            case PanelId.Status:
                UiPanelManager.Instance.SetPanelState(PanelId.Connection, false);
                UiPanelManager.Instance.SetPanelState(PanelId.Status, true);
                break;
        }
    }

    private void StartTimeoutCheck()
    {
        StartCoroutine(TimeUtilities.Timer((transport.Timeout + 1) / 1000, () =>
        {
            if (!NetworkClient.isConnected)
                ChangeConnectionStatus("Timeout.");
        }));
    }

    public void StopTimeoutCheck()
    {
        StopAllCoroutines();
    }
    
    #endregion

    #region ConnectionPannel

    #region Connection
    public void StartHost()
    {
        NetworkManager.singleton.StartHost();
        SwitchPanels(PanelId.Status);
        disconnectButtonText.text = "Stop host";

        ChangeConnectionStatus("Host started.");
    }

    public void ConnectViaIp()
    {
        NetworkManager.singleton.networkAddress = ipAddress;
        NetworkManager.singleton.StartClient();
        SwitchPanels(PanelId.Status);
        disconnectButtonText.text = "Disconnect";

        ChangeConnectionStatus("Trying to connect...");
        StartTimeoutCheck();
    }

    public void StartServer()
    {
        NetworkManager.singleton.StartServer();
        SwitchPanels(PanelId.Status);
        disconnectButtonText.text = "Stop server";
    }
    
    #endregion

    #region GUIManagment
    public void SetPlayerName(string newPlayerName)
    {
        playerName = newPlayerName;
        PlayerPrefs.SetString("PlayerName", newPlayerName);
    }

    public void SetIpAddress(string ipAddress)
    {
        if (ipAddress == "localhost")
        {
            this.ipAddress = ipAddress;
            PlayerPrefs.SetString("LastIpAddress", ipAddress);
            ResetConnectIpButton();
            return;
        }
        
        if(!ipAddress.All(x => Char.IsDigit(x) || x == '.') || ipAddress.Length == 0)
        {
            IncorrectIpNotification();
            return;
        }
        
        string[] numStrings = ipAddress.Split('.');
        foreach (string numString in numStrings)
        {
            if (!Int32.TryParse(numString, out int num) || num > 255 || num < 0 || numStrings.Length != 4)
            {
                IncorrectIpNotification();
                return;
            }
            this.ipAddress = ipAddress;
            PlayerPrefs.SetString("LastIpAddress", ipAddress);
            ResetConnectIpButton();
        }
    }

    private void IncorrectIpNotification()
    {
        ipAddress = "";
        PlayerPrefs.SetString("LastIpAddress", "");
        connectButtonText.text = "Incorrect IP!";
        connectButton.interactable = false;
    }

    private void ResetConnectIpButton()
    {
        connectButtonText.text = connectButtonDefaultText;
        connectButton.interactable = true;
    }
    #endregion
    
    #endregion

    #region ConnectionStatusPanel

    public void RegisterServerClosedHandler()
    {
        NetworkClient.ReplaceHandler<ServerClosedMessage>(_ =>
        {
            ChangeConnectionStatus("Server is closed.");
            UiPanelManager.Instance.SetPanelState(PanelId.Network, true);
            Cursor.lockState = CursorLockMode.Confined;
        });
    }

    public void ChangeConnectionStatus(CustomNetworkAuthenticator.AuthResponseMessage message)
    {
        connectionStatus.text = message.Message;
    }

    public void ChangeConnectionStatus(string status)
    {
        connectionStatus.text = status;
    }

    public void StopConnection()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            ServerController.Instance.StopServer(true);
        }
        else if (NetworkClient.isConnected || NetworkClient.isConnecting)
        {
            NetworkManager.singleton.StopClient();
        }
        else if (NetworkServer.active)
        {
            ServerController.Instance.StopServer(true);
        }

        SwitchPanels(PanelId.Connection);
    }

    #endregion
}