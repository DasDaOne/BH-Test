using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Utilities;
using Utilities.Singletons;

#region Structures

public struct PlayerData
{
    public int ConnectionId;
    public PlayerScoreData PlayerScoreData;
}

public struct PlayerScoreData
{
    public string PlayerName;
    public int Score;
}

public struct ScoreboardMessage : NetworkMessage
{
    public List<PlayerScoreData> PlayerScoreData;
    public int WinScore;
}

public struct WinnerNotificationMessage : NetworkMessage
{
    public string WinnerName;
}

public struct ServerClosedMessage : NetworkMessage { }

#endregion

public class ServerController : NetworkSingleton<ServerController>
{
    [SerializeField] private int winScore;
    [SerializeField] private float delayBeforeRestartGame;

    private List<PlayerData> playerData = new List<PlayerData>();

    private bool gameIsRunning = true;

    public bool GameIsRunning => gameIsRunning;

    private void Start()
    {
        NetworkServer.OnDisconnectedEvent += OnPlayerDisconnect;
    }

    private void OnDestroy()
    {
        NetworkServer.OnDisconnectedEvent -= OnPlayerDisconnect;
    }

    public void StopServer(bool isHost)
    {
        NetworkServer.SendToAll(new ServerClosedMessage());

        StartCoroutine(TimeUtilities.Timer(0.5f, () =>
        {
            if (isHost)
                NetworkManager.singleton.StopHost();
            else
                NetworkManager.singleton.StopServer();
        }));
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        
        playerData.Clear();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        
        NetworkServer.ReplaceHandler<CustomNetworkAuthenticator.ConnectionInfoMessage>(AddPlayer);
    }

    private void OnPlayerDisconnect(NetworkConnectionToClient connection)
    {
        RemovePlayer(connection);
    }

    public bool IsPlayerOnServer(string playerName)
    {
        return playerData.Any(x => x.PlayerScoreData.PlayerName == playerName);
    }
    
    private void AddPlayer(CustomNetworkAuthenticator.ConnectionInfoMessage message)
    {
        PlayerScoreData playerScoreData = new PlayerScoreData
        {
            PlayerName = message.PlayerName,
            Score = 0
        };
        playerData.Add(new PlayerData
        {
            ConnectionId = message.ConnectionId,
            PlayerScoreData = playerScoreData
        });
        SendScoreboard();
    }

    private void RemovePlayer(NetworkConnection connection)
    {
        if (playerData.Any(x => x.ConnectionId == connection.connectionId))
        {
            playerData.Remove(playerData.First(x => x.ConnectionId == connection.connectionId));
            SendScoreboard();
        }
    }

    public void AddScore(int connectionId)
    {
        PlayerData data = playerData.First(x => x.ConnectionId == connectionId);
        PlayerData newData = data;
        newData.PlayerScoreData.Score++;
        playerData.Remove(data);
        playerData.Add(newData);
        SendScoreboard();

        if (newData.PlayerScoreData.Score >= winScore)
            EndGame(newData.PlayerScoreData.PlayerName);
    }

    private void ResetScore(int connectionId)
    {
        PlayerData data = playerData.First(x => x.ConnectionId == connectionId);
        PlayerData newData = data;
        newData.PlayerScoreData.Score = 0;
        playerData.Remove(data);
        playerData.Add(newData);
        SendScoreboard();
    }

    private void SendScoreboard()
    {
        List<PlayerScoreData> data = playerData.Select(x => x.PlayerScoreData).ToList();
        
        Debug.Log("SendScoreboard");
        
        NetworkServer.SendToAll(new ScoreboardMessage
        {
            PlayerScoreData = data.OrderByDescending(x => x.Score).ToList(),
            WinScore = winScore
        });
    }

    private void EndGame(string winnerName)
    {
        SendWinnerNotification(winnerName);
        gameIsRunning = false;

        StartCoroutine(TimeUtilities.Timer(delayBeforeRestartGame, RestartGame));
        
        SendScoreboard();
    }

    private void RestartGame()
    {
        NetworkManager.singleton.playerSpawnMethod = PlayerSpawnMethod.RoundRobin;
            
        foreach (KeyValuePair<int, NetworkConnectionToClient> connection in NetworkServer.connections)
        {
            NetworkServer.DestroyPlayerForConnection(connection.Value);
            NetworkManager.singleton.OnServerAddPlayer(connection.Value);
        }
            
        NetworkManager.singleton.playerSpawnMethod = PlayerSpawnMethod.Random;

        gameIsRunning = true;
            
        foreach (KeyValuePair<int, NetworkConnectionToClient> connection in NetworkServer.connections)
        {
            ResetScore(connection.Value.connectionId);
        }
    }

    private void SendWinnerNotification(string winnerName)
    {
        NetworkServer.SendToAll(new WinnerNotificationMessage
        {
            WinnerName = winnerName
        });
    }
}
