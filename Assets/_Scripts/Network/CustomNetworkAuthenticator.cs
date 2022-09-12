using System.Collections;
using Mirror;
using UnityEngine;

/*
    Documentation: https://mirror-networking.gitbook.io/docs/components/network-authenticators
    API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkAuthenticator.html
*/

public class CustomNetworkAuthenticator : NetworkAuthenticator
{
    #region Messages

    public struct AuthRequestMessage : NetworkMessage
    {
        public string PlayerName;
    }

    public struct AuthResponseMessage : NetworkMessage
    {
        public string PlayerName;
        public int ConnectionId;
        public string Message;
    }

    public struct ConnectionInfoMessage : NetworkMessage
    {
        public string PlayerName;
        public int ConnectionId;
    }

    #endregion

    #region Server

    /// <summary>
    /// Called on server from StartServer to initialize the Authenticator
    /// <para>Server message handlers should be registered in this method.</para>
    /// </summary>
    public override void OnStartServer()
    {
        // register a handler for the authentication request we expect from client
        NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
    }

    /// <summary>
    /// Called on server from OnServerAuthenticateInternal when a client needs to authenticate
    /// </summary>
    /// <param name="conn">Connection to client.</param>
    public override void OnServerAuthenticate(NetworkConnectionToClient conn) { }

    /// <summary>
    /// Called on server when the client's AuthRequestMessage arrives
    /// </summary>
    /// <param name="conn">Connection to client.</param>
    /// <param name="msg">The message payload</param>
    public void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthRequestMessage msg)
    {
        if (ServerController.Instance.IsPlayerOnServer(msg.PlayerName))
        {
            AuthResponseMessage authResponseMessage = new AuthResponseMessage
            {
                PlayerName = msg.PlayerName,
                ConnectionId = conn.connectionId,
                Message = $"Access denied. Player \"{msg.PlayerName}\" is already playing on the server. ConnectionId: {conn.connectionId}."
            };
            
            conn.Send(authResponseMessage);
            
            conn.isAuthenticated = false;

            StartCoroutine(DelayedDisconnect(conn, 1f));
        }
        else
        {
            AuthResponseMessage authResponseMessage = new AuthResponseMessage
            {
                PlayerName = msg.PlayerName,
                ConnectionId = conn.connectionId,
                Message = $"Server accepted authentication. PlayerName: {msg.PlayerName}, ConnectionId: {conn.connectionId}."
            };
            
            conn.Send(authResponseMessage);
            
            ServerAccept(conn);
        }
    }
    
    IEnumerator DelayedDisconnect(NetworkConnectionToClient conn, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        ServerReject(conn);

        yield return null;
    }

    #endregion

    #region Client

    /// <summary>
    /// Called on client from StartClient to initialize the Authenticator
    /// <para>Client message handlers should be registered in this method.</para>
    /// </summary>
    public override void OnStartClient()
    {
        // register a handler for the authentication response we expect from server
        NetworkClient.ReplaceHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
    }

    /// <summary>
    /// Called on client from OnClientAuthenticateInternal when a client needs to authenticate
    /// </summary>
    public override void OnClientAuthenticate()
    {
        AuthRequestMessage authRequestMessage = new AuthRequestMessage
        {
            PlayerName = NetworkHudManager.Instance.PlayerName
        };

        NetworkClient.Send(authRequestMessage);
    }

    /// <summary>
    /// Called on client when the server's AuthResponseMessage arrives
    /// </summary>
    /// <param name="msg">The message payload</param>
    public void OnAuthResponseMessage(AuthResponseMessage msg)
    {
        // Authentication has been accepted
        
        NetworkHudManager.Instance.ChangeConnectionStatus(msg);
        NetworkHudManager.Instance.StopTimeoutCheck();
        
        ClientAccept();
        
        NetworkClient.Send(new ConnectionInfoMessage
        {
            PlayerName = msg.PlayerName,
            ConnectionId = msg.ConnectionId
        });
    }

    #endregion
}
